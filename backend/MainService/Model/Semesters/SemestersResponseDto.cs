namespace MainService
{
    public class SemestersResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string SemesterName { get; set; }
        public required int SemesterCode { get; set; }
        public Guid[]? DisciplinesUuids { get; set; } = [];

        public int Version { get; set; }
        public SemestersResponseDto() { }

        public SemestersResponseDto(Semesters semester)
        {
            Uuid = semester.Uuid;
            SemesterName = semester.SemesterName;
            SemesterCode = semester.SemesterCode;
            DisciplinesUuids = semester.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
            Version = semester.Version;
        }
    }
}
