using CloudPanel.WebApi.Dtos.RoleUserDtos;
using CloudPanel.WebApi.Dtos.UserDtos;

namespace CloudPanel.WebApi.Repositories.RoleUserRepository
{
    public interface IRoleUserRepository
    {
        Task<List<GetRoleUserByUserIdDto>> GetRoleUserByUserIdAsync(int userId);
        Task<List<GetRoleUserByRoleIdDto>> GetRoleUserByRoleIdAsync(int roleId);
        Task CreateRoleUserAsync(CreateRoleUserDto createRoleUserDto);
        Task DeleteRoleUserAsync(int userId, int roleId);
    }
}
