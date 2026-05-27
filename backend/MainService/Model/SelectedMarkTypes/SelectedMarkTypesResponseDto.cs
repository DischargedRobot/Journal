using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class SelectedMarkTypesResponseDto
    {
        public required Guid LessonTypeUuid { get; set; }
        public required Guid MarkTypeUuid { get; set; }
        public required Guid DisciplineUuid { get; set; }

        public int Version { get; set; }

        public SelectedMarkTypesResponseDto() { }

        [SetsRequiredMembers]
        public SelectedMarkTypesResponseDto(SelectedMarkTypes item)
        {
            LessonTypeUuid = item.LessonType!.Uuid;
            MarkTypeUuid = item.MarkType!.Uuid;
            DisciplineUuid = item.Disciplines!.Uuid;
            Version = item.Version;
        }
    }
}
