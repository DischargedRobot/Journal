using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService
{
    public class Brigades
    {
        [Key]
        public int BrigadeId { get; set; }

        public required string Name { get; set; }

        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public Groups? Group { get; set; }

        [Required, MinLength(1)]
        public required ICollection<Students> Students { get; set; } = [];

        public ICollection<Disciplines>? Disciplines { get; set; } = [];
    }
}
