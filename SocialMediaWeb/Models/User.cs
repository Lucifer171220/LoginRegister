using System.ComponentModel.DataAnnotations;

namespace SocialMediaWeb.Models
{
    public class User
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Dob { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? PasswordHash { get; set; }

        public string? Role { get; set; }

        public string? ImagePath { get; set; }

    }
}
