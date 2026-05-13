using System.Diagnostics.CodeAnalysis;

namespace MainService
{
    public class DisciplinesResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public required string ShortName { get; set; }
        public required bool IsArchived { get; set; }
        public Guid? DisciplineRegisterUuid { get; set; }
        public required Guid SemesterUuid { get; set; }
        public required Guid AcademicYearUuid { get; set; }
        public Guid[]? BrigadesUuids { get; set; } = [];
        public required Guid[] GroupsUuids { get; set; } = [];
        public Guid[]? ProfessorsUuids { get; set; } = [];
        public Guid[]? LessonsUuids { get; set; } = [];
        public Guid[]? AttestationsUuids { get; set; } = [];

        public int Version { get; set; }
        public DisciplinesResponseDto() { }

        [SetsRequiredMembers]
        public DisciplinesResponseDto(Disciplines discipline)
        {
            Uuid = discipline.Uuid;
            Name = discipline.Name;
            ShortName = discipline.ShortName;
            IsArchived = discipline.IsArchived;
            DisciplineRegisterUuid = discipline.DisciplineRegister!.Uuid;
            SemesterUuid = discipline.Semester!.Uuid;
            AcademicYearUuid = discipline.AcademicYear!.Uuid;
            BrigadesUuids = discipline.Brigades?.Select(b => b.Uuid).ToArray() ?? [];
            GroupsUuids = discipline.Groups?.Select(g => g.Uuid).ToArray() ?? [];
            ProfessorsUuids = discipline.Professors?.Select(p => p.Uuid).ToArray() ?? [];
            LessonsUuids = discipline.Lessons?.Select(l => l.Uuid).ToArray() ?? [];
            AttestationsUuids = discipline.Attestations?.Select(a => a.Uuid).ToArray() ?? [];
            Version = discipline.Version;
        }
    }
}
