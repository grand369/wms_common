using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.TaskCenter.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsTaskCenterApplicationContractsModule : AbpModule { }
