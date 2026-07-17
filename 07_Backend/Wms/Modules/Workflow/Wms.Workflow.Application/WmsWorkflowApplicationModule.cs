using Volo.Abp.Modularity;
using Wms.Workflow.Domain;
using Wms.Workflow.Application.Contracts;
using Wms.Notification.Application.Contracts;
namespace Wms.Workflow.Application;
[DependsOn(typeof(WmsWorkflowDomainModule), typeof(WmsWorkflowApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsNotificationApplicationContractsModule))]
public class WmsWorkflowApplicationModule : AbpModule { }
