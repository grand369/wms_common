using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Wms.Supplier.Application;

namespace Wms.Supplier.EntityFrameworkCore;

[DependsOn(typeof(WmsSupplierApplicationModule), typeof(AbpEntityFrameworkCoreModule))]
public class WmsSupplierEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WmsSupplierDbContext>(options =>
        {
            options.AddDefaultRepositories(true);
        });
    }
}
