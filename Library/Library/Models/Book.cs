using Library.Enums;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Book : EntityBase
    {
        [MaxLength(250)]
        [Required]
        public string Name { get; set; }
        [Required]
        public Language Language { get; set; }
        [Required]
        public int Quantity { get; set; }
        public List<BookAuthor> AuthorBooks { get; set; }
    }
}
