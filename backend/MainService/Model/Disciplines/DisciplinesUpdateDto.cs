namespace MainService
{
    public class DisciplinesUpdateDto
    {
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public bool? IsArchived { get; set; }
        public Guid? DisciplineRegisterUuid { get; set; }
        public Guid? SemesterUuid { get; set; }
        public Guid? AcademicYearUuid { get; set; }
        public Guid[]? GroupsUuids { get; set; }
        public Guid[]? ProfessorsUuids { get; set; }
    }
}
