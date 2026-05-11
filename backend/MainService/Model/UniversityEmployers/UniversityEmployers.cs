using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MainService
{
    public class UniversityEmployers
    {
        [Key]
        public required int UniversityEmployerId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }

        public ICollection<Professors>? Professors { get; set; } = [];

        public ICollection<MarkTypes>? MarkTypes { get; set; } = [];

    }
}
