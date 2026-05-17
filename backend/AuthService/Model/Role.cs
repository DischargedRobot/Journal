using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class Roles : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required string RoleName { get; set; }

        public ICollection<UserRoles>? UserRoles { get; set; }
        public ICollection<RoleRights>? RoleRights { get; set; }
    }
}
