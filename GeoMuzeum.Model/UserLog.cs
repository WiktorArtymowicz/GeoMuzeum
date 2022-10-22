using System.ComponentModel.DataAnnotations;

namespace GeoMuzeum.Model
{
    public class UserLog
    {
        public UserLog()
        {

        }

        public UserLog(string logDescription, User user)
        {
            LogDescription = logDescription;
            User = user;
        }

        public UserLog(int userLogId, string logDescription, User user)
        {
            UserLogId = userLogId;
            LogDescription = logDescription;
            User = user;
        }

        public int UserLogId { get; set; }
        public string LogDescription { get; set; }
        public virtual User User { get; set; }
    }
}
