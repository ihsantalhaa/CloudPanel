namespace CloudPanel.WebApp.Models.ViewModels
{
    public class GroupFilesByFileIdVM
    {
        public int fileId { get; set; }
        public int groupId { get; set; }
        public string? groupName { get; set; }
        public string? description { get; set; }
    }
}
