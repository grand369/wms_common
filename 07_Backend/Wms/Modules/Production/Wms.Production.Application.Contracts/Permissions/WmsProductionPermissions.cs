using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Production.Application.Contracts.Permissions;

/// <summary>PERM-PD: 3 permissions</summary>
public class WmsProductionPermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.Production";
    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Complete = GroupName + ".Complete";

    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(GroupName, L("生产管理"));
        group.AddPermission(Read, L("生产管理"));
        group.AddPermission(Create, L("创建生产工单"));
        group.AddPermission(Complete, L("完成生产"));
    }
    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
