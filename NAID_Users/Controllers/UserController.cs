using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NAID_Users.Dtos.User;
using NAID_Users.Extensions;
using NAID_Users.Interfaces;
using NAID_Users.Mappers;
using NAID_Users.Models;

namespace NAID_Users.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRolePermissionService _rolePermissionService;
        private readonly IMapper _mapper;
        public UserController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager, RoleManager<Role> roleManager, IRolePermissionService rolePermissionService, IMapper mapper1)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _rolePermissionService = rolePermissionService;
            _mapper = mapper1;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };
                if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
                {
                    return BadRequest("Email is already registered.");
                }
                var userResult = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (userResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "user");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new NewUserDto
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                            Token = _tokenService.CreateToken(appUser, "user")
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, userResult.Errors);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] LoginDto logInDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == logInDto.UserNameOrEmail);
                if (user == null)
                {
                    user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == logInDto.UserNameOrEmail.ToLower());
                    if (user == null)
                    {
                        return Unauthorized("Invalid username or email");
                    }
                }
                var PasswordCheckResult = await _signInManager.CheckPasswordSignInAsync(user, logInDto.Password, false);
                var roles = await _userManager.GetRolesAsync(user);

                if (PasswordCheckResult.Succeeded)
                {
                    return Ok(
                        new UserLogInInfo
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            Token = _tokenService.CreateToken(user, roles[0]),
                            Role = roles[0]
                        }
                    );
                }
                else
                {
                    return Unauthorized("Wrong Password");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpGet("getuserinrole/{roleName}")]
        public async Task<IActionResult> GetUsersInRole([FromRoute] string roleName)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return NotFound($"role {roleName} does not exist");
                }
                var users = await _userManager.GetUsersInRoleAsync(roleName);
                var usersDtos = _mapper.Map<List<UserInfoDto>>(users);
                return Ok(usersDtos);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        [HttpPost("change-role/{userId}")]
        public async Task<IActionResult> ChangeUserRole(string userId, [FromBody] string roleName)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return NotFound($"role {roleName} does not exist");
                }

                var currentRoles = await _userManager.GetRolesAsync(user);

                // Check if the user already has a role
                if (currentRoles.Any())
                {
                    // Remove the current role
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        return StatusCode(500, new { message = "Failed to remove current role", details = removeResult.Errors });
                    }
                }

                // Add the new role
                var addResult = await _userManager.AddToRoleAsync(user, roleName);
                if (!addResult.Succeeded)
                {
                    return StatusCode(500, new { message = "Failed to add user to new role", details = addResult.Errors });
                }

                return Ok(new { message = "User role updated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "An error occurred while changing user role", details = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userName = User.GetUserName();
                if (userName == null)
                {
                    return BadRequest(new { message = "user is not authenicated" });
                }
                var user = await _userManager.FindByNameAsync(userName);
                var roles = await _userManager.GetRolesAsync(user);
                var role = await _roleManager.FindByNameAsync(roles[0]);
                var permissions = await _rolePermissionService.GetRolePermissionsAsync(role.Id);
                return Ok(user.UserToUserMeInfo(role.Name, permissions));
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
    }
}
