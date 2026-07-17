using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.RuleEngine.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsRuleEngineApplicationContractsModule : AbpModule { }
