using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class AttestationTypes
    {
        [Key]
        public int AttestationTypeId { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<Attestations>? Attestations { get; set; }
        public ICollection<AttestationMarks>? AttestationMarks { get; set; }
    }
}