using System.ComponentModel.DataAnnotations;

namespace GeoMuzeum.Model
{
    public class UserLogin
    {
        public UserLogin()
        {

        }

        public UserLogin(int userLoginId, string login, int pinNumber, User user)
        {
            UserLoginId = userLoginId;
            Login = login;
            PinNumber = pinNumber;
            User = user;
        }

        public int UserLoginId { get; set; }
        public string Login { get; set; }
        public int PinNumber { get; set; }

        [Required]
        public User User { get; set; }
    }
}
