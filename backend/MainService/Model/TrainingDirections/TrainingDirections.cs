using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MainService
{
    public class TrainingDirections
    {
        [Key]
        public int TrainingDirectionId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Code { get; set; }

        public ICollection<Groups>? Groups { get; set; }
    }
}
