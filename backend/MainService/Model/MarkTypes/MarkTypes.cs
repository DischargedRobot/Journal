using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class MarkTypes : BaseEntity
    {
        [Key]
        public int MarkTypeId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        public ICollection<Marks>? Marks { get; set; }

        public int? UniversityEmployerId { get; set; }
        [ForeignKey("UniversityEmployerId")]
        public UniversityEmployers? UniversityEmployer { get; set; }
    }
}