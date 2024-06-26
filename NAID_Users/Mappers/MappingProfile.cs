using AutoMapper;
using NAID_Users.Dtos.Permissions;
using NAID_Users.Dtos.Role;
using NAID_Users.Dtos.User;
using NAID_Users.Models;

namespace NAID_Users.Mappers
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Role, RoleDto>();
            CreateMap<AppUser, UserInfoDto>();
            CreateMap<Permission, PermissionDto>();
        }
    }
}
