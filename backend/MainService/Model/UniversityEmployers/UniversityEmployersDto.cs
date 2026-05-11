namespace MainService
{
    public class UniversityEmployersDto
    {
        public required Guid Uuid { get; set; }
        public required Guid UserUuid { get; set; }
        public Guid[]? ProfessorsUuids { get; set; } = [];
        public Guid[]? MarkTypesUuids { get; set; } = [];

        public UniversityEmployersDto() { }

        public UniversityEmployersDto(UniversityEmployers employer)
        {
            Uuid = employer.Uuid;
            UserUuid = employer.User!.Uuid;
            ProfessorsUuids = employer.Professors?.Select(p => p.Uuid).ToArray() ?? [];
            MarkTypesUuids = employer.MarkTypes?.Select(mt => mt.Uuid).ToArray() ?? [];
        }
    }
}
