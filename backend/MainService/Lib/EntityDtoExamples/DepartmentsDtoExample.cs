using Swashbuckle.AspNetCore.Filters;

using System;

namespace MainService.EntityDtoExamples
{
    public class DepartmentsDtoExample : IExamplesProvider<DepartmentsResponseDto>
    {
        public DepartmentsResponseDto GetExamples()
        {
            return new DepartmentsResponseDto
            {
                Uuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afb1"),
                Name = "Кафедра прикладной математики",
                ShortName = "ПМ",
                Code = "PM",
                FacultyUuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            };
        }
    }
}
