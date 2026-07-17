using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Transfer.Application.Contracts.Permissions;

/// <summary>
/// PERM-TF: Transfer module permissions — 9 permissions
/// Read, Create, Update, Delete, Submit, Approve, Outbound, Inbound, Complete
/// </summary>
public class WmsTransferPermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.Transfer";
    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Update = GroupName + ".Update";
    public const string Delete = GroupName + ".Delete";
    public const string Submit = GroupName + ".Submit";
    public const string Approve = GroupName + ".Approve";
    public const string Outbound = GroupName + ".Outbound";
    public const string Inbound = GroupName + ".Inbound";
    public const string Complete = GroupName + ".Complete";

    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(GroupName, L("Permission:Transfer"));
        group.AddPermission(Read, L("Permission:Transfer.Read"));
        group.AddPermission(Create, L("Permission:Transfer.Create"));
        group.AddPermission(Update, L("Permission:Transfer.Update"));
        group.AddPermission(Delete, L("Permission:Transfer.Delete"));
        group.AddPermission(Submit, L("Permission:Transfer.Submit"));
        group.AddPermission(Approve, L("Permission:Transfer.Approve"));
        group.AddPermission(Outbound, L("Permission:Transfer.Outbound"));
        group.AddPermission(Inbound, L("Permission:Transfer.Inbound"));
        group.AddPermission(Complete, L("Permission:Transfer.Complete"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
