using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class RoleRights : BaseEntity
    {
        [Key]
        public int RoleRightId { get; set; }

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }

        public int? RoleId { get; set; }
        public Roles? Role { get; set; }
    }
}
