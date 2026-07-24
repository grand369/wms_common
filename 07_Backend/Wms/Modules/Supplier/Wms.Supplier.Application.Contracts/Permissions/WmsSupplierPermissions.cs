using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Supplier.Application.Contracts.Permissions;

/// <summary>
/// Supplier Module Permission Definitions.
/// </summary>
public class WmsSupplierPermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.Supplier";

    public static class Suppliers
    {
        public const string Default = GroupName + ".Suppliers";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string List = Default + ".List";
        public const string Activate = Default + ".Activate";
        public const string Deactivate = Default + ".Deactivate";
    }

    public override void Define(IPermissionDefinitionContext context)
    {
        var supplierGroup = context.AddGroup(GroupName, L("供应商管理"));

        var suppliers = supplierGroup.AddPermission(Suppliers.Default, L("供应商管理"));
        suppliers.AddChild(Suppliers.Create, L("创建供应商"));
        suppliers.AddChild(Suppliers.Update, L("更新供应商"));
        suppliers.AddChild(Suppliers.Delete, L("删除供应商"));
        suppliers.AddChild(Suppliers.View, L("查看供应商"));
        suppliers.AddChild(Suppliers.List, L("供应商列表"));
        suppliers.AddChild(Suppliers.Activate, L("启用供应商"));
        suppliers.AddChild(Suppliers.Deactivate, L("停用供应商"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
