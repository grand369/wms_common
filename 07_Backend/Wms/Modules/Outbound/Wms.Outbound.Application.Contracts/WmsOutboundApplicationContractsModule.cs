using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Outbound.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsOutboundApplicationContractsModule : AbpModule { }
