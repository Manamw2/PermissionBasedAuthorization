using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NAID_Users.Dtos.Role;
using NAID_Users.Interfaces;
using NAID_Users.Models;

namespace NAID_Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly RoleManager<Role> _roleManager;
        public RoleController(IRoleService roleService, RoleManager<Role> roleManager)
        {
            _roleService = roleService;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _roleService.GetRolesAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] string roleName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _roleService.AddRoleAsync(roleName);
            return Ok(await _roleManager.FindByNameAsync(roleName));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole([FromBody] string roleName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!await _roleService.DeleteRoleAsync(roleName))
                return NotFound($"No Role with name {roleName}");
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> EditRoleName([FromBody] EditRoleDto editRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var name = await _roleService.EditRoleName(editRoleDto);
            if (name == null)
                return NotFound($"No Role with name {editRoleDto.CurrentRoleName}");
            return Ok($"Role name has changed form {editRoleDto.CurrentRoleName} to {name}");
        }
    }
}
