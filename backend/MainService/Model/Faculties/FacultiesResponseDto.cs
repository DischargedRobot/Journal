using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class FacultiesResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public required string ShortName { get; set; }
        public Guid[]? DepartmentsUuids { get; set; } = [];
        public Guid[]? GroupsUuids { get; set; } = [];

        public int Version { get; set; }
        public FacultiesResponseDto() { }

        [SetsRequiredMembers]
        public FacultiesResponseDto(Faculties faculty)
        {
            Uuid = faculty.Uuid;
            Name = faculty.Name;
            ShortName = faculty.ShortName;
            DepartmentsUuids = faculty.Departments?.Select(d => d.Uuid).ToArray() ?? [];
            GroupsUuids = faculty.Groups?.Select(g => g.Uuid).ToArray() ?? [];
            Version = faculty.Version;
        }
    }
}
