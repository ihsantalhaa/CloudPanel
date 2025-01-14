using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Runtime;
using CloudPanel.WebApp.Models;
using CloudPanel.WebApp.Models.Dtos;
using CloudPanel.WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory) : Controller
{

    private readonly ILogger<HomeController> _logger = logger;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? ErrorMessage)
    {
        ViewData["ErrorMessage"] = ErrorMessage;
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> Login(LoginVM model)
    {
        var client = _httpClientFactory.CreateClient();
        var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://localhost:7189/api/Auth/Login", content);

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonData);
            var tokenElement = jsonDocument.RootElement.GetProperty("token");
            var tokenModel = JsonSerializer.Deserialize<JwtResponseDto>(tokenElement);

            if (tokenModel != null && tokenModel.authToken != null)
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var readedToken = handler.ReadJwtToken(tokenModel.authToken);

                var claims = new List<Claim>
                {
                    new Claim("AuthToken", tokenModel.authToken),
                    new Claim("userId", readedToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? ""),
                    //new Claim("roles", JsonSerializer.Serialize(tokenModel.roles))
                };

                var roles = readedToken.Claims.Where(x => x.Type == "roles" || x.Type == ClaimTypes.Role)
                                     .Select(x => x.Value);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                var authProps = new AuthenticationProperties
                {
                    ExpiresUtc = tokenModel.accessTokenExpireDate,
                    IsPersistent = true,
                    IssuedUtc = DateTime.UtcNow
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProps);

                return RedirectToAction("ListUserFilesView", "File");
            }
        }

        ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi");
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

 }