using Volo.Abp.Modularity;
using Wms.Notification.Application.Contracts;
namespace Wms.Notification.HttpApi.Client;
[DependsOn(typeof(WmsNotificationApplicationContractsModule), typeof(AbpHttpClientModule))]
public class WmsNotificationHttpApiClientModule : AbpModule { }
