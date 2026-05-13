namespace MainService
{
    public class AttestationTypesRequestDto
    {
        public Guid Uuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid[]? AttestationsUuids { get; set; } = [];
        public Guid[]? AttestationMarksUuids { get; set; } = [];

        public int Version { get; set; }
        public AttestationTypesRequestDto() { }

        public AttestationTypesRequestDto(AttestationTypes attestationType)
        {
            Uuid = attestationType.Uuid;
            Name = attestationType.Name;
            AttestationsUuids = attestationType.Attestations?.Select(a => a.Uuid).ToArray() ?? [];
            AttestationMarksUuids = attestationType.AttestationMarks?.Select(a => a.Uuid).ToArray() ?? [];
            Version = attestationType.Version;
        }
    }
}
