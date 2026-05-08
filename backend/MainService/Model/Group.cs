using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Groups
    {
        [Key]
        public int GroupId { get; set; }

        [Required]
        public required DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        [Required]
        public required string Code { get; set; }

        public ICollection<Brigades>? Brigades { get; set; }

        public int? TrainingDirectionId { get; set; }
        public TrainingDirections? TrainingDirection { get; set; }

        public ICollection<Students>? Students { get; set; }
    }
}
