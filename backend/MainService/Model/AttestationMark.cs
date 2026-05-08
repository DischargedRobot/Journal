using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class AttestationMarks
    {
        [Key]
        public int AttestationMarkId { get; set; }

        [Required]
        public required string Mark { get; set; }

        public int AttestationId { get; set; }
        public Attestations? Attestation { get; set; }

        public int AttestationTypeId { get; set; }
        public AttestationTypes? AttestationType { get; set; }
    }
}