using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Security.Claims;

namespace Wms.Web.Host.Auth;

/// <summary>
/// Checks JWT token claims for permissions before falling back to ABP's database-based permission store.
/// When the token contains "Wms.All" or the specific permission name (or a prefix match),
/// the requirement is satisfied immediately — no AbpPermissionGrants record needed.
/// </summary>
public class ClaimsPermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<ClaimsPermissionAuthorizationHandler> _logger;
    private readonly ICurrentPrincipalAccessor _principalAccessor;

    public ClaimsPermissionAuthorizationHandler(
        ILogger<ClaimsPermissionAuthorizationHandler> logger,
        ICurrentPrincipalAccessor principalAccessor)
    {
        _logger = logger;
        _principalAccessor = principalAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var principal = _principalAccessor.Principal;

        // 1. Wildcard: "Wms.All" grants every permission
        if (principal.Claims.Any(c => c.Type == "permission" && c.Value == "Wms.All"))
        {
            _logger.LogDebug("Permission granted via Wms.All wildcard: {Permission}", requirement.PermissionName);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 2. Direct match: token has the exact permission name
        var permissionValues = principal.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        if (permissionValues.Contains(requirement.PermissionName))
        {
            _logger.LogDebug("Permission granted via direct claim match: {Permission}", requirement.PermissionName);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 3. Prefix match: "AbpIdentity" in claims satisfies "AbpIdentity.Users"
        if (permissionValues.Any(p => requirement.PermissionName.StartsWith(p + ".", StringComparison.Ordinal)))
        {
            _logger.LogDebug("Permission granted via prefix match: claim={Claim}, required={Permission}",
                permissionValues.First(p => requirement.PermissionName.StartsWith(p + ".", StringComparison.Ordinal)),
                requirement.PermissionName);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 4. Admin role fallback (if role claim exists but no permission claims)
        var roleValues = principal.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (roleValues.Contains("admin") || roleValues.Contains("Admin"))
        {
            _logger.LogDebug("Permission granted via admin role fallback: {Permission}", requirement.PermissionName);
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Not found in claims — let ABP's default handler (database check) decide
        return Task.CompletedTask;
    }
}
