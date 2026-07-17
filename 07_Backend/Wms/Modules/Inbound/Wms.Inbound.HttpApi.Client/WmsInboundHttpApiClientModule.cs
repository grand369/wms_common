using Volo.Abp.Modularity;
using Wms.Inbound.Application.Contracts;
namespace Wms.Inbound.HttpApi.Client;
[DependsOn(typeof(WmsInboundApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsInboundHttpApiClientModule : AbpModule { }
