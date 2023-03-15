using Library.Enums;
using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class UserProfileDTO
    {
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Password must contain at least one uppercase, lowercase letter, digit, special character and minimum 8 in length")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
