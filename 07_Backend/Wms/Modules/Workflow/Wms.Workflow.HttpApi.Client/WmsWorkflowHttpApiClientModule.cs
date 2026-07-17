using Volo.Abp.Modularity;
using Wms.Workflow.Application.Contracts;
namespace Wms.Workflow.HttpApi.Client;
[DependsOn(typeof(WmsWorkflowApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsWorkflowHttpApiClientModule : AbpModule { }
