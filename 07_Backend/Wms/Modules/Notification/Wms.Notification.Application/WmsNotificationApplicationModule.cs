using Volo.Abp.Modularity;
using Wms.Notification.Domain;
using Wms.Notification.Application.Contracts;
namespace Wms.Notification.Application;
[DependsOn(typeof(WmsNotificationDomainModule), typeof(WmsNotificationApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule))]
public class WmsNotificationApplicationModule : AbpModule { }
