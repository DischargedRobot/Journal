using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Model
{
    public class RolesCreateDto
    {
        [SwaggerSchema("Название роли"), Required, ValidateNever]
        public string? Name { get; set; }
        [SwaggerSchema("Базовая роль (в системе может быть только одна)")]
        public bool IsBase { get; set; } = false;
        [SwaggerSchema("UUID прав роли"), Required, ValidateNever]
        public IEnumerable<Guid>? RightsUuids { get; set; }
        [SwaggerSchema("UUID типов роли"), Required, ValidateNever]
        public required IEnumerable<Guid> RoleTypesUuids { get; set; }
    }
}
