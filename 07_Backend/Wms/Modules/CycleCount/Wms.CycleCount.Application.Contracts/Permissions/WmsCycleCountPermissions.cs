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
        var group = context.AddGroup(GroupName, L("Permission:CycleCount"));
        group.AddPermission(Read, L("Permission:CycleCount.Read"));
        group.AddPermission(Create, L("Permission:CycleCount.Create"));
        group.AddPermission(Execute, L("Permission:CycleCount.Execute"));
        group.AddPermission(Confirm, L("Permission:CycleCount.Confirm"));
        group.AddPermission(Adjust, L("Permission:CycleCount.Adjust"));
        group.AddPermission(Complete, L("Permission:CycleCount.Complete"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
