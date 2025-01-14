using CloudPanel.WebApi.Dtos.FileDtos;
using CloudPanel.WebApi.Dtos.RoleDtos;
using CloudPanel.WebApi.Dtos.RoleUserDtos;
using CloudPanel.WebApi.Dtos.UserDtos;
using CloudPanel.WebApi.Repositories.RoleRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudPanel.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        [HttpGet("UserListAsync")]
        public async Task<IActionResult> UserListAsync()
        {
            var values = await _userRepository.GetAllUsersAsync();
            _logger.LogInformation("All users are listed");
            return Ok(values);
        }

        [HttpPost("CreateUserAsync")]
        public async Task<IActionResult> CreateUserAsync(CreateUserDto createUserDto)
        {
            await _userRepository.CreateUserAsync(createUserDto);
            _logger.LogInformation($"{createUserDto} created");
            return Ok("User created successfully.");
        }

        [HttpDelete("DeleteUserAsync/{id}")]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);

            _logger.LogInformation($"User with id {id} deleted");
            return Ok("User deleted successfully.");
        }

        [HttpPut("UpdateUserAsync")]
        public async Task<IActionResult> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            await _userRepository.UpdateUserAsync(updateUserDto);
            _logger.LogInformation($"User with id {updateUserDto.UserId} updated");
            return Ok("User updated successfully.");
        }

        [HttpGet("GetUserByIdAsync/{id}")]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var values = await _userRepository.GetUserByIdAsync(id);
            _logger.LogInformation($"User with id {id} getted from db");
            return Ok(values);
        }

    }
}
