using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Wms.Shared;
using Wms.Warehouse.EntityFrameworkCore;
using Wms.Material.EntityFrameworkCore;
using Wms.Inventory.EntityFrameworkCore;
using Wms.Inbound.EntityFrameworkCore;
using Wms.Outbound.EntityFrameworkCore;
using Wms.BarcodeLabel.EntityFrameworkCore;
using Wms.LineSide.EntityFrameworkCore;
using Wms.RuleEngine.EntityFrameworkCore;
using Wms.Transfer.EntityFrameworkCore;
using Wms.Production.EntityFrameworkCore;
using Wms.CycleCount.EntityFrameworkCore;
using Wms.Workflow.EntityFrameworkCore;
using Wms.TaskCenter.EntityFrameworkCore;
using Wms.Notification.EntityFrameworkCore;

namespace Wms.EntityFrameworkCore;

[DependsOn(
    typeof(WmsSharedModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(WmsWarehouseEntityFrameworkCoreModule),
    typeof(WmsMaterialEntityFrameworkCoreModule),
    typeof(WmsInventoryEntityFrameworkCoreModule),
    typeof(WmsInboundEntityFrameworkCoreModule),
    typeof(WmsOutboundEntityFrameworkCoreModule),
    typeof(WmsBarcodeLabelEntityFrameworkCoreModule),
    typeof(WmsLineSideEntityFrameworkCoreModule),
    typeof(WmsRuleEngineEntityFrameworkCoreModule),
    typeof(WmsTransferEntityFrameworkCoreModule),
    typeof(WmsProductionEntityFrameworkCoreModule),
    typeof(WmsCycleCountEntityFrameworkCoreModule),
    typeof(WmsWorkflowEntityFrameworkCoreModule),
    typeof(WmsTaskCenterEntityFrameworkCoreModule),
    typeof(WmsNotificationEntityFrameworkCoreModule)
)]
public class WmsEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WmsDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}