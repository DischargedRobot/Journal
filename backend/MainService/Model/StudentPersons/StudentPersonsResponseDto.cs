using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class StudentPersonsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string Patronymic { get; set; } = string.Empty;
        public required int Version { get; set; }

        public StudentPersonsResponseDto() { }

        [SetsRequiredMembers]
        public StudentPersonsResponseDto(StudentPersons studentPerson)
        {
            Uuid = studentPerson.Uuid;
            FirstName = studentPerson.User?.FirstName ?? string.Empty;
            LastName = studentPerson.User?.LastName ?? string.Empty;
            Patronymic = studentPerson.User?.Patronymic ?? string.Empty;
            Version = studentPerson.Version;
        }
    }
}
