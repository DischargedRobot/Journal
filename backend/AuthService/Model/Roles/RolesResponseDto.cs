using System.Diagnostics.CodeAnalysis;

namespace AuthService.Model
{
	public class RolesResponseDto
	{
		public required Guid Uuid { get; set; }
		public required string Name { get; set; }
		public required string RoleName { get; set; }
		public IEnumerable<RoleRightsResponseDto>? Rights { get; set; }
		public RolesResponseDto() { }
		[SetsRequiredMembers]
		public RolesResponseDto(Roles role)
		{
			Uuid = role.Uuid;
			Name = role.Name;
			RoleName = role.RoleName;
			Rights = role.RoleRights?.Select(rr => new RoleRightsResponseDto
			{
				Uuid = rr.Uuid,
				Name = rr.Name,
			});
		}
	}
}
