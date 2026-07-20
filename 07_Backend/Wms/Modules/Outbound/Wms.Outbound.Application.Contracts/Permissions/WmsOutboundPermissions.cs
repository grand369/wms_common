using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Outbound.Application.Contracts.Permissions;

/// <summary>
/// Wms Outbound Permissions — defines all permission names for the Outbound module.
/// (PERM-OB, Phase 6 API Design)
/// </summary>
public class WmsOutboundPermissions
{
    public const string GroupName = "Wms.Outbound";

    public static class Order
    {
        public const string Read = GroupName + ".Read";
        public const string Create = GroupName + ".Create";
        public const string Update = GroupName + ".Update";
        public const string Delete = GroupName + ".Delete";
        public const string Allocate = GroupName + ".Allocate";
        public const string Picking = GroupName + ".Picking";
        public const string Shipping = GroupName + ".Shipping";
        public const string Complete = GroupName + ".Complete";
        public const string Cancel = GroupName + ".Cancel";
        public const string ReleaseAllocation = GroupName + ".ReleaseAllocation";
    }
}

/// <summary>
/// Outbound Permission Definition Provider — registers Outbound module permissions with ABP.
/// </summary>
public class WmsOutboundPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(WmsOutboundPermissions.GroupName, L("出库管理"));

        var order = group.AddPermission(WmsOutboundPermissions.Order.Read, L("出库管理"));
        order.AddChild(WmsOutboundPermissions.Order.Create, L("创建出库单"));
        order.AddChild(WmsOutboundPermissions.Order.Update, L("更新出库单"));
        order.AddChild(WmsOutboundPermissions.Order.Delete, L("删除出库单"));
        order.AddChild(WmsOutboundPermissions.Order.Allocate, L("分配库存"));
        order.AddChild(WmsOutboundPermissions.Order.Picking, L("确认拣货"));
        order.AddChild(WmsOutboundPermissions.Order.Shipping, L("确认发货"));
        order.AddChild(WmsOutboundPermissions.Order.Complete, L("完成出库"));
        order.AddChild(WmsOutboundPermissions.Order.Cancel, L("取消出库"));
        order.AddChild(WmsOutboundPermissions.Order.ReleaseAllocation, L("释放分配"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
