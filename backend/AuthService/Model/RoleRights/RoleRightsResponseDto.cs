using System.Diagnostics.CodeAnalysis;

namespace AuthService.Model
{
    public class RoleRightsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }

        public RoleRightsResponseDto() { }
        [SetsRequiredMembers]
        public RoleRightsResponseDto(RoleRights roleRight)
        {
            Uuid = roleRight.Uuid;
            Name = roleRight.Name;
        }

        public static RoleRightsResponseDto Example => new()
        {
            Uuid = Guid.NewGuid(),
            Name = "student_read"
        };
    }
}
