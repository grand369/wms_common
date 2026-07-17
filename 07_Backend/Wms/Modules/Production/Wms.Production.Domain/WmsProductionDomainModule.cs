using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Production.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsProductionDomainModule : AbpModule { }
