# Phase A-5 — TaskCenter 模块 交付总结

> **交付日期**: 2026-06-30 | **Phase**: A-5 | **模块**: TaskCenter (BC-10) | **优先级**: P0

## TL;DR

完成制造业仓储管理平台的任务中心模块 — TaskCenter 模块（38 个 .cs 文件，8 脚手架 + 30 新增），覆盖 1 聚合根、2 模块内 SmartEnum、5 领域事件、1 领域服务(7方法)、1 状态机(SM-03)、1 仓储接口(8查询方法)、6 DTO、17 权限定义、14 API 方法、5 EventHandler、6 EF 索引、26 领域测试用例。同时新增 Shared Kernel ITaskDomainService 跨模块接口。

## 交付概览

| 指标 | TaskCenter 模块 | Shared Kernel 新增 | 合计 |
|------|----------------|-------------------|------|
| .cs 文件总数 | 38 (8脚手架+30新增) | 1 | 39 |
| 聚合根 | 1 (WarehouseTask, AGG-14) | — | 1 |
| 模块内 SmartEnum | 2 (TaskStatus + AssignmentStrategy) | — | 2 |
| 共享 SmartEnum (已存在) | — | TaskType(8值) + TaskPriority(4值) | 2 |
| 领域事件 | 5 (DE-029~033) | — | 5 |
| 仓储接口+实现 | 1+1 (8查询方法) | — | 1+1 |
| 领域服务 | 1 (DS-05, 7方法) | 1 (ITaskDomainService, 2方法) | 2 |
| 状态机 | 1 (SM-03) | — | 1 |
| DTO | 6 (Create/Update/Output/Query/Commands) | — | 6 |
| 权限 | 17 (6组+11子权限) | — | 17 |
| IAppService | 1 (14 API方法) | — | 1 |
| Validator | 4 (Create/Suspend/Progress/BatchAssign) | — | 4 |
| AppService | 1 (14 API方法) | — | 1 |
| EventHandler | 5 (DE-029~033) | — | 5 |
| AutoMapper | 1 | — | 1 |
| EF Configuration | 1 (内嵌在 DbContext) | — | 1 |
| DbContext | 1 (更新) | — | 1 |
| Repository | 1 (8查询) | — | 1 |
| Controller | 1 (14 端点) | — | 1 |
| 领域测试 | 1 (26 用例) | — | 1 |
| AppService 测试 | 1 (placeholder) | — | 1 |
| IS_PASS | YES ✅ | YES ✅ | — |

## Domain 层 (8 新文件)

| 文件 | 说明 |
|------|------|
| TaskStatus.cs | SmartEnum: Created(0)/Assigned(1)/InProgress(2)/Suspended(3)/Completed(4)/Cancelled(5) |
| AssignmentStrategy.cs | SmartEnum: Manual(0)/Region(1)/Skill(2)/LoadBalance(3) |
| WarehouseTask.cs | AGG-14 聚合根: 21 属性 + SM-03 状态机(7转换方法) + 跨模块调用入口 |
| TaskCreatedEvent.cs | DE-029: 创建事件 |
| TaskAssignedEvent.cs | DE-030: 分配事件 → PDA SignalR |
| TaskCompletedEvent.cs | DE-031: 完成事件 → 通知上游模块 |
| TaskSuspendedEvent.cs | DE-032: 挂起事件 → 通知 WS |
| TaskTimeoutEvent.cs | DE-033: 超时事件 → 通知 WS |
| IWarehouseTaskRepository.cs | REP-10: 8 自定义查询方法 |
| TaskDomainService.cs | DS-05: 7 方法 (CreateFromOrder/Assign/AutoAssign/Suspend/Resume/Complete/CheckTimeout) |

### SM-03 状态机

```
Created → Assigned → InProgress → Completed
  ↓         ↓          ↓
Cancelled  Reassign→Created  Suspended → Resume→InProgress
                                 ↓
                             Cancelled
```

### WarehouseTask 关键属性

- **多态关联**: `SourceOrderType + SourceOrderId + SourceOrderNo` — 关联 InboundOrder/OutboundOrder/TransferOrder
- **优先级管理**: BR-028 优先级排序 (Emergency>High>Medium>Low)，同优先级按创建时间
- **进度追踪**: `TaskProgress` decimal(5,2)，0~100 范围
- **超时预警**: `ExpectedCompletionTime + CheckTimeout()` → DE-033 TaskTimeoutEvent

## Shared Kernel 新增 (1 文件)

| 文件 | 说明 |
|------|------|
| ITaskDomainService.cs | CROSS-003 跨模块接口: CreateTaskFromOrderAsync + CancelTasksBySourceOrderAsync |

