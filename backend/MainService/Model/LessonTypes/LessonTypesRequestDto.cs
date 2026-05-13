namespace MainService
{
    public class LessonTypesRequestDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }

        public string? ShortName { get; set; }

        public Guid[]? LessonsUuids { get; set; } = [];

        public int Version { get; set; }
        public LessonTypesRequestDto() { }

        public LessonTypesRequestDto(LessonTypes lessonType)
        {
            Uuid = lessonType.Uuid;
            Name = lessonType.Name;
            ShortName = lessonType.ShortName;
            LessonsUuids = lessonType.Lessons?.Select(l => l.Uuid).ToArray() ?? [];
            Version = lessonType.Version;
        }
    }
}
