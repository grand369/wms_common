using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.RuleEngine.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsRuleEngineDomainModule : AbpModule { }
