using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class DisciplinesRegistersRequestDto
    {
        public required Guid Uuid { get; set; }

        public required string Name { get; set; }

        public required string ShortName { get; set; }

        public int Version { get; set; }
        public DisciplinesRegistersRequestDto() { }

        [SetsRequiredMembers]
        public DisciplinesRegistersRequestDto(DisciplinesRegisters disciplineRegister)
        {
            Uuid = disciplineRegister.Uuid;
            Name = disciplineRegister.Name;
            ShortName = disciplineRegister.ShortName;
            Version = disciplineRegister.Version;
        }
    }
}
