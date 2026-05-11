using Swashbuckle.AspNetCore.Filters;

using MainService;

namespace MainService.EntityDtoExamples
{
    public class FacultiesDtoExample : IExamplesProvider<FacultiesDto>
    {
        public FacultiesDto GetExamples()
        {
            return new FacultiesDto
            {
                Uuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Name = "Факультет математики",
                ShortName = "МФ",
                DepartmentsUuids = [Guid.Parse("3fa37864-5717-4562-3333-2c963f66afa6")],
                GroupsUuids = [Guid.Parse("3fa37864-5717-4562-b3aa-2c963f66afa6")]
            };
        }
    }
}
