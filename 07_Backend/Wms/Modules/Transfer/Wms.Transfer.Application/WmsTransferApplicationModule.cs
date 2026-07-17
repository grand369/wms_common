using Volo.Abp.Modularity;
using Wms.Transfer.Domain;
using Wms.Transfer.Application.Contracts;
using Wms.Inventory.Application.Contracts;
using Wms.Warehouse.Application.Contracts;
using Wms.Workflow.Application.Contracts;
namespace Wms.Transfer.Application;
[DependsOn(typeof(WmsTransferDomainModule), typeof(WmsTransferApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsInventoryApplicationContractsModule), typeof(WmsWarehouseApplicationContractsModule), typeof(WmsWorkflowApplicationContractsModule))]
public class WmsTransferApplicationModule : AbpModule { }
