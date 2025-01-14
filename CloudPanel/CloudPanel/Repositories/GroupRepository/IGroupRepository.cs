using CloudPanel.WebApi.Dtos.GroupDtos;

namespace CloudPanel.WebApi.Repositories.GroupRepository
{
    public interface IGroupRepository
    {
        Task<List<ResultGroupDto>> GetAllGroupsAsync();
        Task CreateGroupAsync(CreateGroupDto createGroupDto);
        Task DeleteGroupAsync(int id);
        Task UpdateGroupAsync(UpdateGroupDto updateGroupDto);
        Task<GetByIdGroupDto> GetGroupByIdAsync(int id);
    }
}
