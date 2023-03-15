using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Author : EntityBase
    {
        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }
        public List<BookAuthor> AuthorBooks { get; set; }
    }
}
