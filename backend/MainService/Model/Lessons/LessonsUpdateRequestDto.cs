namespace MainService
{
    public class LessonsUpdateDto
    {
        public int? Code { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public Guid? LessonTypeUuid { get; set; }
        public Guid? DisciplineUuid { get; set; }
        public Guid[]? LessonPresencesUuids { get; set; }
        public Guid[]? LessonMarksUuids { get; set; }
        public int? Version { get; set; }
    }
}
