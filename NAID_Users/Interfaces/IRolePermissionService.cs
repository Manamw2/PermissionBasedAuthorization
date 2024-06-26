using NAID_Users.Dtos.Permissions;
using System.Security.Claims;

namespace NAID_Users.Interfaces
{
    public interface IRolePermissionService
    {
        Task AddPermissionToRoleAsync(int permmissionId, string roleId);
        Task<bool> DeletePermissionToRoleAsync(int permmissionId, string roleId);
        Task<bool> UpdateRolePermissionsAsync(string roleName, IEnumerable<int> permissionIds);
        Task<bool> UserHasPermissionAsync(ClaimsPrincipal user, string permissionName);
        Task<List<PermissionDto>> GetRolePermissionsAsync(string roleId);
    }
}
