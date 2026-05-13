using Swashbuckle.AspNetCore.Filters;

using MainService;

namespace MainService.EntityDtoExamples
{
    public class FacultiesDtoExample : IExamplesProvider<FacultiesResponseDto>
    {
        public FacultiesResponseDto GetExamples()
        {
            return new FacultiesResponseDto
            {
                Uuid = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Name = "Факультет математики",
                ShortName = "МФ",
            };
        }
    }
}
