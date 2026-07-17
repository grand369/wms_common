using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Production.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsProductionApplicationContractsModule : AbpModule { }