## Application.Contracts 层 (8 新文件)

| 文件 | 说明 |
|------|------|
| WarehouseTaskCreateDto.cs | 创建任务 DTO (10 字段) |
| WarehouseTaskUpdateDto.cs | 更新任务 DTO (仅 Created 状态) |
| WarehouseTaskOutputDto.cs | 输出 DTO (SmartEnum 扁平化, 21 字段) |
| WarehouseTaskQueryDto.cs | 分页查询 DTO (10 过滤条件 + 分页) |
| TaskCommandDtos.cs | 7 命令 DTO: Assign/Complete/Suspend/UpdateProgress/BatchAssign/AutoAssign/Cancel |
| WmsTaskCenterPermissions.cs | 17 权限定义 (6组 + 11子权限) |
| IWarehouseTaskAppService.cs | 14 方法接口 (API-TC-001~014) |
| WarehouseTaskCreateDtoValidator.cs | 4 Validator (Create/Suspend/Progress/BatchAssign) |

## Application 层 (7 新文件)

| 文件 | 说明 |
|------|------|
| WarehouseTaskAppService.cs | 14 API 方法实现, 含 GetMyTasks(当前用户) + BatchAssign + AutoAssign |
| TaskCreatedEventHandler.cs | DE-029 处理, Notification placeholder |
| TaskAssignedEventHandler.cs | DE-030 处理, PDA SignalR placeholder |
| TaskCompletedEventHandler.cs | DE-031 处理, 上游通知 placeholder |
| TaskSuspendedEventHandler.cs | DE-032 处理, WS 通知 placeholder |
| TaskTimeoutEventHandler.cs | DE-033 处理, 超时预警 placeholder |
| WarehouseTaskAutoMapperProfile.cs | Entity → DTO 映射 (SmartEnum → Value + Description) |

## EFCore 层 (2 新文件 + 1 更新)

| 文件 | 说明 |
|------|------|
| WmsTaskCenterDbContext.cs | **更新**: 注册 DbSet<WarehouseTask> + 配置(6索引 + SmartEnum转换 + decimal精度) |
| WarehouseTaskRepository.cs | REP-10 实现: 8 自定义查询方法 |

### EF Core 配置

- **6 索引**: UK_TC_TaskNo + Warehouse/Status + AssignedUser/Status + SourceOrder + Priority/Status + ExpectedTime/Status
- **SmartEnum → int**: HasConversion for TaskType/TaskPriority/TaskStatus/AssignmentStrategy
- **decimal(5,2)**: TaskProgress 精度
- **HasFilter("[IsDeleted] = 0")**: UK 索引配合软删除

## HttpApi 层 (1 新文件)

| 文件 | 说明 |
|------|------|
| WarehouseTaskController.cs | API-TC-001~014, 14 REST 端点 (/api/v1/task-center/tasks) |

### 14 API 端点

| API-ID | Method | Path | 权限 |
|--------|--------|------|------|
| API-TC-001 | GET | /tasks | Read.List |
| API-TC-002 | GET | /tasks/{id} | Read.Detail |
| API-TC-003 | POST | /tasks | Create |
| API-TC-004 | PATCH | /tasks/{id}/assign | Assign.Single |
| API-TC-005 | PATCH | /tasks/{id}/start | Execute.Start |
| API-TC-006 | PATCH | /tasks/{id}/complete | Execute.Complete |
| API-TC-007 | PATCH | /tasks/{id}/suspend | Suspend.Task |
| API-TC-008 | PATCH | /tasks/{id}/resume | Suspend.Resume |
| API-TC-009 | PATCH | /tasks/{id}/cancel | Cancel |
| API-TC-010 | GET | /tasks/my-tasks | Read.MyTasks |
| API-TC-011 | GET | /tasks/by-source-order | Read.BySourceOrder |
| API-TC-012 | POST | /tasks/batch-assign | Assign.Batch |
| API-TC-013 | PATCH | /tasks/{id}/update-progress | Execute.UpdateProgress |
| API-TC-014 | POST | /tasks/auto-assign | Assign.Auto |

## Tests 层 (2 新文件)

| 文件 | 说明 |
|------|------|
| WarehouseTaskTests.cs | 26 领域测试用例 (创建/分配/开始/完成/挂起/恢复/取消/重新分配/进度/优先级/全生命周期) |
| WarehouseTaskAppServiceTests.cs | AppService 测试 placeholder (v1.1 集成测试) |

## 领域测试覆盖 (26 用例)

