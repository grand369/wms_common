using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.BarcodeLabel.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsBarcodeLabelApplicationContractsModule : AbpModule { }
