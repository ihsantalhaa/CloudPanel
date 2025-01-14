using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CloudPanel.WebApi.Dtos.BucketDtos;
using CloudPanel.WebApi.Dtos.LoginDtos;
using CloudPanel.WebApi.Repositories.AuthRepository;
using CloudPanel.WebApi.Repositories.BucketRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI;
using System.Linq;
using System.Text.Json;

namespace CloudPanel.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BucketsController : ControllerBase
    {
        private readonly IBucketRepository _bucketRepository;
        private readonly ILogger<BucketsController> _logger;

        public BucketsController(IBucketRepository bucketRepository, IConfiguration configuration, ILogger<BucketsController> logger)
        {
            _bucketRepository = bucketRepository;
            _logger = logger;
        }



        //[HttpGet("GABucket")]
        //public async Task<IActionResult> GABucket()
        //{
        //    var data = await _amazonS3.ListBucketsAsync();
        //    var buckets = data.Buckets.Select(b => { return b.BucketName; });
        //    return Ok(buckets);
        //}

        //[HttpPost("CBucket")]
        //public async Task<IActionResult> CBucket(string bucketName)
        //{
        //    var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
        //    if (bucketExists) return BadRequest($"Bucket {bucketName} already exists.");
        //    await _amazonS3.PutBucketAsync(bucketName);
        //    return Created("buckets", $"Bucket {bucketName} created.");
        //}

        //[HttpDelete("DBucket")]
        //public async Task<IActionResult> DBucket(string bucketname)
        //{
        //    await _amazonS3.DeleteBucketAsync(bucketname);
        //    return NoContent();
        //}



        [HttpPost("UploadFileAsync")]
        public async Task<IActionResult> UploadFileAsync(UploadS3fileDto uploadS3FileDto)
        {
            await _bucketRepository.UploadFileAsync(uploadS3FileDto);
            _logger.LogInformation($"File {uploadS3FileDto.File?.FileName} was uploaded to s3 by user with id {uploadS3FileDto.FilePath}");
            return Ok("File uploaded successfully.");
        }

        [HttpPost("GetAllFilesAsync")]
        public async Task<IActionResult> GetAllFilesAsync(GetAllFileDto getAllFileDto)
        {
            var values = await _bucketRepository.GetAllFilesAsync(getAllFileDto);
            _logger.LogInformation($"Listed user files with id: {getAllFileDto.FilePath}");
            return Ok(values);
        }

        [HttpGet("GetFileByKeyAsync")]
        public async Task<FileStreamResult> GetFileByKeyAsync([FromQuery] GetFileByKeyDto getFileByKeyDto)
        {
            var s3Object = await _bucketRepository.GetFileByKeyAsync(getFileByKeyDto);
            _logger.LogInformation($"File {getFileByKeyDto.FileName} of user with id {getFileByKeyDto.FilePath} was downloaded");
            return s3Object;
        }

        [HttpPost("DeleteFileAsync")]
        public async Task<IActionResult> DeleteFileAsync(DeleteFileDto deleteFileDto)
        {
            await _bucketRepository.DeleteFileAsync(deleteFileDto);
            _logger.LogInformation($"User with id {deleteFileDto.FilePath} deleted file named {deleteFileDto.FileName}");
            return Ok($"File {deleteFileDto.FileName} was deleted.");
        }










        //[HttpPost("UploadFile")]
        //public async Task<IActionResult> UploadSingleFileAsync(IFormFile file)
        //{
        //    await _bucketRepository.UploadSingleFileAsync(file);
        //    return Ok("File uploaded successfully.");
        //}

        //[HttpPost]
        //public async Task<IActionResult> UploadFileAsync(IFormFile file, string? prefix)
        //{
        //    string bucketName = _configuration["AWS:BucketName"]!;
        //    PutObjectRequest request = new()
        //    {
        //        BucketName = bucketName,
        //        Key = String.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
        //        InputStream = file.OpenReadStream()
        //    };
        //    request.Metadata.Add("Content-Type", file.ContentType);
        //    await _amazonS3.PutObjectAsync(request);
        //    return Ok($"File {prefix}/{file.FileName} uploaded to S3 successfully!");
        //}

        //// Dosya Listeleme
        //[HttpGet("list")]
        //public async Task<IActionResult> ListFiles()
        //{
        //    string BucketName = _configuration["AWS:BucketName"]!;
        //    try
        //    {
        //        var request = new ListObjectsV2Request
        //        {
        //            BucketName = BucketName
        //        };

        //        var response = await _amazonS3.ListObjectsV2Async(request);
        //        var files = response.S3Objects.Select(obj => obj.Key).ToList();

        //        return Ok(files);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Dosyalar listelenirken hata oluştu: {ex.Message}");
        //    }
        //}

        //// Dosya İndirme
        //[HttpGet("download/{fileName}")]
        //public async Task<IActionResult> DownloadFile(string fileName)
        //{
        //    string BucketName = _configuration["AWS:BucketName"]!;
        //    try
        //    {
        //        var request = new GetObjectRequest
        //        {
        //            BucketName = BucketName,
        //            Key = fileName
        //        };

        //        var response = await _amazonS3.GetObjectAsync(request);

        //        var stream = new MemoryStream();
        //        await response.ResponseStream.CopyToAsync(stream);
        //        stream.Position = 0;

        //        return File(stream, response.Headers.ContentType, fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Dosya indirilirken hata oluştu: {ex.Message}");
        //    }
        //}

        //// Dosya Silme
        //[HttpDelete("delete/{fileName}")]
        //public async Task<IActionResult> DeleteFile(string fileName)
        //{
        //    string BucketName = _configuration["AWS:S3BucketName"]!;
        //    try
        //    {
        //        var request = new DeleteObjectRequest
        //        {
        //            BucketName = BucketName,
        //            Key = fileName
        //        };

        //        await _amazonS3.DeleteObjectAsync(request);
        //        return Ok($"Dosya başarıyla silindi: {fileName}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Dosya silinirken hata oluştu: {ex.Message}");
        //    }
        //}
    }
}
