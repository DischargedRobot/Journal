namespace MainService
{
    public class TrainingDirectionsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public Guid[]? GroupsUuids { get; set; } = [];

        public int Version { get; set; }
        public TrainingDirectionsResponseDto() { }

        public TrainingDirectionsResponseDto(TrainingDirections trainingDirection)
        {
            Uuid = trainingDirection.Uuid;
            Name = trainingDirection.Name;
            Code = trainingDirection.Code;
            GroupsUuids = trainingDirection.Groups?.Select(g => g.Uuid).ToArray() ?? [];
            Version = trainingDirection.Version;
        }
    }
}
