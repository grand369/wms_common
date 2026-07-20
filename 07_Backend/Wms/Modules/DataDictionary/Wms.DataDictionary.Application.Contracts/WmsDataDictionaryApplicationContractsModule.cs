using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;
using Wms.Shared;

namespace Wms.DataDictionary.Application.Contracts;

[DependsOn(typeof(WmsSharedModule), typeof(AbpAuthorizationModule), typeof(AbpDddApplicationContractsModule))]
public class WmsDataDictionaryApplicationContractsModule : AbpModule { }
