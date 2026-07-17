using Volo.Abp.Modularity;
using Wms.LineSide.Domain;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
namespace Wms.LineSide.EntityFrameworkCore;
[DependsOn(typeof(WmsLineSideDomainModule), typeof(AbpEntityFrameworkCoreModule), typeof(AbpEntityFrameworkCoreSqlServerModule))]
public class WmsLineSideEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WmsLineSideDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
