namespace MainService
{
    public class BrigadesCreateDto
    {
        public required string Name { get; set; }
        public bool IsTemplateForGroup { get; set; } = false;
        public required Guid GroupUuid { get; set; }
        public Guid[] StudentsUuids { get; set; } = [];
        public Guid[]? DisciplinesUuids { get; set; } = [];

        public static BrigadesCreateDto Example => new()
        {
            Name = "Новая бригада",
            IsTemplateForGroup = false,
            GroupUuid = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            StudentsUuids = [Guid.Parse("33333333-3333-3333-3333-333333333333")],
            DisciplinesUuids = [Guid.Parse("44444444-4444-4444-4444-444444444444")]
        };
    }
}
