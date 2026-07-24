using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Wms.Shared;

namespace Wms.Supplier.Application.Contracts;

[DependsOn(
    typeof(WmsSharedModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class WmsSupplierApplicationContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Supplier Contracts module — DTO and interface definitions
    }
}
