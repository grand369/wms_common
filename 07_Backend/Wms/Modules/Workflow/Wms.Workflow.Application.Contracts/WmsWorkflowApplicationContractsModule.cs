using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Workflow.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsWorkflowApplicationContractsModule : AbpModule { }
