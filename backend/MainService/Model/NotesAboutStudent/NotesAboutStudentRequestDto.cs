namespace MainService
{
    public class NotesAboutStudentRequestDto
    {
        public required Guid Uuid { get; set; }
        public required string NoteText { get; set; }
        public required Guid StudentUuid { get; set; }

        public int Version { get; set; }
        public NotesAboutStudentRequestDto() { }

        public NotesAboutStudentRequestDto(NotesAboutStudent note)
        {
            Uuid = note.Uuid;
            NoteText = note.NoteText;
            StudentUuid = note.Student!.Uuid;
            Version = note.Version;
        }
    }
}
