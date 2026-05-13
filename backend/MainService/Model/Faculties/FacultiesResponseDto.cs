using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class FacultiesResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public required string ShortName { get; set; }

        public int Version { get; set; }
        public FacultiesResponseDto() { }

        [SetsRequiredMembers]
        public FacultiesResponseDto(Faculties faculty)
        {
            Uuid = faculty.Uuid;
            Name = faculty.Name;
            ShortName = faculty.ShortName;
            Version = faculty.Version;
        }
    }
}
