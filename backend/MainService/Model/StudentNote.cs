using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class StudentNotes
    {
        [Key]
        public int StudentNoteId { get; set; }

        [Required]
        public required string NoteText { get; set; }

        public int StudentId { get; set; }
        public Students? Student { get; set; }
    }
}
