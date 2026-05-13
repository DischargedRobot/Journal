namespace MainService
{
    public class LessonsRequestDto
    {
        public required Guid Uuid { get; set; }
        public required int Code { get; set; }
        public required DateTime StartDate { get; set; }
        public required string Name { get; set; } = string.Empty;
        public required string ShortName { get; set; } = string.Empty;
        public required Guid LessonTypeUuid { get; set; }
        public required Guid DisciplineUuid { get; set; }
        public Guid[]? LessonPresencesUuids { get; set; } = [];
        public Guid[]? LessonMarksUuids { get; set; } = [];

        public int Version { get; set; }
        public LessonsRequestDto() { }

        public LessonsRequestDto(Lessons lesson)
        {
            Uuid = lesson.Uuid;
            Code = lesson.Code;
            StartDate = lesson.StartDate;
            Name = lesson.Name;
            ShortName = lesson.ShortName;
            LessonTypeUuid = lesson.LessonType!.Uuid;
            DisciplineUuid = lesson.Discipline!.Uuid;
            LessonPresencesUuids = lesson.LessonPresences?.Select(lp => lp.Uuid).ToArray() ?? [];
            LessonMarksUuids = lesson.LessonMarks?.Select(lm => lm.Uuid).ToArray() ?? [];
            Version = lesson.Version;
        }
    }
}
