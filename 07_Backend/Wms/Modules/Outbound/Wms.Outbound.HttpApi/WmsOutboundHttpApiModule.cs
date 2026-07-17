using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Outbound.Application;
using Wms.Outbound.Application.Contracts;
namespace Wms.Outbound.HttpApi;
[DependsOn(typeof(WmsOutboundApplicationModule), typeof(WmsOutboundApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsOutboundHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsOutboundHttpApiModule).Assembly);
        });
    }
}
