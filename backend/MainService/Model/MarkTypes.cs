using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class MarkTypes
    {
        [Key]
        public int MarkTypeId { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<Marks>? Marks { get; set; }

        public int? UniversityEmployerId { get; set; }
        [ForeignKey("UniversityEmployerId")]
        public UniversityEmployers? UniversityEmployer { get; set; }
    }
}