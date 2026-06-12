namespace AuthService.Model.Auth.Dto
{
    public class GenerateRegistrationCodeRequest
    {
        public Guid[]? Roles { get; set; }
        public string? GroupUuid { get; set; }
        public string? DepartmentUuid { get; set; }
    }
}