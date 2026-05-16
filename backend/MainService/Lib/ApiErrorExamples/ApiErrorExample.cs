namespace MainService.Errors
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ApiErrorExampleAttribute : Attribute
    {
        public string StatusCode { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Field { get; set; } = string.Empty;

        public int HttpStatus { get; }

        public ApiError Error => new(StatusCode, Title, Message, Field);

        public ApiErrorExampleAttribute(
            int httpStatus,
            string statusCode,
            string title,
            string message,
            string field = ""
        )
        {
            HttpStatus = httpStatus;
            StatusCode = statusCode;
            Title = title;
            Message = message;
            Field = field;
        }

        public ApiErrorExampleAttribute(int httpStatus)
        {
            HttpStatus = httpStatus;
        }
    }
}