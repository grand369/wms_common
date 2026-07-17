using Volo.Abp.Modularity;
using Wms.Inbound.Domain;
using Wms.Inbound.Application.Contracts;
using Wms.Inventory.Application.Contracts;
using Wms.Warehouse.Application.Contracts;
using Wms.Material.Application.Contracts;
namespace Wms.Inbound.Application;
[DependsOn(typeof(WmsInboundDomainModule), typeof(WmsInboundApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsInventoryApplicationContractsModule), typeof(WmsWarehouseApplicationContractsModule), typeof(WmsMaterialApplicationContractsModule))]
public class WmsInboundApplicationModule : AbpModule { }
