using NAID_Users.Dtos.Permissions;
using NAID_Users.Dtos.User;
using NAID_Users.Models;

namespace NAID_Users.Mappers
{
    public static class UserMapper
    {
        public static UserMeInfo UserToUserMeInfo(this AppUser user, string roleName, List<PermissionDto> permissions)
        {
            return new UserMeInfo
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = roleName,
                Permissions = permissions
            };
        }
    }
}
