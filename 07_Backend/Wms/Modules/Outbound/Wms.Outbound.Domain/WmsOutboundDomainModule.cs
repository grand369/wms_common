using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Outbound.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsOutboundDomainModule : AbpModule { }
