using Volo.Abp.Modularity;
using Wms.Warehouse.Application.Contracts;

namespace Wms.Warehouse.HttpApi.Client;

[DependsOn(
    typeof(WmsWarehouseApplicationContractsModule),
    typeof(AbpHttpClientModule)
)]
public class WmsWarehouseHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Warehouse HttpApi.Client module — client proxy configuration
    }
}
