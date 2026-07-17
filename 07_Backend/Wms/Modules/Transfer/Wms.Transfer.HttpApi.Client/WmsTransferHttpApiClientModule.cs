using Volo.Abp.Modularity;
using Wms.Transfer.Application.Contracts;
namespace Wms.Transfer.HttpApi.Client;
[DependsOn(typeof(WmsTransferApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsTransferHttpApiClientModule : AbpModule { }
