using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Inventory.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsInventoryApplicationContractsModule : AbpModule { }
