using Swashbuckle.AspNetCore.Filters;

namespace MainService.Errors
{
    public class ApiError
    {
        public required string StatusCode { get; set; }
        public required string Title { get; set; } = string.Empty;
        public required string Message { get; set; } = string.Empty;
        public required string Field { get; set; } = string.Empty;
        public string? Details { get; set; }
        public ApiError() { }
    }

}