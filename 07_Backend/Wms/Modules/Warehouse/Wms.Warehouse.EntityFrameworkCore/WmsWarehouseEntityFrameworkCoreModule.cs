using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Wms.Warehouse.Domain;

namespace Wms.Warehouse.EntityFrameworkCore;

[DependsOn(
    typeof(WmsWarehouseDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule)
)]
public class WmsWarehouseEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Warehouse EF Core module — DbContext configuration and repository registration
        context.Services.AddAbpDbContext<WmsWarehouseDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
