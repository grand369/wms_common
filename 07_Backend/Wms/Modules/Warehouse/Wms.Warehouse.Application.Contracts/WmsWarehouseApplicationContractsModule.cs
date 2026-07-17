using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Wms.Shared;

namespace Wms.Warehouse.Application.Contracts;

[DependsOn(
    typeof(WmsSharedModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class WmsWarehouseApplicationContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Warehouse Contracts module — DTO and interface definitions
    }
}
