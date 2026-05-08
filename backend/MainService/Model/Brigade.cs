using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class Brigades
    {
        [Key]
        public int BrigadeId { get; set; }

        [Required]
        public required string Name { get; set; }

        public int GroupId { get; set; }
        public Groups? Groups { get; set; }

        public required ICollection<Students> Students { get; set; }

        public ICollection<Disciplines>? Notes { get; set; }
    }
}
