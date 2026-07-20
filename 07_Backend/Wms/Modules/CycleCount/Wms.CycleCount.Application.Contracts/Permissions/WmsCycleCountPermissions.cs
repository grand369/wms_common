using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.CycleCount.Application.Contracts.Permissions;

/// <summary>PERM-CC: 6 permissions</summary>
public class WmsCycleCountPermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.CycleCount";
    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Execute = GroupName + ".Execute";
    public const string Confirm = GroupName + ".Confirm";
    public const string Adjust = GroupName + ".Adjust";
    public const string Complete = GroupName + ".Complete";

    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(GroupName, L("盘点管理"));
        group.AddPermission(Read, L("盘点管理"));
        group.AddPermission(Create, L("创建盘点单"));
        group.AddPermission(Execute, L("执行盘点"));
        group.AddPermission(Confirm, L("确认盘点"));
        group.AddPermission(Adjust, L("盘点调整"));
        group.AddPermission(Complete, L("完成盘点"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
