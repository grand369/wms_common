using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Transfer.Domain;
using Wms.Transfer.Application;
using Wms.Transfer.Application.Contracts;
using Wms.TestBase;
namespace Wms.Transfer.Tests;
[DependsOn(typeof(WmsTransferDomainModule), typeof(WmsTransferApplicationModule), typeof(WmsTransferApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsTransferTestModule : AbpModule { }
