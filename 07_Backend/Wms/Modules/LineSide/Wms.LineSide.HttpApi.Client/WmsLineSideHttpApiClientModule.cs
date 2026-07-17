using Volo.Abp.Modularity;
using Wms.LineSide.Application.Contracts;
namespace Wms.LineSide.HttpApi.Client;
[DependsOn(typeof(WmsLineSideApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsLineSideHttpApiClientModule : AbpModule { }
