using CloudPanel.WebApi.Dtos.FileDtos;
using CloudPanel.WebApi.Dtos.GroupDtos;

namespace CloudPanel.WebApi.Repositories.FileRepository
{
    public interface IFileRepository
    {
        Task<List<ResultFileDto>> GetAllFilesAsync();
        Task<List<GetUserFilesDto>> GetUserFilesByIdAsync(int userId);
        Task CreateFileAsync(CreateFileDto createGroupDto);
        Task DeleteFileAsync(int fileId);
        Task UpdateFileAsync(UpdateFileDto updateGroupDto);
        Task<GetByIdFileDto> GetFileByIdAsync(int fileId);
    }
}
