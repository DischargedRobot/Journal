namespace MainService
{
    public class ProfessorsCreateDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Patronymic { get; set; }
        public required Guid AcademicYearUuid { get; set; }
        public Guid? DepartmentUuid { get; set; }
        public Guid? PostUuid { get; set; }
    }
}