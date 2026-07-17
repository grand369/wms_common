using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.LineSide.Domain;
using Wms.LineSide.Application;
using Wms.LineSide.Application.Contracts;
using Wms.TestBase;
namespace Wms.LineSide.Tests;
[DependsOn(typeof(WmsLineSideDomainModule), typeof(WmsLineSideApplicationModule), typeof(WmsLineSideApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsLineSideTestModule : AbpModule { }
