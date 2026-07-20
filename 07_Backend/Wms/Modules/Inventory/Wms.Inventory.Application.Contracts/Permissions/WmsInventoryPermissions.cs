using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Inventory.Application.Contracts.Permissions;

/// <summary>
/// Wms Inventory Permissions — defines all permission names for the Inventory module.
/// Each permission maps to a specific operation within the module.
/// </summary>
public class WmsInventoryPermissions
{
    public const string GroupName = "Wms.Inventory";

    // Balance permissions
    public static class Balance
    {
        public const string Read = GroupName + ".Read";
        public const string Initialize = GroupName + ".Initialize";
        public const string Snapshot = GroupName + ".Snapshot";
    }

    // Adjustment permissions
    public static class Adjust
    {
        public const string Create = GroupName + ".Adjust.Create";
        public const string Submit = GroupName + ".Adjust.Submit";
        public const string Approve = GroupName + ".Adjust.Approve";
        public const string Execute = GroupName + ".Adjust.Execute";
    }

    // Freeze permissions
    public static class Freeze
    {
        public const string Create = GroupName + ".Freeze.Create";
        public const string Approve = GroupName + ".Freeze.Approve";
        public const string Release = GroupName + ".Freeze.Release";
        public const string Cancel = GroupName + ".Freeze.Cancel";
    }

    // Alert permissions
    public static class Alert
    {
        public const string Resolve = GroupName + ".Alert.Resolve";
        public const string Scan = GroupName + ".Alert.Scan";
    }
}

/// <summary>
/// Permission Definition Provider — registers Inventory module permissions with ABP.
/// </summary>
public class WmsInventoryPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(WmsInventoryPermissions.GroupName, L("库存管理"));

        var balance = group.AddPermission(WmsInventoryPermissions.Balance.Read, L("库存余额"));
        balance.AddChild(WmsInventoryPermissions.Balance.Initialize, L("初始化库存"));
        balance.AddChild(WmsInventoryPermissions.Balance.Snapshot, L("库存快照"));

        var adjust = group.AddPermission(WmsInventoryPermissions.Adjust.Create, L("创建调整单"));
        adjust.AddChild(WmsInventoryPermissions.Adjust.Submit, L("提交调整单"));
        adjust.AddChild(WmsInventoryPermissions.Adjust.Approve, L("审批调整单"));
        adjust.AddChild(WmsInventoryPermissions.Adjust.Execute, L("执行调整"));

        var freeze = group.AddPermission(WmsInventoryPermissions.Freeze.Create, L("创建冻结单"));
        freeze.AddChild(WmsInventoryPermissions.Freeze.Approve, L("审批冻结"));
        freeze.AddChild(WmsInventoryPermissions.Freeze.Release, L("释放冻结"));
        freeze.AddChild(WmsInventoryPermissions.Freeze.Cancel, L("取消冻结"));

        var alert = group.AddPermission(WmsInventoryPermissions.Alert.Resolve, L("处理预警"));
        alert.AddChild(WmsInventoryPermissions.Alert.Scan, L("扫描预警"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
