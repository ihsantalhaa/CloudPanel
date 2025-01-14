using CloudPanel.WebApi.Dtos.GroupUserDtos;

namespace CloudPanel.WebApi.Repositories.GroupUserRepository
{
    public interface IGroupUserRepository
    {
        Task<List<GetGroupUserByUserIdDto>> GetGroupUserByUserIdAsync(int userId);
        Task<List<GetGroupUserByGroupIdDto>> GetGroupUserByGroupIdAsync(int groupId);
        Task CreateGroupUserAsync(CreateGroupUserDto createGroupUserDto);
        Task DeleteGroupUserAsync(int userId, int groupId);
    }
}
