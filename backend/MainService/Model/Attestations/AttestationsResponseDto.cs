namespace MainService
{
    public class AttestationsResponseDto
    {
        public Guid Uuid { get; set; }
        public DateOnly Date { get; set; }
        public Guid AttestationTypeUuid { get; set; }
        public Guid? AttestationMarkUuid { get; set; }
        public Guid StudentUuid { get; set; }
        public Guid DisciplineUuid { get; set; }

        public int Version { get; set; }
        public AttestationsResponseDto() { }

        public AttestationsResponseDto(Attestations attestation)
        {
            Uuid = attestation.Uuid;
            Date = attestation.Date;
            AttestationTypeUuid = attestation.AttestationType!.Uuid;
            AttestationMarkUuid = attestation.AttestationMark?.Uuid;
            StudentUuid = attestation.Student!.Uuid;
            DisciplineUuid = attestation.Discipline!.Uuid;
            Version = attestation.Version;
        }
    }
}
