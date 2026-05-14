namespace MainService
{
    public class GroupsUpdateDto
    {
        public string? Code { get; set; }
        public DateOnly? AdmissionDate { get; set; }
        public Guid? TrainingDirectionUuid { get; set; }
        public Guid? FacultyUuid { get; set; }
        public Guid[]? CuratorsUuids { get; set; }
    }
}
