using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Wms.RuleEngine.Domain;
using Wms.RuleEngine.Application.Contracts;
namespace Wms.RuleEngine.Application;
[DependsOn(
    typeof(WmsRuleEngineDomainModule),
    typeof(WmsRuleEngineApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpEventBusModule),
    typeof(AbpAutoMapperModule)
)]
public class WmsRuleEngineApplicationModule : AbpModule { }
