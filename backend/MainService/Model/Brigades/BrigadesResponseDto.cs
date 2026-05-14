namespace MainService
{
    public class BrigadesResponseDto
    {
        public Guid Uuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsTemplateForGroup { get; set; }
        public Guid? GroupUuid { get; set; }
        public Guid[] StudentsUuids { get; set; } = [];
        public Guid[]? DisciplinesUuids { get; set; } = [];

        public int Version { get; set; }
        public BrigadesResponseDto() { }

        public BrigadesResponseDto(Brigades brigade)
        {
            Uuid = brigade.Uuid;
            Name = brigade.Name;
            IsTemplateForGroup = brigade.IsTemplateForGroup;
            GroupUuid = brigade.Group?.Uuid;
            StudentsUuids = brigade.Students.Select(s => s.Uuid).ToArray() ?? [];
            DisciplinesUuids = brigade.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
            Version = brigade.Version;
        }
    }
}
