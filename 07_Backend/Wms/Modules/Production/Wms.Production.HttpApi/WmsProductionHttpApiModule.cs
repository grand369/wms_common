using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Production.Application;
using Wms.Production.Application.Contracts;
namespace Wms.Production.HttpApi;
[DependsOn(typeof(WmsProductionApplicationModule), typeof(WmsProductionApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsProductionHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsProductionHttpApiModule).Assembly);
        });
    }
}
