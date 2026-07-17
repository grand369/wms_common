using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Inbound.Application.Contracts.Permissions;

/// <summary>
/// Wms Inbound Permissions — defines all permission names for the Inbound module.
/// (PERM-IN, Phase 6 API Design)
/// </summary>
public class WmsInboundPermissions
{
    public const string GroupName = "Wms.Inbound";

    public static class Order
    {
        public const string Read = GroupName + ".Read";
        public const string Create = GroupName + ".Create";
        public const string Update = GroupName + ".Update";
        public const string Delete = GroupName + ".Delete";
        public const string Confirm = GroupName + ".Confirm";
        public const string QualityInspect = GroupName + ".QualityInspect";
        public const string Putaway = GroupName + ".Putaway";
        public const string Complete = GroupName + ".Complete";
        public const string Cancel = GroupName + ".Cancel";
    }
}

/// <summary>
/// Inbound Permission Definition Provider — registers Inbound module permissions with ABP.
/// </summary>
public class WmsInboundPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(WmsInboundPermissions.GroupName, L("Inbound"));

        var order = group.AddPermission(WmsInboundPermissions.Order.Read, L("Read Inbound Order"));
        order.AddChild(WmsInboundPermissions.Order.Create, L("Create Inbound Order"));
        order.AddChild(WmsInboundPermissions.Order.Update, L("Update Inbound Order"));
        order.AddChild(WmsInboundPermissions.Order.Delete, L("Delete Inbound Order"));
        order.AddChild(WmsInboundPermissions.Order.Confirm, L("Confirm Receipt"));
        order.AddChild(WmsInboundPermissions.Order.QualityInspect, L("Quality Inspect"));
        order.AddChild(WmsInboundPermissions.Order.Putaway, L("Putaway"));
        order.AddChild(WmsInboundPermissions.Order.Complete, L("Complete Inbound"));
        order.AddChild(WmsInboundPermissions.Order.Cancel, L("Cancel Inbound"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
