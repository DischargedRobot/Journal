using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class PresenceStatusesResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public PresenceStatusesResponseDto() { }
        [SetsRequiredMembers]
        public PresenceStatusesResponseDto(PresenceStatuses presenceStatus)
        {
            Uuid = presenceStatus.Uuid;
            Name = presenceStatus.Name;
            Version = presenceStatus.Version;
        }
    }
}
