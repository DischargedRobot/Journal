using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class Attestations : BaseEntity
    {
        [Key]
        public int AttestationId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        public required int AttestationTypeId { get; set; }
        [ForeignKey("AttestationTypeId")]
        public AttestationTypes? AttestationType { get; set; }


        public int? AttestationMarkId { get; set; }
        [ForeignKey("AttestationMarkId")]
        public AttestationMarks? AttestationMark { get; set; }

        public required int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Students? Student { get; set; }

        public required int DisciplineId { get; set; }
        [ForeignKey("DisciplineId")]
        public Disciplines? Discipline { get; set; }
    }
}
