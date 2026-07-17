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
        var group = context.AddGroup(WmsOutboundPermissions.GroupName, L("Outbound"));

        var order = group.AddPermission(WmsOutboundPermissions.Order.Read, L("Read Outbound Order"));
        order.AddChild(WmsOutboundPermissions.Order.Create, L("Create Outbound Order"));
        order.AddChild(WmsOutboundPermissions.Order.Update, L("Update Outbound Order"));
        order.AddChild(WmsOutboundPermissions.Order.Delete, L("Delete Outbound Order"));
        order.AddChild(WmsOutboundPermissions.Order.Allocate, L("Allocate Inventory"));
        order.AddChild(WmsOutboundPermissions.Order.Picking, L("Confirm Picking"));
        order.AddChild(WmsOutboundPermissions.Order.Shipping, L("Confirm Shipping"));
        order.AddChild(WmsOutboundPermissions.Order.Complete, L("Complete Outbound"));
        order.AddChild(WmsOutboundPermissions.Order.Cancel, L("Cancel Outbound"));
        order.AddChild(WmsOutboundPermissions.Order.ReleaseAllocation, L("Release Allocation"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
