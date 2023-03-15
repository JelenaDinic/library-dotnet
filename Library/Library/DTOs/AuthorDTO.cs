using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class AuthorDTO
    {
        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }
    }
}
