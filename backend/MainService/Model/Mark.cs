using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Marks
    {
        [Key]
        public int MarkId { get; set; }

        [Required]
        public required string Value { get; set; }

        public int? TypeOfAssessmentId { get; set; }
        public TypeOfAssessments? TypeOfAssessment { get; set; }

        public ICollection<LessonMarks>? LessonMarks { get; set; }
    }
}