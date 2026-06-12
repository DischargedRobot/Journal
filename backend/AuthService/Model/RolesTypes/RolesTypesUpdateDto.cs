using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using Swashbuckle.AspNetCore.Annotations;

namespace AuthService.Model
{
    public class RolesTypesUpdateDto
    {
        [SwaggerSchema("Название типа роли") ]
        public string? Name { get; set; }
    }
}
