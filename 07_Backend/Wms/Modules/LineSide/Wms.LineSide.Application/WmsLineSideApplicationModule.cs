using Volo.Abp.Modularity;
using Wms.LineSide.Domain;
using Wms.LineSide.Application.Contracts;
using Wms.Inventory.Application.Contracts;
using Wms.Outbound.Application.Contracts;
namespace Wms.LineSide.Application;
[DependsOn(typeof(WmsLineSideDomainModule), typeof(WmsLineSideApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsInventoryApplicationContractsModule), typeof(WmsOutboundApplicationContractsModule))]
public class WmsLineSideApplicationModule : AbpModule { }
