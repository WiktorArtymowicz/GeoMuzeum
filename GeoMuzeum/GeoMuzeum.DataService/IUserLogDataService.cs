using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IUserLogDataService
    {
        Task<List<UserLog>> GetAllUserLogs();
        Task<List<UserLog>> GetUserLogsByUserName(string userName);
        Task<List<UserLog>> GetUserLogsByDescription(string description);
        Task<bool> AnyLogsByUser(User user);
        Task AddUserLog(UserLog userLog);
    }
}