using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Students
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public required string StudentCode { get; set; }

        public int? GroupId { get; set; }
        public Groups? Group { get; set; }

        public ICollection<Brigades>? Brigades { get; set; }

        public ICollection<StudentNotes>? Notes { get; set; }


        public ICollection<LessonPresences>? LessonPresences { get; set; }
        public ICollection<LessonMarks>? LessonMarks { get; set; }
    }
}
