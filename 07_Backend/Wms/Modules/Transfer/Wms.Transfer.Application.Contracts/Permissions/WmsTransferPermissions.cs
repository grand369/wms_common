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
        var group = context.AddGroup(GroupName, L("移库管理"));
        group.AddPermission(Read, L("移库管理"));
        group.AddPermission(Create, L("创建移库单"));
        group.AddPermission(Update, L("更新移库单"));
        group.AddPermission(Delete, L("删除移库单"));
        group.AddPermission(Submit, L("提交移库单"));
        group.AddPermission(Approve, L("审批移库单"));
        group.AddPermission(Outbound, L("移库出库"));
        group.AddPermission(Inbound, L("移库入库"));
        group.AddPermission(Complete, L("完成移库"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
