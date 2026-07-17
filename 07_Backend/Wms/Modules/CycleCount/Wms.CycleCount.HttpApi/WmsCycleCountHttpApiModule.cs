using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.CycleCount.Application;
using Wms.CycleCount.Application.Contracts;
namespace Wms.CycleCount.HttpApi;
[DependsOn(typeof(WmsCycleCountApplicationModule), typeof(WmsCycleCountApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsCycleCountHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsCycleCountHttpApiModule).Assembly);
        });
    }
}
