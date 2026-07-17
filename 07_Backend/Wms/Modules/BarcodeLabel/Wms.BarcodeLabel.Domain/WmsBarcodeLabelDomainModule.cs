using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.BarcodeLabel.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsBarcodeLabelDomainModule : AbpModule { }
