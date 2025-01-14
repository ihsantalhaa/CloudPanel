using CloudPanel.Controllers;
using CloudPanel.WebApi.Dtos.GroupFileDtos;
using CloudPanel.WebApi.Dtos.GroupUserDtos;
using CloudPanel.WebApi.Repositories.GroupUserRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CloudPanel.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupUsersController(IGroupUserRepository groupUserRepository, ILogger<GroupUsersController> logger) : Controller
    {
        private readonly IGroupUserRepository _groupUserRepository = groupUserRepository;
        private readonly ILogger<GroupUsersController> _logger = logger;

        [HttpPost("CreateGroupUserAsync")]
        public async Task<IActionResult> CreateGroupUserAsync(CreateGroupUserDto createGroupUserDto)
        {
            await _groupUserRepository.CreateGroupUserAsync(createGroupUserDto);
            _logger.LogInformation($"User with id {createGroupUserDto.UserId} added to group with id {createGroupUserDto.GroupId}");
            return Ok("GroupUser created successfully.");
        }

        [HttpDelete("DeleteGroupUserAsync/{userId}/{groupId}")]
        public async Task<IActionResult> DeleteGroupUserAsync(int userId, int groupId)
        {
            await _groupUserRepository.DeleteGroupUserAsync(userId, groupId);
            _logger.LogInformation($"User with id {userId} deleted to group with id {groupId}");
            return Ok("GroupUser deleted successfully.");
        }

        [HttpGet("GetGroupUserByUserIdAsync/{userId}")]
        public async Task<IActionResult> GetGroupUserByUserIdAsync(int userId)
        {
            var values = await _groupUserRepository.GetGroupUserByUserIdAsync(userId);
            _logger.LogInformation($"Groups of the user with id {userId} are listed");
            return Ok(values);
        }

        [HttpGet("GetGroupUserByGroupIdAsync/{groupId}")]
        public async Task<IActionResult> GetGroupUserByGroupIdAsync(int groupId)
        {
            var values = await _groupUserRepository.GetGroupUserByGroupIdAsync(groupId);
            _logger.LogInformation($"Users of the group with id {groupId} are listed");
            return Ok(values);
        }


    }
}
