using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class AttestationMarksDto
    {
        public required Guid Uuid { get; set; }
        public required string Mark { get; set; }
        public required Guid AttestationTypeUuid { get; set; }
        public Guid[]? AttestationsUuids { get; set; } = [];

        public AttestationMarksDto() { }

        [SetsRequiredMembers]
        public AttestationMarksDto(AttestationMarks attestationMark)
        {
            Uuid = attestationMark.Uuid;
            Mark = attestationMark.Mark;
            AttestationTypeUuid = attestationMark.AttestationType!.Uuid;
            AttestationsUuids = attestationMark.Attestation?.Select(a => a.Uuid).ToArray() ?? [];
        }
    }
}
