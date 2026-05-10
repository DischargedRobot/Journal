using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class StudentPersons
    {
        [Key]
        public int StudentPersonId { get; set; }

        public required int UserId { get; set; }
        [ForeignKey("UserId")]
        public required Users User { get; set; }

        public ICollection<Students>? Students { get; set; } = [];
    }
}