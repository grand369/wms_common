using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Workflow.Application;
using Wms.Workflow.Application.Contracts;
namespace Wms.Workflow.HttpApi;
[DependsOn(typeof(WmsWorkflowApplicationModule), typeof(WmsWorkflowApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsWorkflowHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsWorkflowHttpApiModule).Assembly);
        });
    }
}
