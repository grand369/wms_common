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
        var group = context.AddGroup(GroupName, L("Permission:LineSide"));
        group.AddPermission(Read, L("Permission:LineSide.Read"));
        group.AddPermission(Create, L("Permission:LineSide.Create"));
        group.AddPermission(Update, L("Permission:LineSide.Update"));
        group.AddPermission(Replenish, L("Permission:LineSide.Replenish"));
        group.AddPermission(Backflush, L("Permission:LineSide.Backflush"));
    }
    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
