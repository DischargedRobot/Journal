using System.ComponentModel.DataAnnotations;

namespace AuthService.Model
{
    public class RoleRights : BaseEntity
    {
        [Key]
        public int RoleRightId { get; set; }

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public required string Name { get; set; }
        public ICollection<Roles>? Roles { get; set; }

    }
}
