using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NAID_Users.Data;
using NAID_Users.Dtos.Permissions;
using NAID_Users.Interfaces;
using NAID_Users.Models;
using System.Security.Claims;

namespace NAID_Users.Services
{
    public class RolePermssionService : IRolePermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public RolePermssionService(ApplicationDbContext context, RoleManager<Role> roleManager, UserManager<AppUser> userManager, IMapper mapper1)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper1;
        }
        public async Task AddPermissionToRoleAsync(int permmissionId, string roleId)
        {
            await _context.RolePermissions.AddAsync(new RolePermission
            {
                PermissionId = permmissionId,
                RoleId = roleId
            });
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePermissionToRoleAsync(int permmissionId, string roleId)
        {
            if (roleId == null) throw new ArgumentNullException(nameof(roleId));
            var rolePermission = await _context.RolePermissions.FirstOrDefaultAsync(x => x.PermissionId == permmissionId && x.RoleId == roleId);
            if (rolePermission == null)
            {
                return false;
            }
            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRolePermissionsAsync(string roleName, IEnumerable<int> permissionIds)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return false;
            }
            // Remove existing permissions
            var currentRolePermissions = await _context.RolePermissions
                                                    .Where(rp => rp.RoleId == role.Id)
                                                    .ToListAsync();
            _context.RolePermissions.RemoveRange(currentRolePermissions);

            var existingPermissionIds = _context.Permissions
                                            .Where(rp => permissionIds.Contains(rp.Id))
                                            .Select(rp => rp.Id)
                                            .ToList();

            if (!permissionIds.All(id => existingPermissionIds.Contains(id)))
                return false;

            // Add new permissions
            var newRolePermissions = permissionIds.Select(permissionId => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });

            _context.RolePermissions.AddRange(newRolePermissions);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserHasPermissionAsync(ClaimsPrincipal user, string permissionName)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return false;
            var appUser = await _userManager.FindByIdAsync(userId);
            if (appUser == null)
                return false;
            var roles = _userManager.GetRolesAsync(appUser).Result;
            var permissions = await _context.RolePermissions
                .Where(rp => roles.Contains(rp.Role.Name))
                .Select(rp => rp.Permission.Name)
                .ToListAsync();
            return permissions.Contains(permissionName);
        }

        public async Task<List<PermissionDto>> GetRolePermissionsAsync(string roleId)
        {
            var permissions = await _context.RolePermissions.Where(rp => rp.RoleId == roleId).Include(rp => rp.Permission).Select(rp => rp.Permission).ToListAsync();
            return _mapper.Map<List<PermissionDto>>(permissions);
        }
    }
}
