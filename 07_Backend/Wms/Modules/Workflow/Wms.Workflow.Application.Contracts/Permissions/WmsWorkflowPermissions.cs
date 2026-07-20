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
        var group = context.AddGroup(GroupName, L("工作流管理"));
        group.AddPermission(Read, L("工作流管理"));
        group.AddPermission(Create, L("创建工作流"));
        group.AddPermission(Update, L("更新工作流"));
        group.AddPermission(Execute, L("执行工作流"));
        group.AddPermission(Approve, L("审批工作流"));
    }

    private static LocalizableString L(string name) => LocalizableString.Create(name);
}
