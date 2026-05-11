namespace MainService
{
    public class MarksDto
    {
        public required Guid Uuid { get; set; }
        public required string Value { get; set; }
        public required Guid MarkTypeUuid { get; set; }
        public Guid[]? LessonMarksUuids { get; set; } = [];

        public MarksDto() { }

        public MarksDto(Marks mark)
        {
            Uuid = mark.Uuid;
            Value = mark.Value;
            MarkTypeUuid = mark.MarkType!.Uuid;
            LessonMarksUuids = mark.LessonMarks?.Select(lm => lm.Uuid).ToArray() ?? [];
        }
    }
}
