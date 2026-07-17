using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Transfer.Application;
using Wms.Transfer.Application.Contracts;
namespace Wms.Transfer.HttpApi;
[DependsOn(typeof(WmsTransferApplicationModule), typeof(WmsTransferApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsTransferHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsTransferHttpApiModule).Assembly);
        });
    }
}
