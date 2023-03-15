using Library.Enums;

namespace Library.DTOs
{
    public class BookDetailsDTO
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public Language Language { get; set; }
        public int Quantity { get; set; }
        public List<AuthorDetailsDTO> Authors { get; set; }
    }
}
