namespace AuthService.Model
{
    public class RoleRightsResponseDto
    {
        public required Guid Uuid { get; set; }
        public required string Name { get; set; }

        public static RoleRightsResponseDto Example => new()
        {
            Uuid = Guid.NewGuid(),
            Name = "student_read"
        };
    }
}
