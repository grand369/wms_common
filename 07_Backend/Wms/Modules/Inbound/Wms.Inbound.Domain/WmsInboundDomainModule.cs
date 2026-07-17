using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Inbound.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsInboundDomainModule : AbpModule { }
