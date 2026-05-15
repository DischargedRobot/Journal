using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class StudentPersonsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Patronymic { get; set; }
        public required int Version { get; set; }

        public StudentPersonsResponseDto() { }

        [SetsRequiredMembers]
        public StudentPersonsResponseDto(StudentPersons studentPerson)
        {
            Uuid = studentPerson.Uuid;
            FirstName = studentPerson.FirstName;
            LastName = studentPerson.LastName;
            Patronymic = studentPerson.Patronymic;
            Version = studentPerson.Version;
        }
    }
}
