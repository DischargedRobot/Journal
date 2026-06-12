using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Model
{
    public class RolesTypesCreateDto
    {
        [SwaggerSchema("Название типа роли"), Required, ValidateNever]
        public string? Name { get; set; }
    }
}
