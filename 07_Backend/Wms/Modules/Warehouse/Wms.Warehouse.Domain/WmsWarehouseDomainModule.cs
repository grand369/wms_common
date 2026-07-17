using Volo.Abp.Modularity;
using Wms.Shared;

namespace Wms.Warehouse.Domain;

[DependsOn(
    typeof(WmsSharedModule)
)]
public class WmsWarehouseDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Warehouse Domain module configuration
    }
}
