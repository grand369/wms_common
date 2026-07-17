using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Inbound.Application;
using Wms.Inbound.Application.Contracts;
namespace Wms.Inbound.HttpApi;
[DependsOn(typeof(WmsInboundApplicationModule), typeof(WmsInboundApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsInboundHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsInboundHttpApiModule).Assembly);
        });
    }
}
