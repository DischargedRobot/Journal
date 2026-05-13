namespace MainService
{
    public class MarksResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Value { get; set; }
        public required Guid MarkTypeUuid { get; set; }
        public Guid[]? LessonMarksUuids { get; set; } = [];

        public int Version { get; set; }
        public MarksResponseDto() { }

        public MarksResponseDto(Marks mark)
        {
            Uuid = mark.Uuid;
            Value = mark.Value;
            MarkTypeUuid = mark.MarkType!.Uuid;
            LessonMarksUuids = mark.LessonMarks?.Select(lm => lm.Uuid).ToArray() ?? [];
            Version = mark.Version;
        }
    }
}
