using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class DepartmentsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public required string ShortName { get; set; }
        public required string Code { get; set; }
        public required Guid FacultyUuid { get; set; }

        public int Version { get; set; }
        public DepartmentsResponseDto() { }

        [SetsRequiredMembers]
        public DepartmentsResponseDto(Departments department)
        {
            Uuid = department.Uuid;
            Name = department.Name;
            ShortName = department.ShortName;
            Code = department.Code;
            FacultyUuid = department.Faculty!.Uuid;
            Version = department.Version;
        }
    }
}
