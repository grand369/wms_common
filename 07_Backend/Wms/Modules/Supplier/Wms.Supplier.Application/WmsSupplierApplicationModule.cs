using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Wms.Supplier.Application.Contracts;
using Wms.Supplier.Application.Mappings;
using Wms.Supplier.Domain;

namespace Wms.Supplier.Application;

[DependsOn(typeof(WmsSupplierDomainModule), typeof(WmsSupplierApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpAutoMapperModule))]
public class WmsSupplierApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<SupplierAutoMapperProfile>();
        });
    }
}
