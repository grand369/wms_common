using Volo.Abp.Application.Services;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;
using Wms.Material.Domain;
using Wms.Material.Application.Contracts;
namespace Wms.Material.Application;
[DependsOn(typeof(WmsMaterialDomainModule), typeof(WmsMaterialApplicationContractsModule), typeof(AbpDddApplicationModule), typeof(AbpEventBusModule))]
public class WmsMaterialApplicationModule : AbpModule { }
