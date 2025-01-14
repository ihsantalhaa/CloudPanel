using CloudPanel.WebApi.Dtos.RoleUserDtos;

namespace CloudPanel.WebApi.Dtos.UserDtos
{
    public interface IUserRepository
    {
        Task<List<ResultUserDto>> GetAllUsersAsync();
        Task CreateUserAsync(CreateUserDto createUserDto);
        Task DeleteUserAsync(int id);
        Task UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<GetByIdUserDto> GetUserByIdAsync(int id);
        
    }
}
