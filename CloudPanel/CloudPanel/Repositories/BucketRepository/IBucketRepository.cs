using Amazon.S3.Transfer;
using Amazon.S3;
using CloudPanel.WebApi.Dtos.BucketDtos;
using Microsoft.AspNetCore.Mvc;
using Amazon.S3.Model;

namespace CloudPanel.WebApi.Repositories.BucketRepository
{
    public interface IBucketRepository
    {
        Task UploadFileAsync(UploadS3fileDto uploadS3FileDto);
        Task<List<S3ObjectDto>> GetAllFilesAsync(GetAllFileDto getAllFileDto);
        Task<FileStreamResult> GetFileByKeyAsync(GetFileByKeyDto getFileByKeyDto);
        Task DeleteFileAsync(DeleteFileDto deleteFileDto);
    }
}
