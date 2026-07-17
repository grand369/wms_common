using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.CycleCount.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsCycleCountApplicationContractsModule : AbpModule { }
