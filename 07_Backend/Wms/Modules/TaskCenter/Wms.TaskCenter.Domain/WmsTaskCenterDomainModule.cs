using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.TaskCenter.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsTaskCenterDomainModule : AbpModule { }
