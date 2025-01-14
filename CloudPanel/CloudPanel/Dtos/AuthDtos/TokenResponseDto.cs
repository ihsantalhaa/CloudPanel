namespace CloudPanel.WebApi.Dtos.AuthDtos
{
    public class TokenResponseDto
    {
        public bool AuthenticateResult { get; set; }
        public string? AuthToken { get; set; }
        public DateTime AccessTokenExpireDate { get; set; }
        
    }
}
