namespace MainService
{
    public class PresenceStatusesRequestDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; } = string.Empty;
        public Guid[]? LessonPresencesUuids { get; set; } = [];

        public int Version { get; set; }
        public PresenceStatusesRequestDto() { }

        public PresenceStatusesRequestDto(PresenceStatuses presenceStatus)
        {
            Uuid = presenceStatus.Uuid;
            Name = presenceStatus.Name;
            LessonPresencesUuids = presenceStatus.LessonPresences?.Select(lp => lp.Uuid).ToArray() ?? [];
            Version = presenceStatus.Version;
        }
    }
}
