using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.TaskCenter.Application;
using Wms.TaskCenter.Application.Contracts;
namespace Wms.TaskCenter.HttpApi;
[DependsOn(typeof(WmsTaskCenterApplicationModule), typeof(WmsTaskCenterApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsTaskCenterHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsTaskCenterHttpApiModule).Assembly);
        });
    }
}
