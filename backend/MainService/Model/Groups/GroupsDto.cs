namespace MainService
{
    public class GroupsDto
    {
        public required Guid Uuid { get; set; }
        public required DateOnly AdmissionDate { get; set; }
        public required string Code { get; set; }
        public required Guid TrainingDirectionUuid { get; set; }
        public required Guid FacultyUuid { get; set; }
        public Guid[]? BrigadesUuids { get; set; } = [];
        public Guid[]? StudentsUuids { get; set; } = [];
        public Guid[]? DisciplinesUuids { get; set; } = [];
        public Guid[]? CuratorsUuids { get; set; } = [];

        public GroupsDto() { }

        public GroupsDto(Groups group)
        {
            Uuid = group.Uuid;
            AdmissionDate = group.AdmissionDate;
            Code = group.Code;
            TrainingDirectionUuid = group.TrainingDirection!.Uuid;
            FacultyUuid = group.Faculty!.Uuid;
            BrigadesUuids = group.Brigades?.Select(b => b.Uuid).ToArray() ?? [];
            StudentsUuids = group.Students?.Select(s => s.Uuid).ToArray() ?? [];
            DisciplinesUuids = group.Disciplines?.Select(d => d.Uuid).ToArray() ?? [];
            CuratorsUuids = group.Curators?.Select(p => p.Uuid).ToArray() ?? [];
        }
    }
}
