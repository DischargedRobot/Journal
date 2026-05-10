using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MainService
{
    public class UniversityEmployers
    {
        [Key]
        public int UniversityEmployerId { get; set; }

        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }

        public ICollection<Professors>? Professors { get; set; } = [];

        public ICollection<MarkTypes>? MarkTypes { get; set; } = [];

    }
}
