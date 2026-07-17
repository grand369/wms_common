using Volo.Abp.Modularity;
using Volo.Abp;
using Volo.Abp.Autofac;

namespace Wms.TestBase;

[DependsOn(
    typeof(AbpTestBaseModule),
    typeof(AbpAutofacModule)
)]
public class WmsTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Common test infrastructure configuration
        // All module test modules depend on this module
        // Shouldly is the unified assertion library (Phase 8 Coding Conventions Section 6)
        // Moq is the unified mocking framework
    }
}
