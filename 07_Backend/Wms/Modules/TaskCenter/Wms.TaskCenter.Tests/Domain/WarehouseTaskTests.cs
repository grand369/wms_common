using System;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp;
using Wms.Shared.Domain.Enums;
using Wms.TaskCenter.Domain.Aggregates;
using Wms.TaskCenter.Domain.Enums;
using TaskStatus = Wms.TaskCenter.Domain.Enums.TaskStatus;
using Xunit;

namespace Wms.TaskCenter.Tests.Domain;

/// <summary>
/// WarehouseTask domain tests — SM-03 state machine + business rule coverage.
/// </summary>
public class WarehouseTaskTests
{
    private WarehouseTask CreateTestTask()
    {
        return new WarehouseTask(
            Guid.NewGuid(),
            "TC-Putting-INB-20260630-001",
            TaskType.Putaway,
            TaskPriority.Medium,
            "InboundOrder",
            Guid.NewGuid(),
            "INB-20260630-001",
            Guid.NewGuid(),
            "WH-001",
            AssignmentStrategy.Manual,
            expectedCompletionTime: DateTime.UtcNow.AddHours(2));
    }

    // ── 1. Create ──
    [Fact]
    public void Create_ShouldHaveCreatedStatus()
    {
        var task = CreateTestTask();
        task.TaskStatus.ShouldBe(TaskStatus.Created);
        task.TaskType.ShouldBe(TaskType.Putaway);
        task.TaskPriority.ShouldBe(TaskPriority.Medium);
        task.TaskProgress.ShouldBe(0);
    }

