using CloudPanel.WebApp.Models;
using CloudPanel.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Abstractions;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace CloudPanel.WebApp.Controllers
{
    public class PanelController(IHttpClientFactory httpClientFactory) : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue("userId");
        
            
            //var roless = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
          
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7189/api/Logs/GetLogStreamsAsync");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<List<LogStreamVM>>(jsonData);
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> LogEventsView(string item)
        {
            
            var client = _httpClientFactory.CreateClient();
            var asd = WebUtility.UrlEncode(item);
            var response = await client.GetAsync($"https://localhost:7189/api/Logs/GetLogEventsAsync/{asd}");
            List<LogEventVM> modelList= [];
            //var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<Dictionary<string,string>>>(jsonData);

                foreach (var i in items!)
                {
                    var ingestionTime = i.ContainsKey("ingestionTime") ? i["ingestionTime"].ToString() : null;
                    var timestamp = i.ContainsKey("timestamp") ? i["timestamp"].ToString() : null;
                    var message = i.ContainsKey("message") ? i["message"].ToString() : null;

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                    };
                    var jsonObject = JsonNode.Parse(message);
                    var message2 = jsonObject?.ToJsonString(options).Replace("\r\n", "<br>").Replace("\"", "").Replace(",", "").Replace("{", "").Replace("}", "");
                   

                    LogEventVM model = new LogEventVM
                    {
                        ingestionTime = ingestionTime,
                        message = message2,
                        timestamp = timestamp
                    };
                    modelList.Add(model);
                }

                return View(modelList);
            }
            return BadRequest("Api Error!");
        }

        //------------------------------User
        public async Task<IActionResult> ListUsersView()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7189/api/Users/UserListAsync");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<List<User>>(jsonData);
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(User user)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(user);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7189/api/Users/CreateUserAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListUsersView", "Panel");
            }
            return BadRequest("Api Error!");
        }

        public IActionResult AddUserView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7189/api/Users/DeleteUserAsync/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListUsersView", "Panel");
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> UpdateUserView(int userId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/Users/GetUserByIdAsync/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<User>(jsonData);
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(user);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("https://localhost:7189/api/Users/UpdateUserAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListUsersView", "Panel");
            }
            return BadRequest("Api Error!");
        }

        //------------------------------File
        public async Task<IActionResult> ListFilesView()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7189/api/Files/FileListAsync");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<List<S3File>>(jsonData);
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        //------------------------------Role
        public async Task<IActionResult> ListRolesView()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7189/api/Roles/RoleListAsync");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<List<Role>>(jsonData);
                //return BadRequest("EEEEEEEEE");
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        public IActionResult AddRoleView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(Role role)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(role);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7189/api/Roles/CreateRoleAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListRolesView","Panel",role.roleId);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7189/api/Roles/DeleteRoleAsync/{roleId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListRolesView", "Panel");
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> UpdateRoleView(int roleId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/Roles/GetRoleByIdAsync/{roleId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<Role>(jsonData);
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(Role role)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(role);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("https://localhost:7189/api/Roles/UpdateRoleAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListRolesView", "Panel");
            }
            return View();
        }

        public async Task<IActionResult> RoleAddUserView(int roleId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7189/api/Users/UserListAsync");
            var response2 = await client.GetAsync($"https://localhost:7189/api/Roles/GetRoleByIdAsync/{roleId}");
            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var jsonData2 = await response2.Content.ReadAsStringAsync();
                Role? role = JsonSerializer.Deserialize<Role>(jsonData2);
                ViewData["Name"] = role?.roleName;
                var values = JsonSerializer.Deserialize<List<User>>(jsonData);
                List<RoleAddDeleteUserVM> modelList = [];
                if (values != null)
                {
                    foreach (var item in values)
                    {
                        modelList.Add(new RoleAddDeleteUserVM { 
                            roleId = roleId,
                            userId = item.userId,
                            username = item.username
                        });
                    }
                }
                return View(modelList);
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleAddUser(RoleAddDeleteUserVM model)
        {
            RoleUser roleUser = new()
            {
                roleId = model.roleId,
                userId = model.userId,
            };
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(roleUser);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7189/api/RoleUsers/CreateRoleUserAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListRoleUsersView", "Panel", new { model.roleId});
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoleDeleteUser(RoleAddDeleteUserVM model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7189/api/RoleUsers/DeleteRoleUserAsync/{model.userId}/{model.roleId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListRoleUsersView", "Panel", new { model.roleId });
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> ListRoleUsersView(int roleId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/RoleUsers/GetRoleUserByRoleIdAsync/{roleId}");
            var response2 = await client.GetAsync($"https://localhost:7189/api/Roles/GetRoleByIdAsync/{roleId}");
            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var jsonData2 = await response2.Content.ReadAsStringAsync();
                Role? role = JsonSerializer.Deserialize<Role>(jsonData2);
                //HttpContext.Session.SetInt32("ListingRoleUsers-RoleId", roleId);
                TempData["RoleId"] = roleId;
                ViewData["Name"] = role?.roleName;
                var values = JsonSerializer.Deserialize<List<RoleUserByRoleIdVM>>(jsonData);
                //return RedirectToAction("EEEEEEEEE");
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        //------------------------------Group
        public async Task<IActionResult> ListGroupsView()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7189/api/Groups/GroupListAsync");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<List<UserGroup>>(jsonData);
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        public IActionResult AddGroupView()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGroup(UserGroup group)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(group);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7189/api/Groups/CreateGroupAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGroupsView", "Panel", group.groupId);
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7189/api/Groups/DeleteGroupAsync/{groupId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGroupsView", "Panel");
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> UpdateGroupView(int groupId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/Groups/GetGroupByIdAsync/{groupId}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonSerializer.Deserialize<UserGroup>(jsonData);
                return View(values);
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGroup(UserGroup group)
        {
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(group);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("https://localhost:7189/api/Groups/UpdateGroupAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGroupsView", "Panel");
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> GroupAddUserView(int groupId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7189/api/Users/UserListAsync");
            var response2 = await client.GetAsync($"https://localhost:7189/api/Groups/GetGroupByIdAsync/{groupId}");
            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var jsonData2 = await response2.Content.ReadAsStringAsync();
                UserGroup? group = JsonSerializer.Deserialize<UserGroup>(jsonData2);
                ViewData["Name"] = group?.groupName;
                var values = JsonSerializer.Deserialize<List<User>>(jsonData);
                List<GroupAddDeleteUserVM> modelList = [];
                if (values != null)
                {
                    foreach (var item in values)
                    {
                        modelList.Add(new GroupAddDeleteUserVM
                        {
                            groupId = groupId,
                            userId = item.userId,
                            username = item.username
                        });
                    }
                }
                return View(modelList);
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupAddUser(GroupAddDeleteUserVM model)
        {
            GroupUser groupUser = new()
            {
                groupId = model.groupId,
                userId = model.userId,
            };
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonSerializer.Serialize(groupUser);
            StringContent content = new (jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7189/api/GroupUsers/CreateGroupUserAsync", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGroupUsersView", "Panel", new { model.groupId });
            }
            return BadRequest("Api Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GroupDeleteUser(GroupUserByGroupIdVM model)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7189/api/GroupUsers/DeleteGroupUserAsync/{model.userId}/{model.groupId}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListGroupUsersView", "Panel", new { model.groupId });
            }
            return BadRequest("Api Error!");
        }

        public async Task<IActionResult> ListGroupUsersView(int groupId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7189/api/GroupUsers/GetGroupUserByGroupIdAsync/{groupId}");
            var response2 = await client.GetAsync($"https://localhost:7189/api/Groups/GetGroupByIdAsync/{groupId}");
            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var jsonData2 = await response2.Content.ReadAsStringAsync();
                UserGroup? group = JsonSerializer.Deserialize<UserGroup>(jsonData2);
                TempData["GroupId"] = groupId;
                //HttpContext.Session.SetInt32("ListingGroupUsers-GroupId", groupId);
                ViewData["Name"] = group?.groupName;
                var values = JsonSerializer.Deserialize<List<GroupUserByGroupIdVM>>(jsonData);
                //return RedirectToAction("EEEEEEEEE");
                return View(values);
            }
            return BadRequest("Api Error!");
        }



    }
}