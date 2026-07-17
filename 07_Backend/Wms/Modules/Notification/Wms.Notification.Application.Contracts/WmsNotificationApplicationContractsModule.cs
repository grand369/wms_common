using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Notification.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsNotificationApplicationContractsModule : AbpModule { }
