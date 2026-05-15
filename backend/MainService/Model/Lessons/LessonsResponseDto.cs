using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class LessonsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required int Code { get; set; }
        public required DateTime StartDate { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public required Guid LessonTypeUuid { get; set; }
        public required Guid DisciplineUuid { get; set; }

        public int Version { get; set; }
        public LessonsResponseDto() { }

        [SetsRequiredMembers]
        public LessonsResponseDto(Lessons lesson)
        {
            Uuid = lesson.Uuid;
            Code = lesson.Code;
            StartDate = lesson.StartDate;
            Name = lesson.Name;
            ShortName = lesson.ShortName;
            LessonTypeUuid = lesson.LessonType!.Uuid;
            DisciplineUuid = lesson.Discipline!.Uuid;
            Version = lesson.Version;
        }
    }
}
