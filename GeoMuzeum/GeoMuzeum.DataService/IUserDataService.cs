using GeoMuzeum.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IUserDataService
    {
        Task<List<User>> GetAllUsers();
        Task<List<User>> GetUsersByName(string name);
        Task<List<User>> GetUsersByPosition(string position);
        Task<User> GetUserById(User user);
        Task AddNewUser(User user);
        Task UpdateUser(User user);
        Task DeleteUser(User user);
    }
}