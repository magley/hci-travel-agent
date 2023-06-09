using System.ComponentModel.DataAnnotations;

namespace YouTravel.Model
{
    public enum UserType { CLIENT, AGENT, }
    public class User
    {
        [Key]
        public int Id { get; set; }
        public UserType Type { get; set; } = UserType.CLIENT;
        public string Username { get; set; } = "";
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; } = "";
    }
}
