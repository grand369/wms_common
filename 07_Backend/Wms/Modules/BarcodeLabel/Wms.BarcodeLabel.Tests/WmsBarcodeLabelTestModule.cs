using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.BarcodeLabel.Domain;
using Wms.BarcodeLabel.Application;
using Wms.BarcodeLabel.Application.Contracts;
using Wms.TestBase;
namespace Wms.BarcodeLabel.Tests;
[DependsOn(typeof(WmsBarcodeLabelDomainModule), typeof(WmsBarcodeLabelApplicationModule), typeof(WmsBarcodeLabelApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsBarcodeLabelTestModule : AbpModule { }
