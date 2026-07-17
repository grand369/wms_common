using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Inventory.Domain;
using Wms.Inventory.Application;
using Wms.Inventory.Application.Contracts;
using Wms.TestBase;
namespace Wms.Inventory.Tests;
[DependsOn(typeof(WmsInventoryDomainModule), typeof(WmsInventoryApplicationModule), typeof(WmsInventoryApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsInventoryTestModule : AbpModule { }
