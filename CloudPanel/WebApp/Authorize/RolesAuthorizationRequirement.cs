public class RolesAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public RolesAuthorizationRequirement(string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}