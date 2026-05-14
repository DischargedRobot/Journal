namespace MainService
{
    public class StudentsUpdateDto
    {
        public int? StudentCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Patronymic { get; set; }
        public Guid? GroupUuid { get; set; }
    }
}
