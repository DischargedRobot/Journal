namespace MainService
{
    public class BrigadesUpdateDto
    {
        public string? Name { get; set; }
        public bool IsTemplateForGroup { get; set; } = false;
        public Guid? GroupUuid { get; set; }
        public Guid[]? StudentsUuids { get; set; }
        public Guid[]? DisciplinesUuids { get; set; }
    }
}
