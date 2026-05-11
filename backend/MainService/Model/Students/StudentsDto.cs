namespace MainService
{
    public class StudentsDto
    {
        public required Guid Uuid { get; set; }
        public required string StudentCode { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Patronymic { get; set; }
        public required Guid GroupUuid { get; set; }
        public Guid[]? BrigadesUuids { get; set; } = [];
        public Guid[]? NotesAboutStudentUuids { get; set; } = [];
        public Guid[]? LessonPresencesUuids { get; set; } = [];
        public Guid[]? LessonMarksUuids { get; set; } = [];
        public Guid[]? AttestationsUuids { get; set; } = [];

        public StudentsDto() { }

        public StudentsDto(Students student)
        {
            Uuid = student.Uuid;
            StudentCode = student.StudentCode;
            GroupUuid = student.Group!.Uuid;
            BrigadesUuids = student.Brigades?.Select(b => b.Uuid).ToArray() ?? [];
            NotesAboutStudentUuids = student.NotesAboutStudent?.Select(n => n.Uuid).ToArray() ?? [];
            LessonPresencesUuids = student.LessonPresences?.Select(lp => lp.Uuid).ToArray() ?? [];
            LessonMarksUuids = student.LessonMarks?.Select(lm => lm.Uuid).ToArray() ?? [];
            AttestationsUuids = student.Attestations?.Select(a => a.Uuid).ToArray() ?? [];
        }
    }
}
