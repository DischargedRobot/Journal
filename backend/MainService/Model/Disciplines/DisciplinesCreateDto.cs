using System.ComponentModel.DataAnnotations;

namespace MainService
{
    public class DisciplinesCreateDto
    {
        public required string Name { get; set; }
        public string? ShortName { get; set; }
        public bool IsArchived { get; set; } = false;
        public Guid? DisciplineRegisterUuid { get; set; }
        public required Guid SemesterUuid { get; set; }
        public required Guid AcademicYearUuid { get; set; }
        public required Guid[] GroupsUuids { get; set; } = [];
        public Guid[]? ProfessorsUuids { get; set; } = [];
        public required Guid DisciplineType { get; set; }
    }
}
