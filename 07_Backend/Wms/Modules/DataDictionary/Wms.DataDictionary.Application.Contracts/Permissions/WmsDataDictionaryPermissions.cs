using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.DataDictionary.Application.Contracts.Permissions;

public class WmsDataDictionaryPermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.DataDictionary";

    public static class Dictionaries
    {
        public const string Default = GroupName + ".Dictionaries";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string List = Default + ".List";
    }

    public static class Items
    {
        public const string Default = GroupName + ".Items";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string View = Default + ".View";
        public const string List = Default + ".List";
    }

    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(GroupName, L("数据字典"));

        var dictionaries = group.AddPermission(Dictionaries.Default, L("字典管理"));
        dictionaries.AddChild(Dictionaries.Create, L("创建字典"));
        dictionaries.AddChild(Dictionaries.Update, L("更新字典"));
        dictionaries.AddChild(Dictionaries.Delete, L("删除字典"));
        dictionaries.AddChild(Dictionaries.View, L("查看字典"));
        dictionaries.AddChild(Dictionaries.List, L("字典列表"));

        var items = group.AddPermission(Items.Default, L("字典项管理"));
        items.AddChild(Items.Create, L("创建字典项"));
        items.AddChild(Items.Update, L("更新字典项"));
        items.AddChild(Items.Delete, L("删除字典项"));
        items.AddChild(Items.View, L("查看字典项"));
        items.AddChild(Items.List, L("字典项列表"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
