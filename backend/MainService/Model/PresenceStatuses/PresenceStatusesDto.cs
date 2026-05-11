namespace MainService
{
    public class PresenceStatusesDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; } = string.Empty;
        public Guid[]? LessonPresencesUuids { get; set; } = [];

        public PresenceStatusesDto() { }

        public PresenceStatusesDto(PresenceStatuses presenceStatus)
        {
            Uuid = presenceStatus.Uuid;
            Name = presenceStatus.Name;
            LessonPresencesUuids = presenceStatus.LessonPresences?.Select(lp => lp.Uuid).ToArray() ?? [];
        }
    }
}
