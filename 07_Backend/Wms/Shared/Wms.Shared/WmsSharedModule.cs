using Volo.Abp.Modularity;
using Volo.Abp.EventBus;

namespace Wms.Shared;

[DependsOn(
    typeof(AbpEventBusModule)
)]
public class WmsSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Shared kernel configuration — no business module dependencies
    }
}
