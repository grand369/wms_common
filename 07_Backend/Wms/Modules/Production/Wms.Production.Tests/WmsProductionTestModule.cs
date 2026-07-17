using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Production.Domain;
using Wms.Production.Application;
using Wms.Production.Application.Contracts;
using Wms.TestBase;
namespace Wms.Production.Tests;
[DependsOn(typeof(WmsProductionDomainModule), typeof(WmsProductionApplicationModule), typeof(WmsProductionApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsProductionTestModule : AbpModule { }
