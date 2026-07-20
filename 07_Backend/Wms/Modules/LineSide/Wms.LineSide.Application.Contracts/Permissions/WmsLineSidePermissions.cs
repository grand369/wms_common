using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.LineSide.Application.Contracts.Permissions;

/// <summary>PERM-LS: 5 permissions</summary>
public class WmsLineSidePermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.LineSide";
    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Update = GroupName + ".Update";
    public const string Replenish = GroupName + ".Replenish";
    public const string Backflush = GroupName + ".Backflush";

    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(GroupName, L("线边管理"));
        group.AddPermission(Read, L("线边管理"));
        group.AddPermission(Create, L("创建线边仓"));
        group.AddPermission(Update, L("更新线边仓"));
        group.AddPermission(Replenish, L("线边补货"));
        group.AddPermission(Backflush, L("线边倒扣"));
    }
    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
