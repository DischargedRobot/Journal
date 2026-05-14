using Swashbuckle.AspNetCore.Filters;

using MainService.Errors;

namespace MainService.Errors
{
    public class ApiError404NotFoundExample : IExamplesProvider<ApiError>
    {
        public ApiError GetExamples()
        {
            return new ApiError
            {
                StatusCode = "0.0.3",
                Title = "Не найдено",
                Message = "Ресурс не найден",
                Field = string.Empty
            };
        }
    }
}
