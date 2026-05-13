using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class EmployeePosts : BaseEntity
    {
        [Key]
        public int PostId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();

        [Required]
        public required string Name { get; set; }

        public ICollection<Professors>? Professors { get; set; }
    }
}