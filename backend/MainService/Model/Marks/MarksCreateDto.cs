namespace MainService
{
    public class MarksCreateDto
    {
        public required Guid MarkTypeUuid { get; set; }

        public required string Value { get; set; }
    }
}
