using Volo.Abp.Modularity;
using Wms.Outbound.Domain;
using Wms.Outbound.Application.Contracts;
using Wms.Inventory.Application.Contracts;
using Wms.Material.Application.Contracts;
namespace Wms.Outbound.Application;
[DependsOn(typeof(WmsOutboundDomainModule), typeof(WmsOutboundApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsInventoryApplicationContractsModule), typeof(WmsMaterialApplicationContractsModule))]
public class WmsOutboundApplicationModule : AbpModule { }
