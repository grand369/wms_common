using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Inbound.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsInboundApplicationContractsModule : AbpModule { }
