using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Material.Domain;
using Wms.Material.Application;
using Wms.Material.Application.Contracts;
using Wms.TestBase;
namespace Wms.Material.Tests;
[DependsOn(typeof(WmsMaterialDomainModule), typeof(WmsMaterialApplicationModule), typeof(WmsMaterialApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsMaterialTestModule : AbpModule { }
