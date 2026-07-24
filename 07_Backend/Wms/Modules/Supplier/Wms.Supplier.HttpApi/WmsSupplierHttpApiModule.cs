using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Wms.Supplier.Application;
using Wms.Supplier.Application.Contracts;
using Wms.Supplier.EntityFrameworkCore;

namespace Wms.Supplier.HttpApi;

[DependsOn(typeof(WmsSupplierApplicationModule), typeof(WmsSupplierApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]

public class WmsSupplierHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context) 
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsSupplierHttpApiModule).Assembly);
        });
    }
}
