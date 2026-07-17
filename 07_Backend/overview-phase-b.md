# Phase B — Transfer + CycleCount + LineSide + Production 交付总结

> **交付日期**: 2026-06-30  
> **Phase**: Phase B (业务模块第二批)  
> **总文件**: 120 个 .cs 文件 (4 模块)  
> **总 API 端点**: 33 个 REST 端点

---

## 1. Transfer 模块 (35 .cs 文件)

**限界上下文**: BC-06 — Transfer Context (调拨上下文)  
**聚合根**: AGG-15 TransferOrder + TransferLine 子实体  
**状态机**: SM-05 — Draft → Approved → InTransit → Received → Completed → Closed + Rejected/Cancelled

| 层 | 文件数 | 核心内容 |
|---|---|---|
| Domain | 10 | TransferOrder(AGG-15) + TransferLine + 2 SmartEnum(TransferStatus+ApprovalStatus) + 4 领域事件(DE-021~023+Created) + ITransferOrderRepository(5查询) + DS-06(4方法) |
| Application.Contracts | 8 | 6 DTO + 9 权限(PERM-TF) + ITransferOrderAppService(10方法) + Validator |
| Application | 5 | AppService(10方法) + 3 EventHandler + AutoMapper |
| EFCore | 2+1更新 | Repository(5查询) + DbContext(3索引+SmartEnum+decimal) |
| HttpApi | 1 | Controller(10 REST端点, /api/v1/transfer/orders) |
| Tests | 2 | 12 领域测试(SM-05全覆盖) + AppService placeholder |

**核心设计亮点**:
- **SM-05 状态机**: 8状态完整流转 + Rejected回退 + Cancelled取消
- **跨模块调用**: DS-06 在同一 UoW 内同步调用 `IInventoryDomainService`(源仓扣减/目标仓增加) + `ITaskDomainService`(创建出库/入库任务)
- **ER-011 在途超时**: `CheckInTransitTimeout()` 自动检测超时并发布 `TransferInTransitTimeoutEvent`
- **BR-033 单据闭环**: 出库确认量不超过调拨量；入库确认量不超过出库确认量

---

## 2. CycleCount 模块 (30 .cs 文件)

**限界上下文**: BC-07 — Cycle Count Context (盘点上下文)  
**聚合根**: AGG-16 CycleCountPlan + CycleCountItem 子实体 + AGG-17 CycleCountResult  
**无独立状态机** — 使用 CountStatus 枚举推进 (Planned → InProgress → Completed → Closed)

| 层 | 文件数 | 核心内容 |
|---|---|---|
| Domain | 9 | CycleCountPlan(AGG-16) + CycleCountItem + CycleCountResult(AGG-17) + 2 SmartEnum(CountMethod+CountStatus) + 2 领域事件(DE-024~025) + 2 仓储接口(REP-12~13) + DS-07(3方法) |
| Application.Contracts | 6 | 4 DTO + 6 权限(PERM-CC) + IAppService(9方法) + Validator |
| Application | 2 | AppService(9方法) + AutoMapper |
| EFCore | 2+1更新 | 2 Repository + DbContext(3表+3索引) |
| HttpApi | 1 | Controller(9 REST端点, /api/v1/cycle-count/plans) |
| Tests | 2 | 8 领域测试 + AppService placeholder |

**核心设计亮点**:
- **3种盘点方式**: Full(全盘) / Cycle(循环ABC) / Spot(抽盘)
- **BR-032 盘点冻结**: `FreezeInventory` 配置化，盘点期间禁止出入库
- **DE-025 差异超阈值**: `CheckDifferenceOverThreshold()` 自动检测并触发审批
- **盲盘模式**: `BlindCountEnabled` — PDA 不显示账面数量
- **盘点调整单**: DS-07 `GenerateAdjustmentAsync` 自动调用 `IInventoryDomainService` 调整库存

---

## 3. LineSide 模块 (28 .cs 文件)

**限界上下文**: BC-08 — Line-Side Warehouse Context (线边仓上下文)  
**聚合根**: AGG-18 LineSideWarehouse + LineSideKanbanItem 子实体  
**无独立状态机** — Kanban 参数阈值驱动逻辑

