using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.RuleEngine.Domain;
using Wms.RuleEngine.Application;
using Wms.RuleEngine.Application.Contracts;
using Wms.TestBase;
namespace Wms.RuleEngine.Tests;
[DependsOn(typeof(WmsRuleEngineDomainModule), typeof(WmsRuleEngineApplicationModule), typeof(WmsRuleEngineApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsRuleEngineTestModule : AbpModule { }
