namespace CloudPanel.WebApp.Models.ViewModels
{
    public class RoleUserByRoleIdVM
    {
        public int roleId { get; set; }
        public int userId { get; set; }
        public string? username { get; set; }
        public string? mail { get; set; }
        public string? phone { get; set; }
    }
}
