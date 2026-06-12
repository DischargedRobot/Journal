using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Model
{
    public class RoleRightsUpdateDto
    {
        [SwaggerSchema("Название права роли")]
        public string? Name { get; set; }
    }
}
