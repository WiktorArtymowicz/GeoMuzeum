using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class UserLoginDataService : IUserLoginDataService
    {
        public async Task<UserLogin> TryFoundUserByLogin(string userLogin)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.UserLogins.AsNoTracking().Include(x => x.User).FirstOrDefaultAsync(x => x.Login == userLogin);
            }
        }

        public async Task<UserLogin> GetUserLoginByUserId(User user)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.UserLogins.AsNoTracking().Include(x => x.User).FirstOrDefaultAsync(x => x.User.UserId == user.UserId);
            }
        }

        public async Task AddUserLogin(UserLogin userLogin)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var foundUser = dbContext.Users.ToList().LastOrDefault();

                    dbContext.Entry(foundUser).State = EntityState.Unchanged;
                    dbContext.Users.Attach(foundUser);

                    userLogin.User = foundUser;

                    dbContext.UserLogins.Add(userLogin);
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task UpdateUserLogin(UserLogin userLogin)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundUserLogin = await dbContext.UserLogins.FindAsync(userLogin.UserLoginId);
                var foundUser = await dbContext.Users.FindAsync(userLogin.User.UserId);

                dbContext.Entry(foundUser).State = EntityState.Unchanged;

                dbContext.UserLogins.Attach(foundUserLogin);
                dbContext.Users.Attach(foundUser);

                foundUserLogin.Login = userLogin.Login;
                foundUserLogin.PinNumber = userLogin.PinNumber;
                foundUserLogin.User = foundUser;

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteUserLogin(UserLogin userLogin)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundUserLogin = await dbContext.UserLogins.FindAsync(userLogin.UserLoginId);

                dbContext.Entry(foundUserLogin).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckUserLogin(string userLogin)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.UserLogins.AnyAsync(x => x.Login == userLogin);
            }
        }

        public async Task<bool> CheckUserPin(int pin)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.UserLogins.AnyAsync(x => x.PinNumber == pin);
            }
        }
    }
}
