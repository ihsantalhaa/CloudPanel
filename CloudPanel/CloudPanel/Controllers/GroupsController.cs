using CloudPanel.WebApi.Controllers;
using CloudPanel.WebApi.Dtos.GroupDtos;
using CloudPanel.WebApi.Repositories.GroupRepository;
using Microsoft.AspNetCore.Mvc;
namespace CloudPanel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IGroupRepository _groupRepository;
    private readonly ILogger<GroupsController> _logger;
    //private readonly ILogger<WeatherForecastController> _logger;

    public GroupsController(/*ILogger<WeatherForecastController> logger,*/ IGroupRepository groupRepository, ILogger<GroupsController> logger)
    {
        //_logger = logger;
        _groupRepository = groupRepository;
        _logger = logger;
    }

    [HttpGet("GroupListAsync")]
    public async Task<IActionResult> GroupListAsync()
    {
        var values = await _groupRepository.GetAllGroupsAsync();
        _logger.LogInformation("All groups are listed");
        return Ok(values);
    }

    [HttpPost("CreateGroupAsync")]
    public async Task<IActionResult> CreateGroupAsync(CreateGroupDto createGroupDto)
    {
        await _groupRepository.CreateGroupAsync(createGroupDto);
        _logger.LogInformation($"{createGroupDto.GroupName} created");
        return Ok("Group created successfully.");
    }

    [HttpDelete("DeleteGroupAsync/{id}")]
    public async Task<IActionResult> DeleteGroupAsync(int id)
    {
        await _groupRepository.DeleteGroupAsync(id);
        _logger.LogInformation($"Group with id {id} deleted");
        return Ok("Group deleted successfully.");
    }

    [HttpPut("UpdateGroupAsync")]
    public async Task<IActionResult> UpdateGroupAsync(UpdateGroupDto updateGroupDto)
    {
        await _groupRepository.UpdateGroupAsync(updateGroupDto);
        _logger.LogInformation($"Group with id {updateGroupDto.GroupId} updated");
        return Ok("Group updated successfully.");
    }

    [HttpGet("GetGroupByIdAsync/{id}")]
    public async Task<IActionResult> GetGroupByIdAsync(int id)
    {
        var values = await _groupRepository.GetGroupByIdAsync(id);
        _logger.LogInformation($"Group with id {id} getted from db");
        return Ok(values);
    }
}
