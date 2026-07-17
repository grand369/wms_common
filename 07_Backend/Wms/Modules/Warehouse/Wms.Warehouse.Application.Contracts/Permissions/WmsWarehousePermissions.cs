using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Warehouse.Application.Contracts.Permissions;

/// <summary>
/// Warehouse Module Permission Definitions — defines all permissions for Warehouse, Area, and Location CRUD operations.
/// (Phase 8 Coding Conventions, Section 1.4)
/// </summary>
public class WmsWarehousePermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.Warehouse";

    public static class Warehouses
    {
        public const string Default = GroupName + ".Warehouses";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string List = Default + ".List";
        public const string Activate = Default + ".Activate";
        public const string Deactivate = Default + ".Deactivate";
    }

    public static class Areas
    {
        public const string Default = GroupName + ".Areas";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string List = Default + ".List";
    }

    public static class Locations
    {
        public const string Default = GroupName + ".Locations";
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
        var warehouseGroup = context.AddGroup(GroupName, L("仓库管理"));

        var warehouses = warehouseGroup.AddPermission(Warehouses.Default, L("仓库管理"));
        warehouses.AddChild(Warehouses.Create, L("创建仓库"));
        warehouses.AddChild(Warehouses.Update, L("更新仓库"));
        warehouses.AddChild(Warehouses.Delete, L("删除仓库"));
        warehouses.AddChild(Warehouses.View, L("查看仓库"));
        warehouses.AddChild(Warehouses.List, L("仓库列表"));
        warehouses.AddChild(Warehouses.Activate, L("启用仓库"));
        warehouses.AddChild(Warehouses.Deactivate, L("停用仓库"));

        var areas = warehouseGroup.AddPermission(Areas.Default, L("库区管理"));
        areas.AddChild(Areas.Create, L("创建库区"));
        areas.AddChild(Areas.Update, L("更新库区"));
        areas.AddChild(Areas.Delete, L("删除库区"));
        areas.AddChild(Areas.View, L("查看库区"));
        areas.AddChild(Areas.List, L("库区列表"));

        var locations = warehouseGroup.AddPermission(Locations.Default, L("库位管理"));
        locations.AddChild(Locations.Create, L("创建库位"));
        locations.AddChild(Locations.Update, L("更新库位"));
        locations.AddChild(Locations.Delete, L("删除库位"));
        locations.AddChild(Locations.View, L("查看库位"));
        locations.AddChild(Locations.List, L("库位列表"));
        locations.AddChild(Locations.Activate, L("启用库位"));
        locations.AddChild(Locations.Deactivate, L("停用库位"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
