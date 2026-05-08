using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class TypeOfAssessments
    {
        [Key]
        public int TypeOfAssessmentId { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<Marks>? Marks { get; set; }
    }
}