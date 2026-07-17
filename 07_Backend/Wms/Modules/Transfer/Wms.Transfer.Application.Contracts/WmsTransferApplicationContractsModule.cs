using Volo.Abp.Modularity;
using Wms.Shared;
namespace Wms.Transfer.Application.Contracts;
[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsTransferApplicationContractsModule : AbpModule { }