    [Fact]
    public void Create_WithEmptyTaskNo_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new WarehouseTask(Guid.NewGuid(), "", TaskType.Putaway, TaskPriority.Medium,
                "InboundOrder", Guid.NewGuid(), "INB-001", Guid.NewGuid(), "WH-001", AssignmentStrategy.Manual));
        ex.ParamName.ShouldBe("taskNo");
    }

    [Fact]
    public void Create_WithEmptySourceOrderNo_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new WarehouseTask(Guid.NewGuid(), "TC-001", TaskType.Putaway, TaskPriority.Medium,
                "InboundOrder", Guid.NewGuid(), "", Guid.NewGuid(), "WH-001", AssignmentStrategy.Manual));
        ex.ParamName.ShouldBe("sourceOrderNo");
    }

    // ── 2. Assign ── Created → Assigned
    [Fact]
    public void Assign_ShouldTransitionToAssigned()
    {
        var task = CreateTestTask();
        var userId = Guid.NewGuid();
        task.Assign(userId, "张三");
        task.TaskStatus.ShouldBe(TaskStatus.Assigned);
        task.AssignedUserId.ShouldBe(userId);
        task.AssignedUserName.ShouldBe("张三");
    }

    [Fact]
    public void Assign_WhenInProgress_ShouldThrow()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        var ex = Assert.Throws<BusinessException>(() => task.Assign(Guid.NewGuid(), "李四"));
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    [Fact]
    public void Assign_SameUserAgain_ShouldThrow()
    {
        var task = CreateTestTask();
        var userId = Guid.NewGuid();
        task.Assign(userId, "张三");
        var ex = Assert.Throws<BusinessException>(() => task.Assign(userId, "张三"));
        ex.Code.ShouldBe("Wms.TaskCenter:TC-002");
    }

    [Fact]
    public void Assign_WithStrategy_ShouldUpdateStrategy()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三", AssignmentStrategy.Region);
        task.AssignmentStrategy.ShouldBe(AssignmentStrategy.Region);
    }

    // ── 3. Start ── Assigned → InProgress
    [Fact]
    public void Start_ShouldTransitionToInProgress()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        task.TaskStatus.ShouldBe(TaskStatus.InProgress);
        task.ActualStartTime.ShouldNotBeNull();
        task.TaskProgress.ShouldBe(0);
    }

    [Fact]
    public void Start_WhenNotAssigned_ShouldThrow()
    {
        var task = CreateTestTask();
        var ex = Assert.Throws<BusinessException>(() => task.Start());
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    // ── 4. Complete ── InProgress → Completed
    [Fact]
    public void Complete_ShouldTransitionToCompleted()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        task.Complete();
        task.TaskStatus.ShouldBe(TaskStatus.Completed);
        task.ActualCompletionTime.ShouldNotBeNull();
        task.TaskProgress.ShouldBe(100);
    }

    [Fact]
    public void Complete_WhenNotInProgress_ShouldThrow()
    {
        var task = CreateTestTask();
        var ex = Assert.Throws<BusinessException>(() => task.Complete());
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    // ── 5. Suspend ── InProgress → Suspended
    [Fact]
    public void Suspend_ShouldTransitionToSuspended()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        task.Suspend("物料损坏，无法继续上架");
        task.TaskStatus.ShouldBe(TaskStatus.Suspended);
        task.SuspendedReason.ShouldBe("物料损坏，无法继续上架");
    }

    [Fact]
    public void Suspend_WithoutReason_ShouldThrow()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        var ex = Assert.Throws<BusinessException>(() => task.Suspend(""));
        ex.Code.ShouldBe("Wms.TaskCenter:TC-004");
    }

    [Fact]
    public void Suspend_WhenNotInProgress_ShouldThrow()
    {
        var task = CreateTestTask();
        var ex = Assert.Throws<BusinessException>(() => task.Suspend("原因"));
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    // ── 6. Resume ── Suspended → InProgress
    [Fact]
    public void Resume_ShouldTransitionToInProgress()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        task.Suspend("物料损坏");
        task.Resume();
        task.TaskStatus.ShouldBe(TaskStatus.InProgress);
        task.SuspendedReason.ShouldBeNull();
    }

    [Fact]
    public void Resume_WhenNotSuspended_ShouldThrow()
    {
        var task = CreateTestTask();
        var ex = Assert.Throws<BusinessException>(() => task.Resume());
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    // ── 7. Cancel ── Created/Assigned/Suspended → Cancelled
    [Fact]
    public void Cancel_InCreated_ShouldTransitionToCancelled()
    {
        var task = CreateTestTask();
        task.Cancel("不需要此任务");
        task.TaskStatus.ShouldBe(TaskStatus.Cancelled);
    }

    [Fact]
    public void Cancel_InAssigned_ShouldTransitionToCancelled()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Cancel("重新分配");
        task.TaskStatus.ShouldBe(TaskStatus.Cancelled);
    }

    [Fact]
    public void Cancel_InSuspended_ShouldTransitionToCancelled()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        task.Suspend("异常");
        task.Cancel("无法恢复，直接关闭");
        task.TaskStatus.ShouldBe(TaskStatus.Cancelled);
    }

    [Fact]
    public void Cancel_InInProgress_ShouldThrow()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        var ex = Assert.Throws<BusinessException>(() => task.Cancel());
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    [Fact]
    public void Cancel_InCompleted_ShouldThrow()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        task.Complete();
        var ex = Assert.Throws<BusinessException>(() => task.Cancel());
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    // ── 8. Reassign ── Assigned → Created
    [Fact]
    public void Reassign_ShouldTransitionBackToCreated()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Reassign();
        task.TaskStatus.ShouldBe(TaskStatus.Created);
        task.AssignedUserId.ShouldBeNull();
        task.AssignedUserName.ShouldBeNull();
    }

    [Fact]
    public void Reassign_WhenNotAssigned_ShouldThrow()
    {
        var task = CreateTestTask();
        var ex = Assert.Throws<BusinessException>(() => task.Reassign());
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    // ── 9. Update Progress ──
    [Fact]
    public void UpdateProgress_ShouldSetValue()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        task.UpdateProgress(50);
        task.TaskProgress.ShouldBe(50);
    }

    [Fact]
    public void UpdateProgress_OutOfRange_ShouldThrow()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "张三");
        task.Start();
        var ex = Assert.Throws<BusinessException>(() => task.UpdateProgress(150));
        ex.Code.ShouldBe("Wms.TaskCenter:TC-ProgressOutOfRange");
    }

    [Fact]
    public void UpdateProgress_WhenNotInProgress_ShouldThrow()
    {
        var task = CreateTestTask();
        var ex = Assert.Throws<BusinessException>(() => task.UpdateProgress(50));
        ex.Code.ShouldBe("Wms.TaskCenter:TC-001");
    }

    // ── 10. Set Priority ──
    [Fact]
    public void SetPriority_ShouldUpdatePriority()
    {
        var task = CreateTestTask();
        task.SetPriority(TaskPriority.Emergency);
        task.TaskPriority.ShouldBe(TaskPriority.Emergency);
    }

    // ── 11. Full Lifecycle ──
    [Fact]
    public void FullLifecycle_Created_To_Completed()
    {
        var task = CreateTestTask();
        task.TaskStatus.ShouldBe(TaskStatus.Created);

        task.Assign(Guid.NewGuid(), "操作员A");
        task.TaskStatus.ShouldBe(TaskStatus.Assigned);

        task.Start();
        task.TaskStatus.ShouldBe(TaskStatus.InProgress);

        task.UpdateProgress(30);
        task.TaskProgress.ShouldBe(30);

        task.UpdateProgress(60);
        task.TaskProgress.ShouldBe(60);

        task.Complete();
        task.TaskStatus.ShouldBe(TaskStatus.Completed);
        task.TaskProgress.ShouldBe(100);
    }

    // ── 12. Lifecycle with Suspend ──
    [Fact]
    public void Lifecycle_With_Suspend_And_Resume()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "操作员A");
        task.Start();
        task.UpdateProgress(40);

        task.Suspend("找不到物料");
        task.TaskStatus.ShouldBe(TaskStatus.Suspended);

        task.Resume();
        task.TaskStatus.ShouldBe(TaskStatus.InProgress);

        task.UpdateProgress(80);
        task.Complete();
        task.TaskStatus.ShouldBe(TaskStatus.Completed);
    }

    // ── 13. Lifecycle with Cancel ──
    [Fact]
    public void Lifecycle_With_Suspend_Then_Cancel()
    {
        var task = CreateTestTask();
        task.Assign(Guid.NewGuid(), "操作员A");
        task.Start();
        task.Suspend("物料损坏严重");

        task.Cancel("无法恢复，直接关闭");
        task.TaskStatus.ShouldBe(TaskStatus.Cancelled);
    }
}
