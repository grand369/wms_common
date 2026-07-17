using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Material.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsMaterialDomainModule : AbpModule { }
