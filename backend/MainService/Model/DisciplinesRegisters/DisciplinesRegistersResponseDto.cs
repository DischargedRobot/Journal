using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class DisciplinesRegistersResponseDto
    {
        public required Guid Uuid { get; set; }

        public required string Name { get; set; }

        public required string ShortName { get; set; }

        public int Version { get; set; }
        public DisciplinesRegistersResponseDto() { }

        [SetsRequiredMembers]
        public DisciplinesRegistersResponseDto(DisciplinesRegisters disciplineRegister)
        {
            Uuid = disciplineRegister.Uuid;
            Name = disciplineRegister.Name;
            ShortName = disciplineRegister.ShortName;
            Version = disciplineRegister.Version;
        }
    }
}
