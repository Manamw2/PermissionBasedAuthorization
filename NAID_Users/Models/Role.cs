using Microsoft.AspNetCore.Identity;

namespace NAID_Users.Models
{
    public class Role : IdentityRole
    {
        public List<RolePermission>? RolePermissions { get; set; }
    }
}
