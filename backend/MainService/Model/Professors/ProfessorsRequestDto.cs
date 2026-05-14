using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MainService
{
    public class ProfessorsRequestDto
    {
        public required Guid Uuid { get; set; }
        public required Guid DepartmentUuid { get; set; }
        public required Guid PostUuid { get; set; }
        public required string PostName { get; set; }
        public required Guid AcademicYearUuid { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Patronymic { get; set; }
        public Guid[]? GroupCuratorUuids { get; set; } = [];
        public Guid[]? DisciplinesUuids { get; set; } = [];

        public int Version { get; set; }
        public ProfessorsRequestDto() { }

        [SetsRequiredMembers]
        public ProfessorsRequestDto(Professors professor)
        {
            Uuid = professor.Uuid;
            DepartmentUuid = professor.Department!.Uuid;
            PostUuid = professor.Post!.Uuid;
            PostName = professor.Post.Name;
            AcademicYearUuid = professor.AcademicYear!.Uuid;

            GroupCuratorUuids = professor.GroupCurator?.Select(g => g.Uuid).ToArray() ?? [];
            DisciplinesUuids = professor.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
            Version = professor.Version;
        }
    }
}
