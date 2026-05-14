using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class GroupsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required DateOnly AdmissionDate { get; set; }
        public required string Code { get; set; }
        public required Guid TrainingDirectionUuid { get; set; }
        public required Guid FacultyUuid { get; set; }
        public Guid[]? CuratorsUuids { get; set; } = [];

        public int Version { get; set; }
        public GroupsResponseDto() { }

        [SetsRequiredMembers]
        public GroupsResponseDto(Groups group)
        {
            Uuid = group.Uuid;
            AdmissionDate = group.AdmissionDate;
            Code = group.Code;
            TrainingDirectionUuid = group.TrainingDirection!.Uuid;
            FacultyUuid = group.Faculty!.Uuid;
            CuratorsUuids = group.Curators?.Select(p => p.Uuid).ToArray() ?? [];
            Version = group.Version;
        }
    }
}
