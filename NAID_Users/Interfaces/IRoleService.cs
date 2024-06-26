using NAID_Users.Dtos.Role;

namespace NAID_Users.Interfaces
{
    public interface IRoleService
    {
        Task AddRoleAsync(string roleName);
        Task<List<RoleDto>> GetRolesAsync();
        Task<bool> DeleteRoleAsync(string roleName);
        Task<string?> EditRoleName(EditRoleDto roleName);
    }
}
