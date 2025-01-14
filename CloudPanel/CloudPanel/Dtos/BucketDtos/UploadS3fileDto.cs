namespace CloudPanel.WebApi.Dtos.BucketDtos
{
    public class UploadS3fileDto
    {
        public IFormFile? File { get; set; }
        public string? FilePath { get; set; }
    }
}
