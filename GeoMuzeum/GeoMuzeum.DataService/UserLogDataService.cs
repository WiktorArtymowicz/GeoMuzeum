using GeoMuzeum.DataModel;
using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public class UserLogDataService : IUserLogDataService
    {
        public async Task<List<UserLog>> GetAllUserLogs()
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.UserLogs.AsNoTracking().Include(x => x.User).AsNoTracking().ToListAsync();
            }
        }

        public async Task AddUserLog(UserLog userLog)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                try
                {
                    var user = await dbContext.Users.FindAsync(userLog.User.UserId);

                    dbContext.Entry(user).State = EntityState.Unchanged;
                    dbContext.Users.Attach(user);

                    userLog.User = user;

                    dbContext.UserLogs.Add(userLog);
                    dbContext.Entry(userLog).State = EntityState.Added;
                    await dbContext.SaveChangesAsync();
                }
                catch (System.Exception exception)
                {
                    throw exception;
                }
            }
        }

        public async Task<List<UserLog>> GetUserLogsByUserName(string userName)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.UserLogs.AnyAsync())
                    return new List<UserLog>();

                return await dbContext.UserLogs.AsNoTracking().Where(x => x.User.UserName.ToLower().Contains(userName.ToLower())).Include(x => x.User).ToListAsync();
            }
        }

        public async Task<List<UserLog>> GetUserLogsByDescription(string description)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                if (!await dbContext.UserLogs.AnyAsync())
                    return new List<UserLog>();

                return await dbContext.UserLogs.AsNoTracking().Where(x => x.LogDescription.ToLower().Contains(description.ToLower())).Include(x => x.User).ToListAsync();
            }
        }

        public async Task<bool> AnyLogsByUser(User user)
        {
            using (var dbContext = new GeoMuzeumContext())
            {
                return await dbContext.UserLogs.AsNoTracking().Include(x => x.User).Where(x => x.User.UserId == user.UserId).AsNoTracking().AnyAsync();
            }
        }
    }
}
