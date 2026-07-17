using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Transfer.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsTransferDomainModule : AbpModule { }
