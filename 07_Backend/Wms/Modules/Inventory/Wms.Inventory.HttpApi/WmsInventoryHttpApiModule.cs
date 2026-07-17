using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Inventory.Application;
using Wms.Inventory.Application.Contracts;
namespace Wms.Inventory.HttpApi;
[DependsOn(typeof(WmsInventoryApplicationModule), typeof(WmsInventoryApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsInventoryHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsInventoryHttpApiModule).Assembly);
        });
    }
}
