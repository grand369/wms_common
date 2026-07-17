using Volo.Abp.Modularity;
using Wms.Inventory.Application.Contracts;
namespace Wms.Inventory.HttpApi.Client;
[DependsOn(typeof(WmsInventoryApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsInventoryHttpApiClientModule : AbpModule { }
