using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Model
{
    public class RoleRightsCreateDto
    {
        [SwaggerSchema("Название права роли"), Required, ValidateNever]
        public string? Name { get; set; }
    }
}
