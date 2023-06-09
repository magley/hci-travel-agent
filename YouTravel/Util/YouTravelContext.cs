using System;
using System.ComponentModel;
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
            using var db = new TravelContext();
            try
            {
                User = db.Users.Single(u => u.Username == UserConfig.LoggedInUserUsername);
            }
            catch (InvalidOperationException) { }
        }

        public static void Logout()
        {
            User = null;
        }

        public static void Login(string username, string password)
        {
            using var db = new TravelContext();
            try
            {
                User = db.Users.Single(u => u.Username == username && u.Password == password);
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