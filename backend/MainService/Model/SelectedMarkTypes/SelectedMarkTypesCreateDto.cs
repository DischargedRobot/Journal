namespace MainService
{
    public class SelectedMarkTypesCreateDto
    {
        public required Guid LessonTypeUuid { get; set; }
        public required Guid MarkTypeUuid { get; set; }
        public required Guid DisciplineUuid { get; set; }
    }
}
