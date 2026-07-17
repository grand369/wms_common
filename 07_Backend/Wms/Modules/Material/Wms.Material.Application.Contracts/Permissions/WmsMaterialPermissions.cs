using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Material.Application.Contracts.Permissions;

/// <summary>
/// Material Module Permission Definitions — defines all permissions for Material, Classification, UnitOfMeasure, and Substitute operations.
/// (Phase 8 Coding Conventions, Section 1.4)
/// </summary>
public class WmsMaterialPermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.Material";

    public static class Materials
    {
        public const string Default = GroupName + ".Materials";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string List = Default + ".List";
        public const string Activate = Default + ".Activate";
        public const string Deactivate = Default + ".Deactivate";
    }

    public static class Classifications
    {
        public const string Default = GroupName + ".Classifications";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string List = Default + ".List";
        public const string ManageTree = Default + ".ManageTree";
    }

    public static class Units
    {
        public const string Default = GroupName + ".Units";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string List = Default + ".List";
    }

    public static class Substitutes
    {
        public const string Default = GroupName + ".Substitutes";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
    }

    public override void Define(IPermissionDefinitionContext context)
    {
        var materialGroup = context.AddGroup(GroupName, L("物料管理"));

        var materials = materialGroup.AddPermission(Materials.Default, L("物料管理"));
        materials.AddChild(Materials.Create, L("创建物料"));
        materials.AddChild(Materials.Update, L("更新物料"));
        materials.AddChild(Materials.Delete, L("删除物料"));
        materials.AddChild(Materials.View, L("查看物料"));
        materials.AddChild(Materials.List, L("物料列表"));
        materials.AddChild(Materials.Activate, L("启用物料"));
        materials.AddChild(Materials.Deactivate, L("停用物料"));

        var classifications = materialGroup.AddPermission(Classifications.Default, L("物料分类"));
        classifications.AddChild(Classifications.Create, L("创建分类"));
        classifications.AddChild(Classifications.Update, L("更新分类"));
        classifications.AddChild(Classifications.Delete, L("删除分类"));
        classifications.AddChild(Classifications.View, L("查看分类"));
        classifications.AddChild(Classifications.List, L("分类列表"));
        classifications.AddChild(Classifications.ManageTree, L("管理分类树"));

        var units = materialGroup.AddPermission(Units.Default, L("计量单位"));
        units.AddChild(Units.Create, L("创建单位"));
        units.AddChild(Units.Update, L("更新单位"));
        units.AddChild(Units.Delete, L("删除单位"));
        units.AddChild(Units.View, L("查看单位"));
        units.AddChild(Units.List, L("单位列表"));

        var substitutes = materialGroup.AddPermission(Substitutes.Default, L("替代料"));
        substitutes.AddChild(Substitutes.Create, L("添加替代料"));
        substitutes.AddChild(Substitutes.Delete, L("删除替代料"));
        substitutes.AddChild(Substitutes.View, L("查看替代料"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
