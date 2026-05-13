namespace MainService
{
    public class AcademicYearsResponseDto
    {
        public Guid Uuid { get; set; }

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public Guid[]? DisciplinesUuids { get; set; } = [];

        public int Version { get; set; }
        public AcademicYearsResponseDto() { }

        public AcademicYearsResponseDto(AcademicYears academicYear)
        {
            Uuid = academicYear.Uuid;
            Date = academicYear.Date;
            DisciplinesUuids = academicYear.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
            Version = academicYear.Version;
        }
    }
}
