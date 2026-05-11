namespace MainService
{
    public class NotesAboutStudentDto
    {
        public required Guid Uuid { get; set; }
        public required string NoteText { get; set; }
        public required Guid StudentUuid { get; set; }

        public NotesAboutStudentDto() { }

        public NotesAboutStudentDto(NotesAboutStudent note)
        {
            Uuid = note.Uuid;
            NoteText = note.NoteText;
            StudentUuid = note.Student!.Uuid;
        }
    }
}
