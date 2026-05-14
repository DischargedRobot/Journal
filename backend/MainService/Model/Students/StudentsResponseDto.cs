using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class StudentsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required int StudentCode { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Patronymic { get; set; }
        public required Guid GroupUuid { get; set; }
        public Guid[]? BrigadesUuids { get; set; } = [];

        public int Version { get; set; }
        public StudentsResponseDto() { }

        [SetsRequiredMembers] // в этом конструкторе все обязательные свойства инициализированы
        public StudentsResponseDto(Students student)
        {
            Uuid = student.Uuid;
            StudentCode = student.StudentCode;
            FirstName = student.StudentPerson!.User!.FirstName;
            LastName = student.StudentPerson.User.LastName;
            Patronymic = student.StudentPerson.User.Patronymic ?? string.Empty;
            GroupUuid = student.Group!.Uuid;
            BrigadesUuids = student.Brigades?.Select(b => b.Uuid).ToArray() ?? [];
            Version = student.Version;
        }
    }
}
