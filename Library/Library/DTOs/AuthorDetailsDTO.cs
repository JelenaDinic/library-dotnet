using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class AuthorDetailsDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
