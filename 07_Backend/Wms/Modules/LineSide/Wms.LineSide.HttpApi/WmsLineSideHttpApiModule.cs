using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.LineSide.Application;
using Wms.LineSide.Application.Contracts;
namespace Wms.LineSide.HttpApi;
[DependsOn(typeof(WmsLineSideApplicationModule), typeof(WmsLineSideApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsLineSideHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsLineSideHttpApiModule).Assembly);
        });
    }
}
