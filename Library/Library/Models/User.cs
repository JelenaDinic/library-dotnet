using Library.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class User : IdentityUser<int>
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string? Avatar { get; set; }
    }
}