| 层 | 文件数 | 核心内容 |
|---|---|---|
| Domain | 8 | LineSideWarehouse(AGG-18) + LineSideKanbanItem + 1 SmartEnum(ConsumptionMode) + 3 领域事件(DE-026~028) + ILineSideWarehouseRepository(4查询) + DS-08(2方法) |
| Application.Contracts | 6 | 4 DTO + 5 权限(PERM-LS) + IAppService(7方法) + Validator |
| Application | 2 | AppService(7方法) + AutoMapper |
| EFCore | 2+1更新 | Repository(4查询) + DbContext(2表+2索引) |
| HttpApi | 1 | Controller(7 REST端点, /api/v1/line-side/warehouses) |
| Tests | 2 | 5 领域测试 + AppService placeholder |

**核心设计亮点**:
- **Kanban参数**: MinQuantity(触发补料) / MaxQuantity(补料目标) — BR-029 自动触发补料
- **消耗倒推模式**: `BackflushConsume()` 按工单自动扣减 — DE-027 发布 `BackflushConsumedEvent`
- **超最大库存预警**: DE-028 `LineSideOverstockEvent` — ER-013 线边积压预警
- **跨模块调用**: DS-08 补料时调用 `IInventoryDomainService`(主仓扣减) + `ITaskDomainService`(创建补料配送任务)

---

## 4. Production 模块 (27 .cs 文件)

**限界上下文**: BC-09 — Production Context (生产协同上下文)  
**聚合根**: AGG-19 MaterialRequisition + MaterialRequisitionLine 子实体 + AGG-20 SubcontractOrder(v2 placeholder)  
**无独立状态机** — RequisitionStatus 枚举推进

| 层 | 文件数 | 核心内容 |
|---|---|---|
| Domain | 8 | MaterialRequisition(AGG-19) + MaterialRequisitionLine + SubcontractOrder(AGG-20) + 2 SmartEnum(RequisitionStatus+ProductionOrderStatus) + 2 仓储接口(REP-15~16) + DS-09(1方法) |
| Application.Contracts | 5 | 4 DTO + 3 权限(PERM-PD) + IAppService(7方法) + Validator |
| Application | 2 | AppService(7方法) + AutoMapper |
| EFCore | 2+1更新 | Repository(3查询) + DbContext(3表+2索引) |
| HttpApi | 1 | Controller(7 REST端点, /api/v1/production) |
| Tests | 2 | 7 领域测试 + AppService placeholder |

**核心设计亮点**:
- **BOM自动展开**: DS-09 `GenerateRequisitionFromOrderAsync` 从工单+BOM自动生成领料单 — REQ-PD-001
- **BR-023 超领审批**: 领料超过需求量×1.1 时需审批
- **委外追踪**: AGG-20 SubcontractOrder(v2.0 placeholder) — 发料/回收全流程
- **跨模块调用**: 完工入库调用 `IInventoryDomainService.IncreaseInventoryAsync`

---

## 5. 跨模块依赖总图

```
TF_APP → IV_CON(库存扣减/增加) + WH_CON(仓库信息) + TK_CON(任务)
CC_APP → IV_CON(盘点调整) + WH_CON(仓库库位) + TK_CON(盘点任务)
LS_APP → IV_CON(线边库存) + OB_CON(补料出库) + TK_CON(补料任务)
PD_APP → IV_CON(领料扣减/完工入库) + IN_CON(成品入库) + OB_CON(领料出库)
```

所有跨模块调用遵循 DEP-003 规则：仅通过 Contracts 项目引用，不直接引用 Domain/Application 实现。

---

## 6. Phase 9 总进度

| Phase | 模块 | 文件数 | 状态 |
|-------|------|--------|------|
| A-2 | Warehouse + Material | 137 | ✅ |
| A-3 | Inventory 核心 | 96 | ✅ |
| A-4 | Inbound + Outbound | 83 | ✅ |
| A-5 | TaskCenter | 39 | ✅ |
| **B** | **Transfer + CycleCount + LineSide + Production** | **120** | ✅ |
| C | BarcodeLabel + Workflow + RuleEngine + Notification | — | ⏳ |
