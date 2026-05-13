namespace MainService
{
    public class BrigadesRequestDto
    {
        public Guid Uuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? GroupUuid { get; set; }
        public Guid[] StudentsUuids { get; set; } = [];
        public Guid[]? DisciplinesUuids { get; set; } = [];

        public int Version { get; set; }
        public BrigadesRequestDto() { }

        public BrigadesRequestDto(Brigades brigade)
        {
            Uuid = brigade.Uuid;
            Name = brigade.Name;
            GroupUuid = brigade.Group?.Uuid;
            StudentsUuids = brigade.Students.Select(s => s.Uuid).ToArray() ?? [];
            DisciplinesUuids = brigade.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
            Version = brigade.Version;
        }
    }
}
