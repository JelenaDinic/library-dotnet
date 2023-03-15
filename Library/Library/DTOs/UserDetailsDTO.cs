using Library.Enums;
using Library.Models;
using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class UserDetailsDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; }
    }
}
