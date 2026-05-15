using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class UniversityEmployersResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? Patronymic { get; set; }
        public required Guid UserUuid { get; set; }
        public Guid[]? ProfessorsUuids { get; set; } = [];
        public Guid[]? MarkTypesUuids { get; set; } = [];

        public int Version { get; set; }
        public UniversityEmployersResponseDto() { }

        [SetsRequiredMembers]
        public UniversityEmployersResponseDto(UniversityEmployers employer)
        {
            Uuid = employer.Uuid;
            FirstName = employer.FirstName;
            LastName = employer.LastName;
            Patronymic = employer.Patronymic;
            UserUuid = employer.User!.Uuid;
            ProfessorsUuids = employer.Professors?.Select(p => p.Uuid).ToArray() ?? [];
            MarkTypesUuids = employer.MarkTypes?.Select(mt => mt.Uuid).ToArray() ?? [];
            Version = employer.Version;
        }
    }
}
