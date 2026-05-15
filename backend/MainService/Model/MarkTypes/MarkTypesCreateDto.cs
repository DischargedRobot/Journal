namespace MainService
{
    public class MarkTypesCreateDto
    {
        public required string Name { get; set; }
        public Guid? UniversityEmployerUuid { get; set; }
    }
}
