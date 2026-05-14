namespace MainService
{
    public class ProfessorsUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public Guid? AcademicYearUuid { get; set; }
        public Guid? DepartmentUuid { get; set; }
        public Guid? PostUuid { get; set; }
    }
}