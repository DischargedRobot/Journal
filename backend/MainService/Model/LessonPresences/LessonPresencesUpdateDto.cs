namespace MainService
{
    public class LessonPresencesUpdateDto
    {
        public bool? IsPresent { get; set; }
        public Guid? LessonUuid { get; set; }
        public Guid? StudentUuid { get; set; }
        public Guid? PresenceStatusUuid { get; set; }

        public int? Version { get; set; }
    }
}
