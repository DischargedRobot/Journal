using Swashbuckle.AspNetCore.Filters;

using MainService.Errors;

namespace MainService.Errors
{
    public class ApiError400BadRequestExample : IExamplesProvider<ApiError>
    {
        public ApiError GetExamples()
        {
            return new ApiError
            {
                StatusCode = "0.1.1",
                Title = "Неверный запрос",
                Message = "Неправильный или некорректный запрос",
                Field = "BODY"
            };
        }
    }
}
