using Volo.Abp.Modularity;
using Wms.Production.Application.Contracts;
namespace Wms.Production.HttpApi.Client;
[DependsOn(typeof(WmsProductionApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsProductionHttpApiClientModule : AbpModule { }
