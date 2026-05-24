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

        public static BrigadesResponseDto Example => new()
        {
            Uuid = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Бригада 1",
            IsTemplateForGroup = false,
            GroupUuid = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            StudentsUuids = [Guid.Parse("33333333-3333-3333-3333-333333333333")],
            DisciplinesUuids = [Guid.Parse("44444444-4444-4444-4444-444444444444")],
            Version = 1
        };
    }
}
