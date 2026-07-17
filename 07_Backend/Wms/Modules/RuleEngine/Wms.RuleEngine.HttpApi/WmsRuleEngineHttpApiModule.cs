using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.RuleEngine.Application;
using Wms.RuleEngine.Application.Contracts;
namespace Wms.RuleEngine.HttpApi;
[DependsOn(typeof(WmsRuleEngineApplicationModule), typeof(WmsRuleEngineApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsRuleEngineHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsRuleEngineHttpApiModule).Assembly);
        });
    }
}
