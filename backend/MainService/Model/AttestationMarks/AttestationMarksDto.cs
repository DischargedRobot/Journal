namespace MainService
{
    public class AttestationMarksDto
    {
        public Guid Uuid { get; set; }
        public string Mark { get; set; } = string.Empty;
        public Guid AttestationTypeUuid { get; set; }
        public Guid[]? AttestationsUuids { get; set; } = [];

        public AttestationMarksDto() { }

        public AttestationMarksDto(AttestationMarks attestationMark)
        {
            Uuid = attestationMark.Uuid;
            Mark = attestationMark.Mark;
            AttestationTypeUuid = attestationMark.AttestationType!.Uuid;
            AttestationsUuids = attestationMark.Attestation?.Select(a => a.Uuid).ToArray() ?? [];
        }
    }
}
