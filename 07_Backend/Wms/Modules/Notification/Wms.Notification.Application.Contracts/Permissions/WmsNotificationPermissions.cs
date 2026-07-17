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
        var group = context.AddGroup(WmsNotificationPermissions.GroupName, L("Notification"));

        group.AddPermission(WmsNotificationPermissions.Read, L("Read Notifications"));
        group.AddPermission(WmsNotificationPermissions.Create, L("Create Notifications/Templates/Rules"));
        group.AddPermission(WmsNotificationPermissions.Update, L("Update Notifications/Templates/Rules"));
        group.AddPermission(WmsNotificationPermissions.Delete, L("Delete Notifications/Templates/Rules"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
