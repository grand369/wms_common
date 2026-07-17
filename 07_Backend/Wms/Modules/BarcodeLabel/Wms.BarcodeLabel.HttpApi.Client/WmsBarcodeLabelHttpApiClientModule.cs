using Volo.Abp.Modularity;
using Wms.BarcodeLabel.Application.Contracts;
namespace Wms.BarcodeLabel.HttpApi.Client;
[DependsOn(typeof(WmsBarcodeLabelApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsBarcodeLabelHttpApiClientModule : AbpModule { }
