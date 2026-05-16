using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class LessonMarksRequestDto
    {
        public required Guid Uuid { get; set; }
        public required Guid LessonUuid { get; set; }
        public required Guid MarkUuid { get; set; }
        public required Guid StudentUuid { get; set; }

        public int Version { get; set; }
        public LessonMarksRequestDto() { }

        [SetsRequiredMembers]
        public LessonMarksRequestDto(LessonMarks lessonMark)
        {
            Uuid = lessonMark.Uuid;
            LessonUuid = lessonMark.Lesson!.Uuid;
            MarkUuid = lessonMark.Mark!.Uuid;
            StudentUuid = lessonMark.Student!.Uuid;
            Version = lessonMark.Version;
        }
    }
}
