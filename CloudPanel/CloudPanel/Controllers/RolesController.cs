using CloudPanel.WebApi.Dtos.GroupDtos;
using CloudPanel.WebApi.Dtos.RoleDtos;
using CloudPanel.WebApi.Repositories.FileRepository;
using CloudPanel.WebApi.Repositories.GroupRepository;
using CloudPanel.WebApi.Repositories.RoleRepository;
using Microsoft.AspNetCore.Mvc;

namespace CloudPanel.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleRepository roleRepository, ILogger<RolesController> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        [HttpGet("RoleListAsync")]
        public async Task<IActionResult> RoleListAsync()
        {
            var values = await _roleRepository.GetAllRolesAsync();
            _logger.LogInformation("All roles are listed");
            return Ok(values);
        }

        [HttpPost("CreateRoleAsync")]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            await _roleRepository.CreateRoleAsync(createRoleDto);
            _logger.LogInformation($"{createRoleDto.RoleName} created");
            return Ok("Role created successfully.");
        }

        [HttpDelete("DeleteRoleAsync/{id}")]
        public async Task<IActionResult> DeleteRoleAsync(int id)
        {
            await _roleRepository.DeleteRoleAsync(id);
            _logger.LogInformation($"Role with id {id} deleted");
            return Ok("Role deleted successfully.");
        }

        [HttpPut("UpdateRoleAsync")]
        public async Task<IActionResult> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            await _roleRepository.UpdateRoleAsync(updateRoleDto);
            _logger.LogInformation($"Role with id {updateRoleDto.RoleId} updated");
            return Ok("Role updated successfully.");
        }

        [HttpGet("GetRoleByIdAsync/{id}")]
        public async Task<IActionResult> GetRoleByIdAsync(int id)
        {
            var values = await _roleRepository.GetRoleByIdAsync(id);
            _logger.LogInformation($"Role with id {id} getted from db");
            return Ok(values);
        }
    }
}
