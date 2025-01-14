using Amazon.Runtime;
using CloudPanel.WebApp.Models;
using CloudPanel.WebApp.Models.Dtos;
using CloudPanel.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ThirdParty.Json.LitJson;

namespace CloudPanel.WebApp.Controllers
{
    public class GroupController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<IActionResult> ListUserGroupsView()
        {
            var userId = User.FindFirstValue("userId");
            if (userId != null)
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:7189/api/GroupUsers/GetGroupUserByUserIdAsync/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<List<UserGroup>>(jsonData);
                    return View(values);
                }
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> ListGroupFilesView(int? groupId)
        {
            if (groupId != null)
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:7189/api/GroupFiles/GetGroupFilesAndUsersAsync/{groupId}");
                var response2 = await client.GetAsync($"https://localhost:7189/api/Groups/GetGroupByIdAsync/{groupId}");
                if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<List<GroupFilesAndUserVM>>(jsonData);

                    var jsonData2 = await response2.Content.ReadAsStringAsync();
                    UserGroup? group = JsonSerializer.Deserialize<UserGroup>(jsonData2);
                    TempData["GroupId"] = groupId;
                    ViewData["Name"] = group?.groupName;
                    ViewData["Description"] = group?.description;

                    return View(values);
                }
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> GroupAddFileView(int? groupId)
        {
            var userId = User.FindFirstValue("userId");
            if (userId != null && groupId != null)
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://localhost:7189/api/Files/GetUserFilesByIdAsync/{userId}");
                var response2 = await client.GetAsync($"https://localhost:7189/api/Groups/GetGroupByIdAsync/{groupId}");
                if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<List<GroupAddFileVM>>(jsonData);
                    if (values != null)
                    {
                        values.ForEach(item => item.groupId = groupId.Value); // LINQ ile daha kısa atama
                    }

                    var jsonData2 = await response2.Content.ReadAsStringAsync();
                    UserGroup? group = JsonSerializer.Deserialize<UserGroup>(jsonData2);
                    ViewData["Name"] = group?.groupName;
                    ViewData["Description"] = group?.description;

                    return View(values);
                }
            }
            return BadRequest("Api Error!");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupAddFile(GroupAddFileVM model)
        {
            GroupFile groupFile = new()
            {
                groupId = model.groupId,
                fileId = model.fileId,
            };
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(groupFile);
            StringContent content = new(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7189/api/GroupFiles/CreateGroupFileAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGroupFilesView", "Group", new { model.groupId });
            }
            return BadRequest("File is already in group!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupDeleteFile(GroupFilesByFileIdVM model)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.DeleteAsync($"https://localhost:7189/api/GroupFiles/DeleteGroupFileAsync/{model.fileId}/{model.groupId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListFileGroupsView", "File", new { model.fileId });
            }
            return BadRequest("File not in the group!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadGroupFile(int fileId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/Files/GetFileByIdAsync/{fileId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var file = JsonSerializer.Deserialize<GetFileDto>(jsonData);
                if (file != null)
                {
                    var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
                    query["FilePath"] = file.filePath;
                    query["FileName"] = file.fileName;

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
