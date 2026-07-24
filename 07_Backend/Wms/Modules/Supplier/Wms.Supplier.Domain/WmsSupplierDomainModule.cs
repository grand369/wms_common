using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Wms.Supplier.Domain;

[DependsOn(typeof(AbpDddDomainModule))]
public class WmsSupplierDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}
