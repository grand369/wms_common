using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Notification.Application.Contracts.Permissions;

/// <summary>
/// PERM-NT: Wms Notification Permissions
/// </summary>
public class WmsNotificationPermissions
{
    public const string GroupName = "Wms.Notification";

    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Update = GroupName + ".Update";
    public const string Delete = GroupName + ".Delete";
}

/// <summary>
/// Permission Definition Provider for Notification module.
/// </summary>
public class WmsNotificationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(WmsNotificationPermissions.GroupName, L("通知管理"));

        group.AddPermission(WmsNotificationPermissions.Read, L("通知管理"));
        group.AddPermission(WmsNotificationPermissions.Create, L("创建通知/模板/规则"));
        group.AddPermission(WmsNotificationPermissions.Update, L("更新通知/模板/规则"));
        group.AddPermission(WmsNotificationPermissions.Delete, L("删除通知/模板/规则"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
