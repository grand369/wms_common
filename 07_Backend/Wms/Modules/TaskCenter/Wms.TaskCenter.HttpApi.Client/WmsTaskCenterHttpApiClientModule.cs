using Volo.Abp.Modularity;
using Wms.TaskCenter.Application.Contracts;
namespace Wms.TaskCenter.HttpApi.Client;
[DependsOn(typeof(WmsTaskCenterApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsTaskCenterHttpApiClientModule : AbpModule { }
