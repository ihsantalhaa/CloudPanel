using CloudPanel.WebApi.Dtos.RoleDtos;

namespace CloudPanel.WebApi.Repositories.RoleRepository
{
    public interface IRoleRepository
    {
        Task<List<ResultRoleDto>> GetAllRolesAsync();
        Task CreateRoleAsync(CreateRoleDto createRoleDto);
        Task DeleteRoleAsync(int id);
        Task UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<GetByIdRoleDto> GetRoleByIdAsync(int id);
    }
}
