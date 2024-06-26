using NAID_Users.Dtos.Permissions;

namespace NAID_Users.Dtos.User
{
    public class UserMeInfo
    {
        public string? Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
    }
}
