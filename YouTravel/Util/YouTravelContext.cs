using System;
using System.Linq;
using YouTravel.Model;

namespace YouTravel.Util
{
    public static class YouTravelContext
    {
        public static UserConfig UserConfig { get; private set; }
        public static User? User { get; set; }

        static YouTravelContext()
        {
            UserConfig = UserConfig.Instance;
            using var db = new TravelContext();
            try
            {
                User = db.Users.Single(u => u.Username == UserConfig.LoggedInUserUsername);
            }
            catch (InvalidOperationException) { }
        }
    }
}