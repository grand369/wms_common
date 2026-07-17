using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.Workflow.Application.Contracts.Permissions;

/// <summary>
/// PERM-WF: Workflow module permissions — 5 permissions
/// Read, Create, Update, Execute, Approve
/// </summary>
public class WmsWorkflowPermissions : PermissionDefinitionProvider
{
    public const string GroupName = "Wms.Workflow";
    public const string Read = GroupName + ".Read";
    public const string Create = GroupName + ".Create";
    public const string Update = GroupName + ".Update";
    public const string Execute = GroupName + ".Execute";
    public const string Approve = GroupName + ".Approve";

    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(GroupName, L("Permission:Workflow"));
        group.AddPermission(Read, L("Permission:Workflow.Read"));
        group.AddPermission(Create, L("Permission:Workflow.Create"));
        group.AddPermission(Update, L("Permission:Workflow.Update"));
        group.AddPermission(Execute, L("Permission:Workflow.Execute"));
        group.AddPermission(Approve, L("Permission:Workflow.Approve"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
