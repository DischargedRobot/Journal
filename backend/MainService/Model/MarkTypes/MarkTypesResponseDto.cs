using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class MarkTypesResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public Guid? UniversityEmployerUuid { get; set; }
        public Guid[]? MarksUuids { get; set; } = [];

        public int Version { get; set; }
        public MarkTypesResponseDto() { }

        [SetsRequiredMembers]
        public MarkTypesResponseDto(MarkTypes markType)
        {
            Uuid = markType.Uuid;
            Name = markType.Name;
            UniversityEmployerUuid = markType.UniversityEmployer?.Uuid;
            MarksUuids = markType.Marks?.Select(m => m.Uuid).ToArray() ?? [];
            Version = markType.Version;
        }
    }
}
