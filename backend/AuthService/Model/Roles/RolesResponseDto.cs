using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AuthService.Model
{
	public class RolesResponseDto
	{
		public required Guid Uuid { get; set; }
		public required string Name { get; set; }
		public bool IsBase { get; set; }
		public IEnumerable<RoleRightsResponseDto> Rights { get; set; } = [];
		public IEnumerable<RolesTypesResponseDto> RoleTypes { get; set; } = [];
		public RolesResponseDto() { }
		[SetsRequiredMembers]
		public RolesResponseDto(Roles role)
		{
			Uuid = role.Uuid;
			Name = role.Name;
			IsBase = role.IsBase;
			Rights = role.RoleRights?.Select(rr => new RoleRightsResponseDto
			{
				Uuid = rr.Uuid,
				Name = rr.Name,
			}) ?? [];
			RoleTypes = role.RoleType.Select(rt => new RolesTypesResponseDto
			{
				Uuid = rt.Uuid,
				Name = rt.Name,
			});
		}

		public static RolesResponseDto Example => new()
		{
			Uuid = Guid.NewGuid(),
			Name = "Admin",
			IsBase = false,
			Rights = [RoleRightsResponseDto.Example],
			RoleTypes = [RolesTypesResponseDto.Example]
		};
	}
}
