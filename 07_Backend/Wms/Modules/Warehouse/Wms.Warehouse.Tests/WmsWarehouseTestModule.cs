using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Warehouse.Domain;
using Wms.Warehouse.Application;
using Wms.Warehouse.Application.Contracts;
using Wms.TestBase;

namespace Wms.Warehouse.Tests;

[DependsOn(
    typeof(WmsWarehouseDomainModule),
    typeof(WmsWarehouseApplicationModule),
    typeof(WmsWarehouseApplicationContractsModule),
    typeof(WmsTestBaseModule),
    typeof(AbpAutofacModule)
)]
public class WmsWarehouseTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Warehouse test module configuration
    }
}
