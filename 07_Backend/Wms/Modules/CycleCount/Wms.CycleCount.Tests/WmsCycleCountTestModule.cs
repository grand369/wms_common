using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.CycleCount.Domain;
using Wms.CycleCount.Application;
using Wms.CycleCount.Application.Contracts;
using Wms.TestBase;
namespace Wms.CycleCount.Tests;
[DependsOn(typeof(WmsCycleCountDomainModule), typeof(WmsCycleCountApplicationModule), typeof(WmsCycleCountApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsCycleCountTestModule : AbpModule { }
