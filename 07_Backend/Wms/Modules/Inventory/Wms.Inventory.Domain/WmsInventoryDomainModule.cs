using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Inventory.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsInventoryDomainModule : AbpModule { }
