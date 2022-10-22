using GeoMuzeum.Model.Enums;

namespace GeoMuzeum.Model
{
    public class User
    {
        public User()
        {

        }

        public User(int userId, string userName, string userSurname, UserPosition userPosition)
        {
            UserId = userId;
            UserName = userName;
            UserSurname = userSurname;
            UserPosition = userPosition;
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public UserPosition UserPosition { get; set; }

        public string UserPositionInfo => UserPosition.ToString();
    }
}
