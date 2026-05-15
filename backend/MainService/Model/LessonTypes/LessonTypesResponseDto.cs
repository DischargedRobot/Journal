using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class LessonTypesResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }

        public string? ShortName { get; set; }

        public int Version { get; set; }
        public LessonTypesResponseDto() { }

        [SetsRequiredMembers]
        public LessonTypesResponseDto(LessonTypes lessonType)
        {
            Uuid = lessonType.Uuid;
            Name = lessonType.Name;
            ShortName = lessonType.ShortName;
            Version = lessonType.Version;
        }
    }
}
