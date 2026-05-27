using System.ComponentModel.DataAnnotations;

namespace AuthService.Model
{

    public class RolesTypes : BaseEntity
    {
        [Key]
        public int RoleTypeId { get; set; }
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public required string Name { get; set; }
        public ICollection<Roles>? Roles { get; set; }
    }
}