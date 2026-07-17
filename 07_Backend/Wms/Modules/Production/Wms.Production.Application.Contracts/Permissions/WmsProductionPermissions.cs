using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Production.Application.Contracts.Permissions;

/// <summary>PERM-PD: 3 permissions</summary>
public class WmsProductionPermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.Production";
    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Complete = GroupName + ".Complete";

    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(GroupName, L("Permission:Production"));
        group.AddPermission(Read, L("Permission:Production.Read"));
        group.AddPermission(Create, L("Permission:Production.Create"));
        group.AddPermission(Complete, L("Permission:Production.Complete"));
    }
    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
