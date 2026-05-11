namespace MainService
{
    public class DepartmentsDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public required string ShortName { get; set; }
        public required string Code { get; set; }
        public required Guid FacultyUuid { get; set; }
        public Guid[]? ProfessorsUuids { get; set; } = [];

        public DepartmentsDto() { }

        public DepartmentsDto(Departments department)
        {
            Uuid = department.Uuid;
            Name = department.Name;
            ShortName = department.ShortName;
            Code = department.Code;
            FacultyUuid = department.Faculty!.Uuid;
            ProfessorsUuids = department.Professors?.Select(p => p.Uuid).ToArray() ?? [];
        }
    }
}
