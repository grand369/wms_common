using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace Wms.Web.Host.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : AbpControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IdentityUserManager _userManager;
    private readonly IIdentityRoleRepository _roleRepository;

    public AuthController(
        IConfiguration configuration,
        IdentityUserManager userManager,
        IIdentityRoleRepository roleRepository)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleRepository = roleRepository;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserNameOrEmailAddress) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = new { message = "Username/email and password are required." } });
        }

        var isEmail = request.UserNameOrEmailAddress.Contains('@');
        IdentityUser? user;

        if (isEmail)
        {
            user = await _userManager.FindByEmailAsync(request.UserNameOrEmailAddress);
        }
        else
        {
            user = await _userManager.FindByNameAsync(request.UserNameOrEmailAddress);
        }

        if (user == null)
        {
            return Unauthorized(new { error = new { message = "Invalid username/email or password." } });
        }

        var isValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValid)
        {
            return Unauthorized(new { error = new { message = "Invalid username/email or password." } });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var (accessToken, expiresIn) = GenerateJwtToken(user, roles);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = Guid.NewGuid().ToString("N"),
            ExpiresIn = expiresIn
        });
    }

    [HttpGet("current-user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = new { message = "Invalid token." } });
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Unauthorized(new { error = new { message = "User not found." } });
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserProfile
        {
            Id = user.Id.ToString(),
            UserName = user.UserName!,
            Email = user.Email ?? string.Empty,
            Name = user.Name,
            Surname = user.Surname,
            PhoneNumber = user.PhoneNumber,
            TenantId = null,
            Roles = roles.ToArray()
        });
    }

    /// <summary>
    /// All WMS granular permissions, mapped to the 14 BC modules + Dashboard + System.
    /// These correspond to the Phase7 UI route <c>meta.permission</c> values.
    /// </summary>
    private static readonly string[] AllWmsPermissions =
    {
        // BC-01 Warehouse
       "Wms.Warehouse", "Wms.Warehouse.Read", "Wms.Warehouse.Create", "Wms.Warehouse.Update", "Wms.Warehouse.Delete",
        // BC-02 Material
       "Wms.Material", "Wms.Material.Read", "Wms.Material.Create", "Wms.Material.Update", "Wms.Material.Delete",
        // BC-03 Inventory
        "Wms.Inventory","Wms.Inventory.Read", "Wms.Inventory.Adjust.Create", "Wms.Inventory.Freeze.Create", "Wms.Inventory.Snapshot",
        // BC-04 Inbound
       "Wms.Inbound", "Wms.Inbound.Read", "Wms.Inbound.Create", "Wms.Inbound.Update", "Wms.Inbound.Delete",
        // BC-05 Outbound
       "Wms.Outbound", "Wms.Outbound.Read", "Wms.Outbound.Create", "Wms.Outbound.Update", "Wms.Outbound.Delete",
        // BC-06 Transfer
        "Wms.Transfer","Wms.Transfer.Read", "Wms.Transfer.Create", "Wms.Transfer.Update", "Wms.Transfer.Delete",
        // BC-07 CycleCount
        "Wms.CycleCount","Wms.CycleCount.Read", "Wms.CycleCount.Execute", "Wms.CycleCount.Confirm",
        // BC-08 LineSide
        "Wms.LineSide","Wms.LineSide.Read", "Wms.LineSide.Replenish",
        // BC-09 Production
        "Wms.Production","Wms.Production.Read", "Wms.Production.Complete",
        // BC-10 TaskCenter
        "Wms.TaskCenter","Wms.TaskCenter.Read", "Wms.TaskCenter.Assign",
        // BC-11 BarcodeLabel
        "Wms.BarcodeLabel","Wms.BarcodeLabel.Read", "Wms.BarcodeLabel.Print",
        // BC-12 Workflow
        "Wms.Workflow","Wms.Workflow.Read", "Wms.Workflow.Approve",
        // BC-13 RuleEngine
        "Wms.RuleEngine","Wms.RuleEngine.Read", "Wms.RuleEngine.Execute",
        // BC-14 Notification
        "Wms.Notification","Wms.Notification.Read", "Wms.Notification.Create",
        // BC-15 Dashboard
        "Wms.Dashboard","Wms.Dashboard.Read",

        "Wms.DataDictionary.Dictionaries","Wms.DataDictionary.Dictionaries.Create","Wms.DataDictionary.Dictionaries.Update","Wms.DataDictionary.Dictionaries.Delete",
        "Wms.DataDictionary.Items","Wms.DataDictionary.Items.Create","Wms.DataDictionary.Items.Update","Wms.DataDictionary.Items.Delete",

        // ──── System / ABP Identity Module ────
        // Identity — Users (对应前端 /system/users)
        "AbpIdentity.Users",
        "AbpIdentity.Users.Create",
        "AbpIdentity.Users.Update",
        "AbpIdentity.Users.Delete",
        // Identity — Roles (对应前端 /system/roles)
        "AbpIdentity.Roles",
        "AbpIdentity.Roles.Create",
        "AbpIdentity.Roles.Update",
        "AbpIdentity.Roles.Delete",
        // Identity — Organization Units (对应前端 /system/organization)
        "AbpIdentity.OrganizationUnits",
        "AbpIdentity.OrganizationUnits.Create",
        "AbpIdentity.OrganizationUnits.Update",
        "AbpIdentity.OrganizationUnits.Delete",
        // Identity — Claim Types
        "AbpIdentity.ClaimTypes",

        // ──── ABP Permission Management (对应前端 /system/permissions) ────
        "AbpPermissionManagement",

        // ──── ABP Setting Management (对应前端 /system/settings) ────
        "AbpSettingManagement.Email",
        "AbpSettingManagement.Email.Send",

        // ──── ABP Feature Management ────
        "AbpFeatureManagement",

        // ──── ABP Audit Logging ────
        "AbpAuditLogging",

        // ──── ABP Localization ────
        "AbpLocalization",
        "AbpLocalization.Create",
        "AbpLocalization.Update",
        "AbpLocalization.Delete",
    };

    [HttpGet("permissions")]
    [Authorize]
    public async Task<IActionResult> GetPermissions()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = new { message = "Invalid token." } });
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Unauthorized(new { error = new { message = "User not found." } });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var permissionSet = new HashSet<string>();
        foreach (var roleName in roles)
        {
            var role = await _roleRepository.FindByNormalizedNameAsync(roleName.ToUpperInvariant());
            if (role == null) continue;

            permissionSet.Add(roleName);
        }

        if (roles.Contains("admin")|| roles.Contains("Admin"))
        {
            permissionSet.Add("Wms.All");
            foreach (var perm in AllWmsPermissions)
            {
                permissionSet.Add(perm);
            }
        }

        return Ok(permissionSet.ToArray());
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { error = new { message = "Invalid token." } });
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Unauthorized(new { error = new { message = "User not found." } });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, expiresIn) = GenerateJwtToken(user, roles);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = Guid.NewGuid().ToString("N"),
            ExpiresIn = expiresIn
        });
    }

    private (string token, int expiresInSeconds) GenerateJwtToken(
        IdentityUser user, IList<string> roles)
    {
        var jwtSecret = _configuration["Jwt:Secret"] ?? "Wms-Super-Secret-Key-2025-Must-Be-256-Bits!";
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? _configuration["App:SelfUrl"] ?? "https://localhost:5000";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "WmsApi";
        var expireHoursStr = _configuration["Jwt:ExpireHours"] ?? "8";

        if (!int.TryParse(expireHoursStr, out var expireHours))
        {
            expireHours = 8;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Admin role gets all permissions in token claims
        if (roles.Contains("admin") || roles.Contains("Admin"))
        {
            claims.Add(new Claim("permission", "Wms.All"));
            foreach (var perm in AllWmsPermissions)
            {
                claims.Add(new Claim("permission", perm));
            }
        }

        if (!string.IsNullOrEmpty(user.Name))
        {
            claims.Add(new Claim("given_name", user.Name));
        }
        if (!string.IsNullOrEmpty(user.Surname))
        {
            claims.Add(new Claim("family_name", user.Surname));
        }

        var expiresIn = expireHours * 3600;
        var expires = DateTime.UtcNow.AddHours(expireHours);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        // 清除 OutboundClaimTypeMap，确保 ClaimTypes.NameIdentifier、ClaimTypes.Role 等完整 URI claims
        // 在 JWT 中原样保留，避免被映射为 nameid/role 等短名称。
        // 配合 WmsWebHostModule 中 MapInboundClaims = false，ABP 才能正确识别这些 claims。
        tokenHandler.OutboundClaimTypeMap.Clear();
        return (tokenHandler.WriteToken(token), expiresIn);
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string UserNameOrEmailAddress { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}

public class UserProfile
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? TenantId { get; set; }
    public string[] Roles { get; set; } = Array.Empty<string>();
}