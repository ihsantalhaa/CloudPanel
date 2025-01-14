using CloudPanel.WebApp.Models.ViewModels;
using CloudPanel.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using CloudPanel.WebApp.Models.Dtos;

namespace CloudPanel.WebApp.Controllers
{
    public class FileController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        
        public async Task<IActionResult> ListUserFilesView()
        {
            var userId = User.FindFirstValue("userId");
            if (userId != null) {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:7189/api/Files/GetUserFilesByIdAsync/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<List<S3File>>(jsonData);
                    return View(values);
                }
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> UpdateFileView(int fileId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/Files/GetFileByIdAsync/{fileId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<S3File>(jsonData);
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFile(S3File s3File)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(s3File);
            StringContent content = new(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("https://localhost:7189/api/Files/UpdateFileAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListUserFilesView", "File");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var userId = User.FindFirstValue("userId");
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/Files/GetFileByIdAsync/{fileId}");
            if (userId != null && response.IsSuccessStatusCode) 
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var file = JsonSerializer.Deserialize<GetFileDto>(jsonData);
                //var response = await client.DeleteAsync($"https://localhost:7189/api/Files/DeleteFileAsync/{fileId}");
                if (file != null)
                {
                    DeleteFileDto model = new DeleteFileDto
                    {
                        filePath = userId,
                        fileName = file.fileName,
                    };
                    var jsonData2 = JsonSerializer.Serialize(model);
                    StringContent content = new(jsonData, Encoding.UTF8, "application/json");
                    var response2 = await client.PostAsync("https://localhost:7189/api/Buckets/DeleteFileAsync", content);
                    var response3 = await client.DeleteAsync($"https://localhost:7189/api/Files/DeleteFileAsync/{fileId}");
                    if (response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ListUserFilesView", "File");
                    }
                    return BadRequest("File can't deleted!");
                } 
            }
            return BadRequest("User or file not found!");
        }

        public async Task<IActionResult> ListFileGroupsView(int fileId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/GroupFiles/GetGroupFilesByFileIdAsync/{fileId}");
            var response2 = await client.GetAsync($"https://localhost:7189/api/Files/GetFileByIdAsync/{fileId}");
            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<List<GroupFilesByFileIdVM>>(jsonData);

                var jsonData2 = await response2.Content.ReadAsStringAsync();
                var file = JsonSerializer.Deserialize<S3File>(jsonData2);
                ViewData["Name"] = file?.fileName;

                return View(values);
            }
            return BadRequest("Api Error!");
        }

        public IActionResult UploadFileView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IFormFile? file, IFormCollection? data, string? description)
        {
            var userId = User.FindFirstValue("userId");

            if (file != null && file?.Length != 0 && data != null && userId != null)
            {
                var client = _httpClientFactory.CreateClient();
                using var content = new MultipartFormDataContent();
                using var fileStream = file!.OpenReadStream();
                using var fileContent = new StreamContent(fileStream);

                content.Add(fileContent, "File", file.FileName);
                content.Add(new StringContent(userId.ToString()), "FilePath");

                if (data != null)
                {
                    foreach (var key in data.Keys)
                    {
                        content.Add(new StringContent(data[key]!), key);
                    }
                }

                var response = await client.PostAsync("https://localhost:7189/api/Buckets/UploadFileAsync", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Dosya başarıyla yüklendi.";
                    var client2 = _httpClientFactory.CreateClient();
                    CreateFileDto model = new CreateFileDto
                    {
                        fileName = file.FileName,
                        filePath = userId.ToString(),
                        fileDescription = description,
                        userId = int.Parse(userId)
                    };
                    var jsonData = JsonSerializer.Serialize(model);
                    StringContent content2 = new(jsonData, Encoding.UTF8, "application/json");
                    var response2 = await client2.PostAsync("https://localhost:7189/api/Files/CreateFileAsync", content2);
                    if (response2.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ListUserFilesView", "File");
                    }
                    return BadRequest("File uploaded but can't write database!");
                }

                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"File upload error: {errorMessage}";
                }
            }
            return BadRequest("Form Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadFile(int? fileId)
        {
            var userId = User.FindFirstValue("userId");

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/Files/GetFileByIdAsync/{fileId}");
            if (userId != null && response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var file = JsonSerializer.Deserialize<GetFileDto>(jsonData);
                if (file != null)
                {
                    GetFileByKeyDto model = new GetFileByKeyDto
                    {
                        filePath = "1",//userId,
                        fileName = file.fileName,
                    };
                    var jsonData2 = JsonSerializer.Serialize(model);
                    StringContent content = new(jsonData, Encoding.UTF8, "application/json");

                    var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
                    query["FilePath"] = userId.ToString();
                    query["FileName"] = file.fileName;

                    //var response2 = await client.GetAsync($"https://localhost:7189/api/Buckets/GetFileByKeyAsync?{query}");
                    //if (response2.IsSuccessStatusCode)
                    var objectResponse = await client.GetAsync($"https://localhost:7189/api/Buckets/GetFileByKeyAsync?{query}");
                    if (objectResponse.IsSuccessStatusCode)
                    {
                        var fileStream = await objectResponse.Content.ReadAsStreamAsync();
                        var contentType = objectResponse.Content.Headers.ContentType!.MediaType;

                        return File(fileStream, contentType!, file.fileName);
                    }
                    else
                    {
                        var errorContent = await objectResponse.Content.ReadAsStringAsync();
                        return Content($"API Error: {objectResponse.StatusCode} - {errorContent}");
                    }
                }
            }
            return BadRequest("File can't downloaded!");
        }
    }
}
