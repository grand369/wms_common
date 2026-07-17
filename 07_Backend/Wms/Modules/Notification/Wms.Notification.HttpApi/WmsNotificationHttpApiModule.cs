using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc;
using Wms.Notification.Application;
using Wms.Notification.Application.Contracts;
namespace Wms.Notification.HttpApi;
[DependsOn(typeof(WmsNotificationApplicationModule), typeof(WmsNotificationApplicationContractsModule), typeof(AbpAspNetCoreMvcModule))]
public class WmsNotificationHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(WmsNotificationHttpApiModule).Assembly);
        });
    }
}
