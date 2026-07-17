using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Workflow.Domain;
using Wms.Workflow.Application;
using Wms.Workflow.Application.Contracts;
using Wms.TestBase;
namespace Wms.Workflow.Tests;
[DependsOn(typeof(WmsWorkflowDomainModule), typeof(WmsWorkflowApplicationModule), typeof(WmsWorkflowApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsWorkflowTestModule : AbpModule { }
