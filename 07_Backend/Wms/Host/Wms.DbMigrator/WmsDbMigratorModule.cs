using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Wms.EntityFrameworkCore;
using Wms.EntityFrameworkCore.SqlServer;

namespace Wms.DbMigrator;

[DependsOn(
    typeof(WmsEntityFrameworkCoreModule),
    typeof(WmsEntityFrameworkCoreSqlServerModule),
    typeof(AbpAutofacModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule)
)]
public class WmsDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer();
        });
    }
}