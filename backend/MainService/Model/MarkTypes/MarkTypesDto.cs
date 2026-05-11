namespace MainService
{
    public class MarkTypesDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public Guid? UniversityEmployerUuid { get; set; }
        public Guid[]? MarksUuids { get; set; } = [];

        public MarkTypesDto() { }

        public MarkTypesDto(MarkTypes markType)
        {
            Uuid = markType.Uuid;
            Name = markType.Name;
            UniversityEmployerUuid = markType.UniversityEmployer?.Uuid;
            MarksUuids = markType.Marks?.Select(m => m.Uuid).ToArray() ?? [];
        }
    }
}
