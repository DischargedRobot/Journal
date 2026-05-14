namespace MainService
{
    public class BrigadesCreateDto
    {
        public required string Name { get; set; }
        public bool IsTemplateForGroup { get; set; } = false;
        public required Guid GroupUuid { get; set; }
        public Guid[] StudentsUuids { get; set; } = [];
        public Guid[]? DisciplinesUuids { get; set; } = [];
    }
}
