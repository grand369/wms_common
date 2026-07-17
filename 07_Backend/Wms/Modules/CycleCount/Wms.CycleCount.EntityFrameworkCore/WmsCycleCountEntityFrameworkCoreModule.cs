using Volo.Abp.Modularity;
using Wms.CycleCount.Domain;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
namespace Wms.CycleCount.EntityFrameworkCore;
[DependsOn(typeof(WmsCycleCountDomainModule), typeof(AbpEntityFrameworkCoreModule), typeof(AbpEntityFrameworkCoreSqlServerModule))]
public class WmsCycleCountEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WmsCycleCountDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
