namespace MainService
{
    public class AcademicYearsRequestDto
    {
        public Guid Uuid { get; set; }

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public Guid[]? DisciplinesUuids { get; set; } = [];

        public int Version { get; set; }
        public AcademicYearsRequestDto() { }

        public AcademicYearsRequestDto(AcademicYears academicYear)
        {
            Uuid = academicYear.Uuid;
            Date = academicYear.Date;
            DisciplinesUuids = academicYear.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
            Version = academicYear.Version;
        }
    }
}
