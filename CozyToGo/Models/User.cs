using System.ComponentModel.DataAnnotations;

namespace CozyToGo.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; } = "User";

    }
}
