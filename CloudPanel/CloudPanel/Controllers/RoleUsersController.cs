using CloudPanel.WebApi.Dtos.GroupUserDtos;
using CloudPanel.WebApi.Dtos.RoleUserDtos;
using CloudPanel.WebApi.Repositories.RoleUserRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CloudPanel.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleUsersController(IRoleUserRepository roleUserRepository, ILogger<RoleUsersController> logger) : Controller
    {
        private readonly IRoleUserRepository _roleUserRepository = roleUserRepository;
        private readonly ILogger<RoleUsersController> _logger = logger;

        [HttpPost("CreateRoleUserAsync")]
        public async Task<IActionResult> CreateRoleUserAsync(CreateRoleUserDto createRoleUserDto)
        {
            await _roleUserRepository.CreateRoleUserAsync(createRoleUserDto);
            _logger.LogInformation($"User with id {createRoleUserDto.UserId} added to role with id {createRoleUserDto.RoleId}");
            return Ok("RoleUser created successfully.");
        }

        [HttpDelete("DeleteRoleUserAsync/{userId}/{roleId}")]
        public async Task<IActionResult> DeleteRoleUserAsync(int userId, int roleId)
        {
            await _roleUserRepository.DeleteRoleUserAsync(userId, roleId);
            _logger.LogInformation($"User with id {userId} deleted to role with id {roleId}");
            return Ok("RoleUser deleted successfully.");
        }

        [HttpGet("GetRoleUserByUserIdAsync/{userId}")]
        public async Task<IActionResult> GetRoleUserByUserIdAsync(int userId)
        {
            var values = await _roleUserRepository.GetRoleUserByUserIdAsync(userId);
            _logger.LogInformation($"Roles of the user with id {userId} are listed");
            return Ok(values);
        }

        [HttpGet("GetRoleUserByRoleIdAsync/{roleId}")]
        public async Task<IActionResult> GetRoleUserByRoleIdAsync(int roleId)
        {
            var values = await _roleUserRepository.GetRoleUserByRoleIdAsync(roleId);
            _logger.LogInformation($"Users of the role with id {roleId} are listed");
            return Ok(values);
        }


    }
}
