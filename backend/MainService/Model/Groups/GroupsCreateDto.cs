namespace MainService
{
    public class GroupsCreateDto
    {
        public required string Code { get; set; }
        public DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public required Guid TrainingDirectionUuid { get; set; }
        public required Guid FacultyUuid { get; set; }
        public Guid[]? CuratorsUuids { get; set; } = [];
    }
}
