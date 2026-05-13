using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class StudentPersons : BaseEntity
    {
        [Key]
        public int StudentPersonId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }

        public ICollection<Students>? Students { get; set; } = [];
    }
}