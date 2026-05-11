namespace MainService
{
    public class AcademicYearsDto
    {
        public Guid Uuid { get; set; }

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public Guid[]? DisciplinesUuids { get; set; } = [];

        public AcademicYearsDto() { }

        public AcademicYearsDto(AcademicYears academicYear)
        {
            Uuid = academicYear.Uuid;
            Date = academicYear.Date;
            DisciplinesUuids = academicYear.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
        }
    }
}