namespace AuthService.Errors
{
    public class ApiError
    {
        /// <summary>
        /// Код ошибки в формате "X.Y.Z", где 
        /// X - клиентская ошибка (0), ошибка сервера (1) или ошибка интеграции с внешними сервисами (2),
        /// Y - из-за какого объекта ошибка (поле, тела запроса
        /// Z - конкретная ошибка
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