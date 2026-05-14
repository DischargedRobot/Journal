namespace MainService
{
    public class StudentsCreateDto
    {
        public int? StudentCode { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Patronymic { get; set; }
        public required Guid GroupUuid { get; set; }
    }
}
