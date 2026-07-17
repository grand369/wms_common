using Volo.Abp.Modularity;
using Wms.Inventory.Domain;
using Wms.Inventory.Application.Contracts;
using Wms.Warehouse.Application.Contracts;
using Wms.Material.Application.Contracts;
namespace Wms.Inventory.Application;
[DependsOn(typeof(WmsInventoryDomainModule), typeof(WmsInventoryApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsWarehouseApplicationContractsModule), typeof(WmsMaterialApplicationContractsModule))]
public class WmsInventoryApplicationModule : AbpModule { }
