namespace MainService
{
    public class UniversityEmployersResponseDto
    {
        public required Guid Uuid { get; set; }
        public required Guid UserUuid { get; set; }
        public Guid[]? ProfessorsUuids { get; set; } = [];
        public Guid[]? MarkTypesUuids { get; set; } = [];

        public int Version { get; set; }
        public UniversityEmployersResponseDto() { }

        public UniversityEmployersResponseDto(UniversityEmployers employer)
        {
            Uuid = employer.Uuid;
            UserUuid = employer.User!.Uuid;
            ProfessorsUuids = employer.Professors?.Select(p => p.Uuid).ToArray() ?? [];
            MarkTypesUuids = employer.MarkTypes?.Select(mt => mt.Uuid).ToArray() ?? [];
            Version = employer.Version;
        }
    }
}
