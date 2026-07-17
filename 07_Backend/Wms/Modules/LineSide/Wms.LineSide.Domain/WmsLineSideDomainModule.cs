using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.LineSide.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsLineSideDomainModule : AbpModule { }
