namespace CloudPanel.WebApi.Dtos.BucketDtos
{
    public class S3ObjectDto
    {
        public string? FileName { get; set; }
        public string? PresignedUrl { get; set; }
    }
}
