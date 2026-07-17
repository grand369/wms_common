using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace Wms.TestBase;

/// <summary>
/// WmsDomainTestBase — unified base class for Domain unit tests.
/// Inherits AbpIntegratedTest&lt;TModule&gt; to provide ABP DI container access.
/// All Domain test classes should inherit this base for consistent test infrastructure.
/// (Phase 8 Coding Conventions Section 6, Phase 10 standardization)
/// </summary>
/// <typeparam name="TModule">The ABP module to boot for this test suite</typeparam>
public class WmsDomainTestBase<TModule> : AbpIntegratedTest<TModule>
    where TModule : IAbpModule
{
    /// <summary>
    /// Get a service from the DI container — shorthand for ServiceProvider.GetService.
    /// </summary>
    protected T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();
}
