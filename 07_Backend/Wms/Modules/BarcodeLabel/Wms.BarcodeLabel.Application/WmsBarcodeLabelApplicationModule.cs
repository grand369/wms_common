using Volo.Abp.Modularity;
using Wms.BarcodeLabel.Domain;
using Wms.BarcodeLabel.Application.Contracts;
using Wms.Warehouse.Application.Contracts;
using Wms.Material.Application.Contracts;
namespace Wms.BarcodeLabel.Application;
[DependsOn(typeof(WmsBarcodeLabelDomainModule), typeof(WmsBarcodeLabelApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule), typeof(WmsWarehouseApplicationContractsModule), typeof(WmsMaterialApplicationContractsModule))]
public class WmsBarcodeLabelApplicationModule : AbpModule { }
