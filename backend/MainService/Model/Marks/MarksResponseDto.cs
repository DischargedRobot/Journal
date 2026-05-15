using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class MarksResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Value { get; set; }
        public required Guid MarkTypeUuid { get; set; }
        public int Version { get; set; }
        public MarksResponseDto() { }

        [SetsRequiredMembers]
        public MarksResponseDto(Marks mark)
        {
            Uuid = mark.Uuid;
            Value = mark.Value;
            MarkTypeUuid = mark.MarkType!.Uuid;
            Version = mark.Version;
        }
    }
}
