using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.TaskCenter.Domain;
using Wms.TaskCenter.Application;
using Wms.TaskCenter.Application.Contracts;
using Wms.TestBase;
namespace Wms.TaskCenter.Tests;
[DependsOn(typeof(WmsTaskCenterDomainModule), typeof(WmsTaskCenterApplicationModule), typeof(WmsTaskCenterApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsTaskCenterTestModule : AbpModule { }
