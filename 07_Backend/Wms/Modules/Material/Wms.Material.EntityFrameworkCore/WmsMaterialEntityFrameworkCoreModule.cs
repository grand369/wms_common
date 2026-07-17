using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Wms.Material.Domain;
namespace Wms.Material.EntityFrameworkCore;
[DependsOn(typeof(WmsMaterialDomainModule), typeof(AbpEntityFrameworkCoreModule), typeof(AbpEntityFrameworkCoreSqlServerModule))]
public class WmsMaterialEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WmsMaterialDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
