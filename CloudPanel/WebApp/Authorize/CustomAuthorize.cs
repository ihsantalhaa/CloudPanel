using CloudPanel.WebApp.Authorize;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

public class CustomAuthorize : TypeFilterAttribute
{
    public CustomAuthorize(params string[] roles) : base(typeof(CustomAuthorizationFilter))
    {
        Arguments = new[] { new RolesAuthorizationRequirement(roles) };
    }
}