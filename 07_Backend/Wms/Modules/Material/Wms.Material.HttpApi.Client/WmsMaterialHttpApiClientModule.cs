using Volo.Abp.Modularity;
using Wms.Material.Application.Contracts;
namespace Wms.Material.HttpApi.Client;
[DependsOn(typeof(WmsMaterialApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsMaterialHttpApiClientModule : AbpModule { }
