using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    public class UserRoles : BaseEntity
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users? User { get; set; }

        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Roles? Role { get; set; }
    }
}
