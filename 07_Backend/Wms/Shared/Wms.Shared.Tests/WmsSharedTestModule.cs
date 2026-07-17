using Volo.Abp.Modularity;
using Volo.Abp;
using Volo.Abp.Autofac;
using Wms.Shared;

namespace Wms.Shared.Tests;

[DependsOn(
    typeof(WmsSharedModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAutofacModule)
)]
public class WmsSharedTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Test-specific configuration for Shared Kernel
    }
}
