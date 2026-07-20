using Volo.Abp.EntityFrameworkCore;

namespace Wms.DataDictionary.EntityFrameworkCore;

[DependsOn(typeof(AbpEntityFrameworkCoreModule))]
public class WmsDataDictionaryEntityFrameworkCoreModule : AbpModule
{
}
