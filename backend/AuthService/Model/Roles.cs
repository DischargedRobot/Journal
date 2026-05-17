using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class Roles : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;

        public required string RoleName { get; set; }
        public ICollection<Users>? Users { get; set; }
        public ICollection<RoleRights>? RoleRights { get; set; }
    }
}
