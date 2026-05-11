namespace MainService
{
    public class LessonTypesDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }

        public string? ShortName { get; set; }

        public Guid[]? LessonsUuids { get; set; } = [];

        public LessonTypesDto() { }

        public LessonTypesDto(LessonTypes lessonType)
        {
            Uuid = lessonType.Uuid;
            Name = lessonType.Name;
            ShortName = lessonType.ShortName;
            LessonsUuids = lessonType.Lessons?.Select(l => l.Uuid).ToArray() ?? [];
        }
    }
}
