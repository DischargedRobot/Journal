using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class NotesAboutStudent
    {
        [Key]
        public int StudentNoteId { get; set; }

        [Required]
        public required string NoteText { get; set; }

        public required int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Students? Student { get; set; }
    }
}
