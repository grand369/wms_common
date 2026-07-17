using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace Wms.EntityFrameworkCore.Data;

public class WmsDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;

    public WmsDataSeedContributor(IdentityUserManager userManager, IdentityRoleManager roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateRolesAsync();
        await CreateAdminUserAsync();
    }

    private async Task CreateRolesAsync()
    {
        var roles = new[] { "Admin", "Manager", "Operator", "Viewer" };
        
        foreach (var roleName in roles)
        {
            if (await _roleManager.FindByNameAsync(roleName) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), roleName, null));
            }
        }
    }

    private async Task CreateAdminUserAsync()
    {
        const string adminUserName = "admin";
        const string adminEmail = "admin@wms.com";
        const string adminPassword = "Wms@2025";

        if (await _userManager.FindByNameAsync(adminUserName) == null)
        {
            var adminUser = new IdentityUser(Guid.NewGuid(), adminUserName, adminEmail, null)
            {
                Name = "System",
                Surname = "Administrator"
            };

            await _userManager.CreateAsync(adminUser, adminPassword);
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}