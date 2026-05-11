namespace MainService
{
    public class TrainingDirectionsDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public Guid[]? GroupsUuids { get; set; } = [];

        public TrainingDirectionsDto() { }

        public TrainingDirectionsDto(TrainingDirections trainingDirection)
        {
            Uuid = trainingDirection.Uuid;
            Name = trainingDirection.Name;
            Code = trainingDirection.Code;
            GroupsUuids = trainingDirection.Groups?.Select(g => g.Uuid).ToArray() ?? [];
        }
    }
}
