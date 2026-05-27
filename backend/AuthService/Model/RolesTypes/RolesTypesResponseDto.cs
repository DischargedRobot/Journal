namespace AuthService.Model
{
    public class RolesTypesResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }

        public static RolesTypesResponseDto Example => new()
        {
            Uuid = Guid.NewGuid(),
            Name = "DefaultType"
        };
    }
}
