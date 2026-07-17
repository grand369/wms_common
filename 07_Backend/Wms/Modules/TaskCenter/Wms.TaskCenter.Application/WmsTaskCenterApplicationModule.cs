using Volo.Abp.Modularity;
using Wms.TaskCenter.Domain;
using Wms.TaskCenter.Application.Contracts;
namespace Wms.TaskCenter.Application;
[DependsOn(typeof(WmsTaskCenterDomainModule), typeof(WmsTaskCenterApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule))]
public class WmsTaskCenterApplicationModule : AbpModule { }
