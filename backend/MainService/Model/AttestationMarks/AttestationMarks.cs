using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class AttestationMarks
    {
        [Key]
        public int AttestationMarkId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Mark { get; set; }

        public ICollection<Attestations>? Attestation { get; set; }

        public required int AttestationTypeId { get; set; }
        [ForeignKey("AttestationTypeId")]
        public AttestationTypes? AttestationType { get; set; }
    }
}