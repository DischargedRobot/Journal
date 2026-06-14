
using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Model
{
    public class RolesUpdateDto
    {
        [SwaggerSchema("Название роли")]
        public string? Name { get; set; }
        [SwaggerSchema("Базовая роль (в системе может быть только одна)")]
        public bool? IsBase { get; set; }
        public IEnumerable<Guid>? RightsUuids { get; set; }
        public IEnumerable<Guid>? RoleTypesUuids { get; set; }
    }
}
