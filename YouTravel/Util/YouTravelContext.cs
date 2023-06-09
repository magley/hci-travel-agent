using System;
using System.Linq;
using YouTravel.Model;

namespace YouTravel.Util
{
    public static class YouTravelContext
    {
        public static UserConfig UserConfig { get; private set; }
        public static User? User { get; private set; }

        static YouTravelContext()
        {
            UserConfig = UserConfig.Instance;
            var username = UserConfig.LoggedInUserUsername;
            if (UserConfig.LoggedInUserUsername == "") return;
            using var db = new TravelContext();
            try
            {
                User = db.Users.Single(u => u.Username == username);
            }
            catch (InvalidOperationException)
            {
#if DEBUG
                Console.WriteLine($"Preferences error: '{username}' not in users table, defaulting to null");
#endif
                User = null;
                UserConfig.LoggedInUserUsername = "";
                UserConfig.Save();
            }
        }

        public static void Logout()
        {
            User = null;
            UserConfig.LoggedInUserUsername = "";
            UserConfig.Save();
        }

        public static void Login(string username, string password)
        {
            using var db = new TravelContext();
            try
            {
                User = db.Users.Single(u => u.Username == username && u.Password == password);
                UserConfig.LoggedInUserUsername = User.Username;
                UserConfig.Save();
            }
            catch (InvalidOperationException)
            {
                throw new LoginFailedException();
            }
        }
    }

    public class LoginFailedException : Exception
    {
    }
}