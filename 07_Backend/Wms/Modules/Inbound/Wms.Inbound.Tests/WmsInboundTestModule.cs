using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Inbound.Domain;
using Wms.Inbound.Application;
using Wms.Inbound.Application.Contracts;
using Wms.TestBase;
namespace Wms.Inbound.Tests;
[DependsOn(typeof(WmsInboundDomainModule), typeof(WmsInboundApplicationModule), typeof(WmsInboundApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsInboundTestModule : AbpModule { }
