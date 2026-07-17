using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Outbound.Domain;
using Wms.Outbound.Application;
using Wms.Outbound.Application.Contracts;
using Wms.TestBase;
namespace Wms.Outbound.Tests;
[DependsOn(typeof(WmsOutboundDomainModule), typeof(WmsOutboundApplicationModule), typeof(WmsOutboundApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsOutboundTestModule : AbpModule { }
