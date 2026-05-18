namespace AuthService.Model
{
    public class RolesCreateDto
    {
        public string Name { get; set; } = null!;
        public string RoleName { get; set; } = null!;
        public IEnumerable<Guid>? RightsUuids { get; set; }
    }
}
