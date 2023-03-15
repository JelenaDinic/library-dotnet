using Library.Enums;
using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class BookDTO
    {
        [MaxLength(250)]
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression("ENGLISH|GERMAN|RUSSIAN|SERBIAN", ErrorMessage = "Available languages: ENGLISH, GERMAN, RUSSIAN and SERBIAN")]
        public Language Language { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
