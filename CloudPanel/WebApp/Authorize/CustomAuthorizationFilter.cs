using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CloudPanel.WebApp.Authorize
{
    public class CustomAuthorizationFilter : IAuthorizationFilter
    {
        private readonly RolesAuthorizationRequirement _requirement;

        public CustomAuthorizationFilter(RolesAuthorizationRequirement requirement)
        {
            _requirement = requirement;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //var user = context.HttpContext.User;
            //if (!user.Identity.IsAuthenticated)
            //{
            //    context.Result = new UnauthorizedResult();
            //    return;
            //}

            //var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            //var userRoles = _userRepository.GetUserRoles(userId);

            //if (!_requirement.AllowedRoles.Any(role => userRoles.Contains(role)))
            //{
            //    context.Result = new ForbidResult();
            //}
        }
    }

    public class RolesAuthorizationRequirement
    {
        public string[] AllowedRoles { get; }

        public RolesAuthorizationRequirement(string[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
        }
    }

}