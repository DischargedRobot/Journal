using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Attestations
    {
        [Key]
        public int AttestationId { get; set; }

        [Required]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public int AttestationTypeId { get; set; }
        public AttestationTypes? AttestationType { get; set; }

        public ICollection<AttestationMarks>? AttestationMarks { get; set; }
    }
}
