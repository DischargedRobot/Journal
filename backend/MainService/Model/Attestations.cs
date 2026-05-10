using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class Attestations
    {
        [Key]
        public int AttestationId { get; set; }

        [Required]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public int AttestationTypeId { get; set; }
        [ForeignKey("AttestationTypeId")]
        public AttestationTypes? AttestationType { get; set; }

        public AttestationMarks? AttestationMark { get; set; }

        public required int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Students? Student { get; set; }

        public required int DisciplineId { get; set; }
        [ForeignKey("DisciplineId")]
        public Disciplines? Discipline { get; set; }
    }
}
