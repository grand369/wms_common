using Volo.Abp.Modularity;
using Wms.Production.Domain;
using Wms.Production.Application.Contracts;
using Wms.Inbound.Application.Contracts;
using Wms.Outbound.Application.Contracts;
using Wms.Material.Application.Contracts;
namespace Wms.Production.Application;
[DependsOn(typeof(WmsProductionDomainModule), typeof(WmsProductionApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsInboundApplicationContractsModule), typeof(WmsOutboundApplicationContractsModule), typeof(WmsMaterialApplicationContractsModule))]
public class WmsProductionApplicationModule : AbpModule { }
