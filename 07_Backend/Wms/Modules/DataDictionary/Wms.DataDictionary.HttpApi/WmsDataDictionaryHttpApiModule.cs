using Volo.Abp.AspNetCore.Mvc;
using Wms.DataDictionary.Application;
using Wms.DataDictionary.Application.Contracts;

namespace Wms.DataDictionary.HttpApi;

[DependsOn(typeof(WmsDataDictionaryApplicationModule), typeof(WmsDataDictionaryApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsDataDictionaryHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsDataDictionaryHttpApiModule).Assembly);
        });
    }
}
