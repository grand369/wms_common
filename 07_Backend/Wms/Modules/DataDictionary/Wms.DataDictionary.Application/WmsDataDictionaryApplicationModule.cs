using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Wms.DataDictionary.Application.Contracts;
using Wms.DataDictionary.Domain;

namespace Wms.DataDictionary.Application;

[DependsOn(typeof(WmsDataDictionaryDomainModule), typeof(WmsDataDictionaryApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpAutoMapperModule))]
public class WmsDataDictionaryApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WmsDataDictionaryApplicationModule>();
        });
    }
}
