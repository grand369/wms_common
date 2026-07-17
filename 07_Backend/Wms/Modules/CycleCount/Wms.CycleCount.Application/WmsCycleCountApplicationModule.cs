using Volo.Abp.Modularity;
using Wms.CycleCount.Domain;
using Wms.CycleCount.Application.Contracts;
using Wms.Inventory.Application.Contracts;
using Wms.Warehouse.Application.Contracts;
using Wms.Workflow.Application.Contracts;
namespace Wms.CycleCount.Application;
[DependsOn(typeof(WmsCycleCountDomainModule), typeof(WmsCycleCountApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsInventoryApplicationContractsModule), typeof(WmsWarehouseApplicationContractsModule), typeof(WmsWorkflowApplicationContractsModule))]
public class WmsCycleCountApplicationModule : AbpModule { }