| # | 测试 | 覆盖点 |
|---|------|--------|
| 1 | Create_ShouldHaveCreatedStatus | 创建 + 初始状态 |
| 2 | Create_WithEmptyTaskNo_ShouldThrow | 参数校验 |
| 3 | Create_WithEmptySourceOrderNo_ShouldThrow | 参数校验 |
| 4 | Assign_ShouldTransitionToAssigned | SM-03 分配 |
| 5 | Assign_WhenInProgress_ShouldThrow | 状态守卫 TC-001 |
| 6 | Assign_SameUserAgain_ShouldThrow | 重复分配 TC-002 |
| 7 | Assign_WithStrategy_ShouldUpdateStrategy | 策略变更 |
| 8 | Start_ShouldTransitionToInProgress | SM-03 开始 |
| 9 | Start_WhenNotAssigned_ShouldThrow | 状态守卫 TC-001 |
| 10 | Complete_ShouldTransitionToCompleted | SM-03 完成 |
| 11 | Complete_WhenNotInProgress_ShouldThrow | 状态守卫 TC-001 |
| 12 | Suspend_ShouldTransitionToSuspended | SM-03 挂起 |
| 13 | Suspend_WithoutReason_ShouldThrow | TC-004 原因必填 |
| 14 | Suspend_WhenNotInProgress_ShouldThrow | 状态守卫 TC-001 |
| 15 | Resume_ShouldTransitionToInProgress | SM-03 恢复 |
| 16 | Resume_WhenNotSuspended_ShouldThrow | 状态守卫 TC-001 |
| 17 | Cancel_InCreated_ShouldTransitionToCancelled | 取消 |
| 18 | Cancel_InAssigned_ShouldTransitionToCancelled | 取消 |
| 19 | Cancel_InSuspended_ShouldTransitionToCancelled | 取消 |
| 20 | Cancel_InInProgress_ShouldThrow | 取消守卫 TC-001 |
| 21 | Cancel_InCompleted_ShouldThrow | 取消守卫 TC-001 |
| 22 | Reassign_ShouldTransitionBackToCreated | 重新分配 |
| 23 | Reassign_WhenNotAssigned_ShouldThrow | 重新分配守卫 TC-001 |
| 24 | UpdateProgress_ShouldSetValue | 进度更新 |
| 25 | UpdateProgress_OutOfRange_ShouldThrow | 进度范围校验 |
| 26 | UpdateProgress_WhenNotInProgress_ShouldThrow | 状态守卫 TC-001 |
| 27 | SetPriority_ShouldUpdatePriority | 优先级变更 |
| 28 | FullLifecycle_Created_To_Completed | 全生命周期 |
| 29 | Lifecycle_With_Suspend_And_Resume | 挂起恢复生命周期 |
| 30 | Lifecycle_With_Suspend_Then_Cancel | 挂起后取消 |

## 核心设计亮点

### 1. 多态关联 (Polymorphic Association)

WarehouseTask 通过 `SourceOrderType + SourceOrderId + SourceOrderNo` 关联多种业务单据：
- `InboundOrder` → 上架任务 (TaskType.Putaway)
- `OutboundOrder` → 拣货任务 (TaskType.Picking)
- `TransferOrder` → 移库任务 (TaskType.Transfer)
- `CycleCountPlan` → 盘点任务 (TaskType.CycleCount)

### 2. 跨模块接口 (CROSS-003)

ITaskDomainService 定义在 Shared Kernel，供 Inbound/Outbound 同步调用：
- `CreateTaskFromOrderAsync` — 单据确认后自动创建任务
- `CancelTasksBySourceOrderAsync` — 单据取消后批量取消关联任务

### 3. 状态机 SM-03

6 状态 + 7 转换 + 2 回退路径:
```
Created → Assigned → InProgress → Completed
  ↓         ↓          ↓
Cancelled  Reassign→Created  Suspended → Resume→InProgress
                                 ↓
                             Cancelled
```

### 4. 优先级排序 (BR-028)

任务列表和自动分配均按 `TaskPriority.Value DESC + CreationTime ASC` 排序，确保紧急任务优先调度。

### 5. 超时预警 (DE-033)

`CheckTimeout()` 方法扫描活跃任务，超过 `ExpectedCompletionTime` 的任务发布 `TaskTimeoutEvent`。

## Phase 9 进度

| 子阶段 | 模块 | 状态 |
|--------|------|------|
| Phase A-2 | Warehouse + Material | ✅ 已完成 |
| Phase A-3 | Inventory ⚠️（核心） | ✅ 已完成 |
| Phase A-4 | Inbound + Outbound | ✅ 已完成 |
| Phase A-5 | **TaskCenter** | ✅ **已完成** ← 本次 |
| Phase B | Transfer + CycleCount + LineSide + Production | ⏳ 下一步 |
| Phase C | BarcodeLabel + Workflow + RuleEngine + Notification | ⏳ |
