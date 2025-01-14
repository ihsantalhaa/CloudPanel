using CloudPanel.WebApi.Dtos.AuthDtos;
using CloudPanel.WebApi.Dtos.LoginDtos;

namespace CloudPanel.WebApi.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Task<TokenResponseDto> Login(LoginDto loginDto);
    }
}
