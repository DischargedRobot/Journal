using Swashbuckle.AspNetCore.Filters;

namespace MainService.Errors
{
    public class ApiError
    {
        /// <summary>
        /// Код ошибки в формате "X.Y.Z", где X - категория ошибки, Y - подкатегория, Z - конкретная ошибка
        /// </summary>
        public string StatusCode { get; set; } = null!;

        /// <summary>
        /// Краткое описание ошибки
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Подробное описание ошибки и рекомендации по её исправлению
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Поле запроса, которое вызвало ошибку (например, имя параметра или части тела запроса)
        /// </summary>
        public string? Field { get; set; }

        /// <summary>
        /// Дополнительные сведения об ошибке
        /// </summary>
        public string? Details { get; set; }
        public ApiError() { }

        public ApiError(string statusCode, string title, string message, string? field = null)
        {
            StatusCode = statusCode;
            Title = title;
            Message = message;
            Field = field;
        }
    }

}