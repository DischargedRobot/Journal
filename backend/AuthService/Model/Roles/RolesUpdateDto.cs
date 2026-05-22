namespace AuthService.Model
{
    public class RolesUpdateDto
    {
        public string? Name { get; set; }
        public IEnumerable<Guid>? RightsUuids { get; set; }
    }
}
