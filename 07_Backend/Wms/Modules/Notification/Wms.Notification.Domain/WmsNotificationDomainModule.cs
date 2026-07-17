using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Notification.Domain;
[DependsOn(typeof(WmsSharedModule))]
public class WmsNotificationDomainModule : AbpModule { }
