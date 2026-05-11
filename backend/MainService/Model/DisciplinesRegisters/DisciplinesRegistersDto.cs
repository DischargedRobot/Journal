namespace MainService
{
    public class DisciplinesRegistersDto
    {
        public required Guid Uuid { get; set; }

        public required string Name { get; set; }

        public required string ShortName { get; set; }

        public DisciplinesRegistersDto() { }

        public DisciplinesRegistersDto(DisciplinesRegisters disciplineRegister)
        {
            Uuid = disciplineRegister.Uuid;
            Name = disciplineRegister.Name;
            ShortName = disciplineRegister.ShortName;
        }
    }
}
