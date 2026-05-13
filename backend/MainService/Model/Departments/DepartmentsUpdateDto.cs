namespace MainService
{
    public class DepartmentsUpdateDto
    {
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? Code { get; set; }
        public Guid? FacultyUuid { get; set; }
        public int Version { get; set; }

    }
}
