using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Model
{
    public class RolesTypesResponseDto
    {
        [SwaggerSchema("UUID типа роли")]
        public required Guid Uuid { get; set; }
        [SwaggerSchema("Название типа роли")]
        public required string Name { get; set; }

        public RolesTypesResponseDto() { }

        [SetsRequiredMembers]
        public RolesTypesResponseDto(RolesTypes roleType)
        {
            Uuid = roleType.Uuid;
            Name = roleType.Name;
        }

        public static RolesTypesResponseDto Example => new()
        {
            Uuid = Guid.NewGuid(),
            Name = "DefaultType"
        };
    }
}
