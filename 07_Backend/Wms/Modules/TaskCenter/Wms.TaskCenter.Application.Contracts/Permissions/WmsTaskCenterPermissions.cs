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
            L("任务中心"));

        // Read
        var read = group.AddPermission(WmsTaskCenterPermissions.Read, L("任务中心"));
        read.AddChild(WmsTaskCenterPermissions.ReadList, L("任务列表"));
        read.AddChild(WmsTaskCenterPermissions.ReadDetail, L("任务详情"));
        read.AddChild(WmsTaskCenterPermissions.ReadMyTasks, L("我的任务"));
        read.AddChild(WmsTaskCenterPermissions.ReadBySourceOrder, L("按来源单查询"));

        // Create
        group.AddPermission(WmsTaskCenterPermissions.Create, L("创建任务"));

        // Assign
        var assign = group.AddPermission(WmsTaskCenterPermissions.Assign, L("任务分配"));
        assign.AddChild(WmsTaskCenterPermissions.AssignSingle, L("单人分配"));
        assign.AddChild(WmsTaskCenterPermissions.AssignBatch, L("批量分配"));
        assign.AddChild(WmsTaskCenterPermissions.AssignAuto, L("自动分配"));

        // Execute
        var execute = group.AddPermission(WmsTaskCenterPermissions.Execute, L("任务执行"));
        execute.AddChild(WmsTaskCenterPermissions.ExecuteStart, L("开始任务"));
        execute.AddChild(WmsTaskCenterPermissions.ExecuteComplete, L("完成任务"));
        execute.AddChild(WmsTaskCenterPermissions.ExecuteUpdateProgress, L("更新进度"));

        // Suspend
        var suspend = group.AddPermission(WmsTaskCenterPermissions.Suspend, L("任务挂起"));
        suspend.AddChild(WmsTaskCenterPermissions.SuspendTask, L("挂起任务"));
        suspend.AddChild(WmsTaskCenterPermissions.ResumeTask, L("恢复任务"));

        // Cancel
        group.AddPermission(WmsTaskCenterPermissions.Cancel, L("取消任务"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create(name);
    }
}
