using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.LineSide.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsLineSideApplicationContractsModule : AbpModule { }
