using Library.Enums;
using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class UserRegistrationDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", ErrorMessage = "Password must contain at least one uppercase, lowercase letter, digit, special character and minimum 8 in length")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [RegularExpression("USER|LIBRARIAN", ErrorMessage = "Only USER or LIBRARIAN can be registered.")]
        public UserRole Role { get; set; }
    }
}
