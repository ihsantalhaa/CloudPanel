using CloudPanel.WebApi.Dtos.GroupFileDtos;

namespace CloudPanel.WebApi.Repositories.GroupFileRepository
{
    public interface IGroupFileRepository
    {
        Task<List<GetGroupFilesAndUsersDto>> GetGroupFilesAndUsersAsync(int groupId);
        Task<List<GetGroupFilesByFileIdDto>> GetGroupFilesByFileIdAsync(int fileId);
        Task CreateGroupFileAsync(CreateGroupFileDto createGroupFileDto);
        Task DeleteGroupFileAsync(int fileId, int groupId);
    }
}
