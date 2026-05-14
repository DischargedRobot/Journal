namespace MainService
{
    public class StudentPersonsCreateDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Patronymic { get; set; }
    }
}
