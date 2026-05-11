namespace MainService
{
    public class AttestationTypesDto
    {
        public Guid Uuid { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid[]? AttestationsUuids { get; set; } = [];
        public Guid[]? AttestationMarksUuids { get; set; } = [];

        public AttestationTypesDto() { }

        public AttestationTypesDto(AttestationTypes attestationType)
        {
            Uuid = attestationType.Uuid;
            Name = attestationType.Name;
            AttestationsUuids = attestationType.Attestations?.Select(a => a.Uuid).ToArray() ?? [];
            AttestationMarksUuids = attestationType.AttestationMarks?.Select(a => a.Uuid).ToArray() ?? [];
        }
    }
}
