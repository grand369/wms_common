using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.BarcodeLabel.Application;
using Wms.BarcodeLabel.Application.Contracts;
namespace Wms.BarcodeLabel.HttpApi;
[DependsOn(typeof(WmsBarcodeLabelApplicationModule), typeof(WmsBarcodeLabelApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsBarcodeLabelHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsBarcodeLabelHttpApiModule).Assembly);
        });
    }
}
