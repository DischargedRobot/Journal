namespace MainService
{
    public class LessonMarksDto
    {
        public required Guid Uuid { get; set; }
        public required Guid LessonUuid { get; set; }
        public required Guid MarkUuid { get; set; }
        public required Guid StudentUuid { get; set; }

        public LessonMarksDto() { }

        public LessonMarksDto(LessonMarks lessonMark)
        {
            Uuid = lessonMark.Uuid;
            LessonUuid = lessonMark.Lesson!.Uuid;
            MarkUuid = lessonMark.Mark!.Uuid;
            StudentUuid = lessonMark.Student!.Uuid;
        }
    }
}
