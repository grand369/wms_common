using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Wms.CycleCount.Domain;
using Wms.CycleCount.EntityFrameworkCore;
using Wms.Inbound.Domain;
using Wms.Inbound.EntityFrameworkCore;
using Wms.Inventory.Domain;
using Wms.Inventory.EntityFrameworkCore;
using Wms.Material.Domain;
using Wms.Material.EntityFrameworkCore;
using Wms.Outbound.Domain;
using Wms.Outbound.EntityFrameworkCore;
using Wms.Shared;
using Wms.TaskCenter.Domain;
using Wms.TaskCenter.EntityFrameworkCore;
using Wms.Transfer.Domain;
using Wms.Transfer.EntityFrameworkCore;
using Wms.Warehouse.Domain;
using Wms.Warehouse.EntityFrameworkCore;
using Wms.Workflow.Domain;
using Wms.Workflow.EntityFrameworkCore;

namespace Wms.IntegrationTests;

/// <summary>
/// Integration test module for cross-module flow testing.
///
/// Strategy: We reference EF Core projects for type access (DbContext classes,
/// entity configurations) but avoid depending on EF Core modules directly
/// (which would pull in AbpEntityFrameworkCoreSqlServerModule). Instead we
/// manually register each DbContext with Microsoft.EntityFrameworkCore.InMemory.
/// </summary>
[DependsOn(
    typeof(WmsSharedModule),
    // Domain modules — bring in aggregates, domain services, SmartEnums
    typeof(WmsWarehouseDomainModule),
    typeof(WmsMaterialDomainModule),
    typeof(WmsInventoryDomainModule),
    typeof(WmsInboundDomainModule),
    typeof(WmsOutboundDomainModule),
    typeof(WmsTransferDomainModule),
    typeof(WmsCycleCountDomainModule),
    typeof(WmsWorkflowDomainModule),
    typeof(WmsTaskCenterDomainModule),
    // ABP infrastructure
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAutofacModule)
)]
public class WmsIntegrationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;

        // Register each module's DbContext with InMemory provider
        RegisterDbContext<WmsWarehouseDbContext>(services);
        RegisterDbContext<WmsMaterialDbContext>(services);
        RegisterDbContext<WmsInventoryDbContext>(services);
        RegisterDbContext<WmsInboundDbContext>(services);
        RegisterDbContext<WmsOutboundDbContext>(services);
        RegisterDbContext<WmsTransferDbContext>(services);
        RegisterDbContext<WmsCycleCountDbContext>(services);
        RegisterDbContext<WmsWorkflowDbContext>(services);
        RegisterDbContext<WmsTaskCenterDbContext>(services);

        // Disable unit-of-work transactions for InMemory database
        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }

    private static void RegisterDbContext<TDbContext>(IServiceCollection services)
        where TDbContext : AbpDbContext<TDbContext>
    {
        services.AddAbpDbContext<TDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure<TDbContext>(ctxOptions =>
            {
                ctxOptions.DbContextOptions.UseInMemoryDatabase(
                    $"WmsIntegrationTest_{typeof(TDbContext).Name}");
            });
        });
    }
}
