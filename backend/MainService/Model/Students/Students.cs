using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class Students : BaseEntity
    {
        [Key]
        public int StudentId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string StudentCode { get; set; }

        public required int StudentPersonId { get; set; }
        [ForeignKey("StudentPersonId")]
        public StudentPersons? StudentPerson { get; set; }

        public required int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Groups? Group { get; set; }

        public ICollection<Brigades>? Brigades { get; set; } = [];

        public ICollection<NotesAboutStudent>? NotesAboutStudent { get; set; } = [];

        public ICollection<LessonPresences>? LessonPresences { get; set; } = [];
        public ICollection<LessonMarks>? LessonMarks { get; set; } = [];

        public ICollection<Attestations>? Attestations { get; set; } = [];
    }
}
