using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
