using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.CycleCount.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsCycleCountDomainModule : AbpModule { }
