namespace MainService
{
    public class LessonPresencesDto
    {
        public required Guid Uuid { get; set; }
        public required bool IsPresent { get; set; }
        public required Guid LessonUuid { get; set; }
        public required Guid StudentUuid { get; set; }
        public required Guid PresenceStatusUuid { get; set; }

        public LessonPresencesDto() { }

        public LessonPresencesDto(LessonPresences lessonPresence)
        {
            Uuid = lessonPresence.Uuid;
            IsPresent = lessonPresence.IsPresent;
            LessonUuid = lessonPresence.Lesson!.Uuid;
            StudentUuid = lessonPresence.Student!.Uuid;
            PresenceStatusUuid = lessonPresence.PresenceStatus!.Uuid;
        }
    }
}
