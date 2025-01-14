using Amazon.S3;
using Amazon.S3.Model;
using CloudPanel.WebApi.Controllers;
using CloudPanel.WebApi.Dtos.FileDtos;
using CloudPanel.WebApi.Dtos.RoleDtos;
using CloudPanel.WebApi.Repositories.FileRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CloudPanel.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileRepository fileRepository, ILogger<FilesController> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }

    [HttpGet("FileListAsync")]
    public async Task<IActionResult> FileListAsync()
    {
        var values = await _fileRepository.GetAllFilesAsync();
        _logger.LogInformation("All files are listed");
        return Ok(values);
    }

    [HttpPost("CreateFileAsync")]
    public async Task<IActionResult> CreateFileAsync(CreateFileDto createFileDto)
    {
        await _fileRepository.CreateFileAsync(createFileDto);
        _logger.LogInformation($"{createFileDto.UserId} created {createFileDto.FileName}");
        return Ok("File created successfully.");
    }

    [HttpDelete("DeleteFileAsync/{fileId}")]
    public async Task<IActionResult> DeleteFileAsync(int fileId)
    {
        await _fileRepository.DeleteFileAsync(fileId);
        _logger.LogInformation($"File with id {fileId} deleted");
        return Ok("File deleted successfully.");
    }

    [HttpPut("UpdateFileAsync")]
    public async Task<IActionResult> UpdateFileAsync(UpdateFileDto updateFileDto)
    {
        await _fileRepository.UpdateFileAsync(updateFileDto);
        _logger.LogInformation($"File named {updateFileDto.FileName} description changed");
        return Ok("File updated successfully.");
    }

    [HttpGet("GetFileByIdAsync/{fileId}")]
    public async Task<IActionResult> GetFileByIdAsync(int fileId)
    {
        var values = await _fileRepository.GetFileByIdAsync(fileId);
        _logger.LogInformation($"File with id {fileId} getted from db");
        return Ok(values);
    }

    [HttpGet("GetUserFilesByIdAsync/{userId}")]
    public async Task<IActionResult> GetUserFilesByIdAsync(int userId)
    {
        var values = await _fileRepository.GetUserFilesByIdAsync(userId);
        _logger.LogInformation($"User with id {userId} listed own files");
        return Ok(values);
    }

}
