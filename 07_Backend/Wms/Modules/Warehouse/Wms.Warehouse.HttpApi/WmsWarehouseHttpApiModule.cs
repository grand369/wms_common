using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Wms.Warehouse.Application;
using Wms.Warehouse.Application.Contracts;

namespace Wms.Warehouse.HttpApi;

[DependsOn(
    typeof(WmsWarehouseApplicationModule),
    typeof(WmsWarehouseApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
)]
public class WmsWarehouseHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsWarehouseHttpApiModule).Assembly);
        });
    }
}
