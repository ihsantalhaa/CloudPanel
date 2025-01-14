using Amazon.Runtime.Internal;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using CloudPanel.WebApi.Controllers;
using CloudPanel.WebApi.Dtos.AuthDtos;
using CloudPanel.WebApi.Dtos.LoginDtos;
using CloudPanel.WebApi.Dtos.RoleUserDtos;
using CloudPanel.WebApi.Dtos.UserDtos;
using CloudPanel.WebApi.Models.DapperContext;
using CloudPanel.WebApi.Repositories.AuthRepository;
using CloudPanel.WebApi.Repositories.RoleRepository;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BC = BCrypt.Net.BCrypt;

public class AuthRepository : IAuthRepository
{
    private readonly Context _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthRepository> _logger;
    private readonly IRoleRepository _roleRepository;

    public AuthRepository(Context context, IConfiguration configuration, ILogger<AuthRepository> logger, IRoleRepository roleRepository) //, IAmazonSimpleNotificationService snsClient)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _roleRepository = roleRepository;
        //_snsClient = snsClient;
    }

    public async Task<TokenResponseDto> Login(LoginDto loginDto)
    {
        string query = "select * from Users where Mail=@mail";
        var parameters = new DynamicParameters();
        parameters.Add("@mail", loginDto.Mail);

        try
        {
            using (var connection = _context.CreateConnection())
            {
                AuthUserDto? user = await connection.QueryFirstOrDefaultAsync<AuthUserDto>(query, parameters);

                if (user == null || !BC.Verify(loginDto.Password, user.Password))
                {
                    throw new UnauthorizedAccessException("Geçersiz mail veya parola");
                }
                //await SendCodeAsync(user);
                return await GenerateJwtToken(user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"The following error was encountered when logging in with {loginDto.Mail} ​​email address: {ex}");
            throw new Exception($"Login Failed: {ex.Message}");
        }
    }

    //private async Task SendCodeAsync(AuthUserDto user)
    //{
    //    try
    //    {
    //        string verificationCode = GenerateVerificationCode();

    //        // SMS gönderme
    //        var publishRequest = new PublishRequest
    //        {
    //            Message = $"Hi {user.Username} your login verification code is: {verificationCode}",
    //            PhoneNumber = user.Phone,
    //        };

    //        var response = await _snsClient.PublishAsync(publishRequest);

    //        // Kodu geçici bellekte saklama
    //        _verificationCodes[user.Phone!] = verificationCode;

    //    }
    //    catch (Exception ex) {
    //        throw new Exception($"2fa code not send: {ex}");
    //    }
    //}

    private async Task<List<UserRoleDto>> GetUserRoles(int userId)
    {
        string query = "Select Roles.RoleName from RoleUsers inner join Roles on RoleUsers.RoleId = Roles.RoleId Where RoleUsers.UserId=@userId";
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId);
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var values = await connection.QueryAsync<UserRoleDto>(query, parameters);
                return values.ToList();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error fetching user roles: {ex.Message}");
        }
    }

    private async Task<TokenResponseDto> GenerateJwtToken(AuthUserDto user)
    {
        List<UserRoleDto> roles = await GetUserRoles(user.UserId);

        TokenResponseDto tokenResponseDto = new();

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        List<string>? userRoles = [];
        if (roles != null)
        {
            userRoles = roles.Select(r => r.RoleName).ToList()!;
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username!),
            new Claim("roles", JsonSerializer.Serialize(roles?.Select(r => r.RoleName).ToList())),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expireDate = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expireDate,
            signingCredentials: credentials
        );

        tokenResponseDto.AuthenticateResult = true;
        tokenResponseDto.AuthToken = new JwtSecurityTokenHandler().WriteToken(token);
        tokenResponseDto.AccessTokenExpireDate = expireDate;

        return tokenResponseDto;
    }

   
}