using Volo.Abp.Modularity;
using Wms.Inventory.Domain;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
namespace Wms.Inventory.EntityFrameworkCore;
[DependsOn(typeof(WmsInventoryDomainModule), typeof(AbpEntityFrameworkCoreModule), typeof(AbpEntityFrameworkCoreSqlServerModule))]
public class WmsInventoryEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WmsInventoryDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
