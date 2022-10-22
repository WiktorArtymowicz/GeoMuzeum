using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class UserDataService : IUserDataService
    {
        public async Task<List<User>> GetAllUsers()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Users.AsNoTracking().ToListAsync();
            }
        }

        public async Task<List<User>> GetUsersByName(string name)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Users.AnyAsync())
                    return new List<User>();

                return await dbContext.Users.AsNoTracking().Where(x => x.UserName.ToLower().Contains(name.ToLower())).ToListAsync();
            }
        }

        public async Task<List<User>> GetUsersByPosition(string position)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.Users.AnyAsync())
                    return new List<User>();

                return await dbContext.Users.AsNoTracking().Where(x => x.UserPosition.ToString().ToLower().Contains(position.ToLower())).ToListAsync();
            }
        }

        public async Task<User> GetUserById(User user)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == user.UserId);
            }
        }

        public async Task AddNewUser(User user)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                using (var dbContext = new GeoMuzeumContext())
                {
                    var foundUser = await dbContext.Users.FindAsync(user.UserId);

                    foundUser.UserName = user.UserName;
                    foundUser.UserSurname = user.UserSurname;
                    foundUser.UserPosition = user.UserPosition;

                    dbContext.Entry(foundUser).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
        }

        public async Task DeleteUser(User user)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                var foundUser = await dbContext.Users.FindAsync(user.UserId);

                dbContext.Entry(foundUser).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
