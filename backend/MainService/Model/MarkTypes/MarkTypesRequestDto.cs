namespace MainService
{
    public class MarkTypesRequestDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public Guid? UniversityEmployerUuid { get; set; }
        public Guid[]? MarksUuids { get; set; } = [];

        public int Version { get; set; }
        public MarkTypesRequestDto() { }

        public MarkTypesRequestDto(MarkTypes markType)
        {
            Uuid = markType.Uuid;
            Name = markType.Name;
            UniversityEmployerUuid = markType.UniversityEmployer?.Uuid;
            MarksUuids = markType.Marks?.Select(m => m.Uuid).ToArray() ?? [];
            Version = markType.Version;
        }
    }
}
