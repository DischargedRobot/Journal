using System.ComponentModel.DataAnnotations;

namespace AuthService.Model
{
    public class Roles : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;

        public bool IsBase { get; set; }

        public ICollection<Users>? Users { get; set; }
        public ICollection<RoleRights>? RoleRights { get; set; } = [];
        [Required, MinLength(1)]
        public ICollection<RolesTypes> RoleType { get; set; } = [];
    }
}
