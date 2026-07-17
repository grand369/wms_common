using Volo.Abp.Modularity;
using Wms.Inbound.Domain;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
namespace Wms.Inbound.EntityFrameworkCore;
[DependsOn(typeof(WmsInboundDomainModule), typeof(AbpEntityFrameworkCoreModule), typeof(AbpEntityFrameworkCoreSqlServerModule))]
public class WmsInboundEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WmsInboundDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
