using CloudPanel.WebApi.Dtos.AuthDtos;
using CloudPanel.WebApi.Dtos.LoginDtos;
using CloudPanel.WebApi.Repositories.AuthRepository;
using Microsoft.AspNetCore.Mvc;

namespace CloudPanel.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepository authRepository, ILogger<AuthController> logger)
        {
            _authRepository = authRepository;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var token = await _authRepository.Login(loginDto);
                _logger.LogInformation($"The system was logged in with the {loginDto.Mail} email address");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An attempt was made to log in to the system with the {loginDto.Mail} email address.");
                return Unauthorized(ex.Message);
            }
        }


    }
}
