using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Wms.TaskCenter.Application.Contracts.Permissions;

/// <summary>
/// WmsTaskCenterPermissions — PERM-TC
/// 6 permission definitions for TaskCenter module.
/// </summary>
public class WmsTaskCenterPermissions
{
    public const string GroupName = "Wms.TaskCenter";

    // ── Read ──
    public const string Read = GroupName + ".Read";
    public const string ReadList = Read + ".List";
    public const string ReadDetail = Read + ".Detail";
    public const string ReadMyTasks = Read + ".MyTasks";
    public const string ReadBySourceOrder = Read + ".BySourceOrder";

    // ── Create ──
    public const string Create = GroupName + ".Create";

    // ── Assign ──
    public const string Assign = GroupName + ".Assign";
    public const string AssignSingle = Assign + ".Single";
    public const string AssignBatch = Assign + ".Batch";
    public const string AssignAuto = Assign + ".Auto";

    // ── Execute ──
    public const string Execute = GroupName + ".Execute";
    public const string ExecuteStart = Execute + ".Start";
    public const string ExecuteComplete = Execute + ".Complete";
    public const string ExecuteUpdateProgress = Execute + ".UpdateProgress";

    // ── Suspend ──
    public const string Suspend = GroupName + ".Suspend";
    public const string SuspendTask = Suspend + ".Task";
    public const string ResumeTask = Suspend + ".Resume";

    // ── Cancel ──
    public const string Cancel = GroupName + ".Cancel";
}

public class WmsTaskCenterPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(
            WmsTaskCenterPermissions.GroupName,
            L("Permission:TaskCenter"));

        // Read
        var read = group.AddPermission(WmsTaskCenterPermissions.Read, L("Permission:TaskCenter.Read"));
        read.AddChild(WmsTaskCenterPermissions.ReadList, L("Permission:TaskCenter.Read.List"));
        read.AddChild(WmsTaskCenterPermissions.ReadDetail, L("Permission:TaskCenter.Read.Detail"));
        read.AddChild(WmsTaskCenterPermissions.ReadMyTasks, L("Permission:TaskCenter.Read.MyTasks"));
        read.AddChild(WmsTaskCenterPermissions.ReadBySourceOrder, L("Permission:TaskCenter.Read.BySourceOrder"));

        // Create
        group.AddPermission(WmsTaskCenterPermissions.Create, L("Permission:TaskCenter.Create"));

        // Assign
        var assign = group.AddPermission(WmsTaskCenterPermissions.Assign, L("Permission:TaskCenter.Assign"));
        assign.AddChild(WmsTaskCenterPermissions.AssignSingle, L("Permission:TaskCenter.Assign.Single"));
        assign.AddChild(WmsTaskCenterPermissions.AssignBatch, L("Permission:TaskCenter.Assign.Batch"));
        assign.AddChild(WmsTaskCenterPermissions.AssignAuto, L("Permission:TaskCenter.Assign.Auto"));

        // Execute
        var execute = group.AddPermission(WmsTaskCenterPermissions.Execute, L("Permission:TaskCenter.Execute"));
        execute.AddChild(WmsTaskCenterPermissions.ExecuteStart, L("Permission:TaskCenter.Execute.Start"));
        execute.AddChild(WmsTaskCenterPermissions.ExecuteComplete, L("Permission:TaskCenter.Execute.Complete"));
        execute.AddChild(WmsTaskCenterPermissions.ExecuteUpdateProgress, L("Permission:TaskCenter.Execute.UpdateProgress"));

        // Suspend
        var suspend = group.AddPermission(WmsTaskCenterPermissions.Suspend, L("Permission:TaskCenter.Suspend"));
        suspend.AddChild(WmsTaskCenterPermissions.SuspendTask, L("Permission:TaskCenter.Suspend.Task"));
        suspend.AddChild(WmsTaskCenterPermissions.ResumeTask, L("Permission:TaskCenter.Suspend.Resume"));

        // Cancel
        group.AddPermission(WmsTaskCenterPermissions.Cancel, L("Permission:TaskCenter.Cancel"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create(name);
    }
}
