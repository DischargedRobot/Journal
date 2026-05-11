namespace MainService
{
    public class SemestersDto
    {
        public required Guid Uuid { get; set; }
        public required string SemesterName { get; set; }
        public required int SemesterCode { get; set; }
        public Guid[]? DisciplinesUuids { get; set; } = [];

        public SemestersDto() { }

        public SemestersDto(Semesters semester)
        {
            Uuid = semester.Uuid;
            SemesterName = semester.SemesterName;
            SemesterCode = semester.SemesterCode;
            DisciplinesUuids = semester.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
        }
    }
}
