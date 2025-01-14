using CloudPanel.Controllers;
using CloudPanel.WebApi.Dtos.GroupFileDtos;
using CloudPanel.WebApi.Repositories.GroupFileRepository;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CloudPanel.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupFilesController : Controller
    {
        private readonly IGroupFileRepository _groupFileRepository;
        private readonly ILogger<GroupFilesController> _logger;

        public GroupFilesController(IGroupFileRepository groupFileRepository, ILogger<GroupFilesController> logger)
        {
            _groupFileRepository = groupFileRepository;
            _logger = logger;
        }

        [HttpPost("CreateGroupFileAsync")]
        public async Task<IActionResult> CreateGroupFileAsync(CreateGroupFileDto createGroupFileDto)
        {
            await _groupFileRepository.CreateGroupFileAsync(createGroupFileDto);
            _logger.LogInformation($"File with id {createGroupFileDto.FileId} added to group with id {createGroupFileDto.GroupId}");
            return Ok("GroupFile created successfully.");
        }

        [HttpDelete("DeleteGroupFileAsync/{fileId}/{groupId}")]
        public async Task<IActionResult> DeleteGroupFileAsync(int fileId, int groupId)
        {
            await _groupFileRepository.DeleteGroupFileAsync(fileId, groupId);
            _logger.LogInformation($"File with id {fileId} deleted to group with id {groupId}");
            return Ok("GroupFile deleted successfully.");
        }

        [HttpGet("GetGroupFilesAndUsersAsync/{groupId}")]
        public async Task<IActionResult> GetGroupFilesAndUsersAsync(int groupId)
        {
            var values = await _groupFileRepository.GetGroupFilesAndUsersAsync(groupId);
            _logger.LogInformation($"Listed files and users of group with id {groupId}");
            return Ok(values);
        }

        [HttpGet("GetGroupFilesByFileIdAsync/{fileId}")]
        public async Task<IActionResult> GetGroupFilesByFileIdAsync(int fileId)
        {
            var values = await _groupFileRepository.GetGroupFilesByFileIdAsync(fileId);
            _logger.LogInformation($"The groups of the file with id {fileId} are listed");
            return Ok(values);
        }

    }
}
