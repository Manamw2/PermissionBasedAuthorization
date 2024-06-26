namespace NAID_Users.Models
{
    public class RolePermission
    {
        public string? RoleId { get; set; }
        public Role? Role { get; set; }
        public int PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }
}
