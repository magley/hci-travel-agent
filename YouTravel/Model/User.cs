using System;
using System.ComponentModel.DataAnnotations;

namespace YouTravel.Model
{
    public enum UserType { CLIENT, AGENT, }
    public class User
    {
        [Key]
        public int Id { get; set; }
        public UserType Type { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
