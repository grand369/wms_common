using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;

namespace Wms.EntityFrameworkCore.SqlServer;

[DependsOn(
    typeof(WmsEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule)
)]
public class WmsEntityFrameworkCoreSqlServerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer(builder =>
            {
                builder.MigrationsAssembly(typeof(WmsEntityFrameworkCoreSqlServerModule).Assembly.GetName().Name);
            });
        });
    }
}