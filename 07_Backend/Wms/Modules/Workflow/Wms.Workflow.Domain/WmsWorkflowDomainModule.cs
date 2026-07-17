using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Workflow.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsWorkflowDomainModule : AbpModule { }
