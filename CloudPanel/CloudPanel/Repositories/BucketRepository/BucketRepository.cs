using Amazon.S3.Transfer;
using Amazon.S3;
using CloudPanel.WebApi.Dtos.BucketDtos;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using ZstdSharp;
using System.Linq.Expressions;
using CloudPanel.WebApi.Models.DapperContext;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using System.Net;

namespace CloudPanel.WebApi.Repositories.BucketRepository
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _amazonS3;
        public BucketRepository(IConfiguration configuration, IAmazonS3 amazonS3)
        {
            _configuration = configuration;
            _amazonS3 = amazonS3;
        }

        public async Task UploadFileAsync(UploadS3fileDto uploadS3FileDto)
        {
            try
            {
                string prefix = uploadS3FileDto.FilePath!;
                IFormFile file = uploadS3FileDto.File!;
                string bucketName = _configuration["AWS:BucketName"]!;
                var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
                if (!bucketExists) throw new Exception($"Bucket {bucketName} does not exist.");
                var request = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                    InputStream = file.OpenReadStream()
                };
                request.Metadata.Add("Content-Type", file.ContentType);
                await _amazonS3.PutObjectAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"S3 file error: {ex.Message}");
            }
        }

        public async Task<List<S3ObjectDto>> GetAllFilesAsync(GetAllFileDto getAllFileDto)
        {
            try
            {
                string? prefix = getAllFileDto?.FilePath;
                string bucketName = _configuration["AWS:BucketName"]!;
                var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
                if (!bucketExists) throw new Exception($"Bucket {bucketName} does not exist.");
                var request = new ListObjectsV2Request()
                {
                    BucketName = bucketName,
                    Prefix = prefix
                };
                var result = await _amazonS3.ListObjectsV2Async(request);
                var s3Objects = result.S3Objects.Select(s =>
                {
                    var urlRequest = new GetPreSignedUrlRequest()
                    {
                        BucketName = bucketName,
                        Key = s.Key,
                        Expires = DateTime.UtcNow.AddMinutes(1)
                    };
                    return new S3ObjectDto()
                    {
                        FileName = s.Key.ToString(),
                        PresignedUrl = _amazonS3.GetPreSignedURL(urlRequest),
                    };
                });
                List<S3ObjectDto> values = s3Objects.ToList();
                return values;
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"S3 file error: {ex.Message}");
            }
        }

        public async Task<FileStreamResult> GetFileByKeyAsync(GetFileByKeyDto getFileByKeyDto)
        {
            string key = getFileByKeyDto.FilePath + "/" + getFileByKeyDto.FileName;
            string bucketName = _configuration["AWS:BucketName"]!;

            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                var response = await _amazonS3.GetObjectAsync(request);

                //Response.Headers.Add("Content-Disposition", $"attachment; filename={getFileByKeyDto.FileName}");
                return new FileStreamResult(response.ResponseStream, response.Headers.ContentType);
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"S3 file error: {ex.Message}");
            }

            //try
            //{
            //    string key = getFileByKeyDto.FilePath + "/" + getFileByKeyDto.FileName;
            //    string bucketName = _configuration["AWS:BucketName"]!;
            //    var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            //    if (!bucketExists) throw new Exception($"Bucket {bucketName} does not exist.");
            //    var s3Object = await _amazonS3.GetObjectAsync(bucketName, key);
            //    return s3Object;
            //}
            //catch (AmazonS3Exception ex)
            //{
            //    throw new Exception($"S3 file error: {ex.Message}");
            //}
        }

        public async Task DeleteFileAsync(DeleteFileDto deleteFileDto)
        {
            try
            {
                string key = deleteFileDto.FilePath + "/" + deleteFileDto.FileName;
                string bucketName = _configuration["AWS:BucketName"]!;
                var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
                if (!bucketExists) throw new Exception($"Bucket {bucketName} does not exist");
                await _amazonS3.DeleteObjectAsync(bucketName, key);
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"S3 file error: {ex.Message}");
            }
        }


    }
}
