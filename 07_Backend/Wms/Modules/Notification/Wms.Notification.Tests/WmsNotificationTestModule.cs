using Volo.Abp.Modularity;
using Volo.Abp.Autofac;
using Wms.Notification.Domain;
using Wms.Notification.Application;
using Wms.Notification.Application.Contracts;
using Wms.TestBase;
namespace Wms.Notification.Tests;
[DependsOn(typeof(WmsNotificationDomainModule), typeof(WmsNotificationApplicationModule), typeof(WmsNotificationApplicationContractsModule), typeof(WmsTestBaseModule), typeof(AbpAutofacModule))]
public class WmsNotificationTestModule : AbpModule { }
