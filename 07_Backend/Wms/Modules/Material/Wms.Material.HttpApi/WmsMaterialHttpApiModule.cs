using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Wms.Material.Application;
using Wms.Material.Application.Contracts;
namespace Wms.Material.HttpApi;
[DependsOn(typeof(WmsMaterialApplicationModule), typeof(WmsMaterialApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsMaterialHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsMaterialHttpApiModule).Assembly);
        });
    }
}
