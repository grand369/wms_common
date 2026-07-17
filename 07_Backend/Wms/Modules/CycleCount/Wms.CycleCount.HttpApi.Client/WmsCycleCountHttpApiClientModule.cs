using Volo.Abp.Modularity;
using Wms.CycleCount.Application.Contracts;
namespace Wms.CycleCount.HttpApi.Client;
[DependsOn(typeof(WmsCycleCountApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsCycleCountHttpApiClientModule : AbpModule { }
