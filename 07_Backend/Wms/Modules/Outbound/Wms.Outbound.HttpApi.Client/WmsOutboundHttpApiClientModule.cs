using Volo.Abp.Modularity;
using Wms.Outbound.Application.Contracts;
namespace Wms.Outbound.HttpApi.Client;
[DependsOn(typeof(WmsOutboundApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsOutboundHttpApiClientModule : AbpModule { }
