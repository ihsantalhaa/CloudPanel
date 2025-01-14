namespace CloudPanel.WebApp.Models.Dtos
{
    public class JwtResponseDto
    {
        public bool authenticateResult { get; set; }
        public string? authToken { get; set; }
        public DateTime accessTokenExpireDate { get; set; }
    }
}
