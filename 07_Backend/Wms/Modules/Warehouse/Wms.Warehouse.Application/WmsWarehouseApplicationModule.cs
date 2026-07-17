using Volo.Abp.Application.Services;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;
using Wms.Warehouse.Domain;
using Wms.Warehouse.Application.Contracts;

namespace Wms.Warehouse.Application;

[DependsOn(
    typeof(WmsWarehouseDomainModule),
    typeof(WmsWarehouseApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpEventBusModule)
)]
public class WmsWarehouseApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Warehouse Application module configuration
    }
}
