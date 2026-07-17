using Volo.Abp.Modularity;
using Wms.RuleEngine.Application.Contracts;
namespace Wms.RuleEngine.HttpApi.Client;
[DependsOn(typeof(WmsRuleEngineApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsRuleEngineHttpApiClientModule : AbpModule { }
