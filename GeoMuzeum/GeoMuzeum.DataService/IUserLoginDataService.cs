using GeoMuzeum.Model;
using System.Threading.Tasks;

namespace GeoMuzeum.DataService
{
    public interface IUserLoginDataService
    {
        Task<UserLogin> TryFoundUserByLogin(string userLogin);
        Task<UserLogin> GetUserLoginByUserId(User user);
        Task AddUserLogin(UserLogin userLogin);
        Task UpdateUserLogin(UserLogin userLogin);
        Task DeleteUserLogin(UserLogin userLogin);
        Task<bool> CheckUserLogin(string userLogin);
        Task<bool> CheckUserPin(int pin);
    }
}
