namespace CloudPanel.WebApi.Dtos.RoleUserDtos
{
    public class GetRoleUserByRoleIdDto
    {
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        
    }
}
