using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class EmployeePosts
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<Professors>? Professors { get; set; }
    }
}