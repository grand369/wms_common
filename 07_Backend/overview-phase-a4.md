# Phase A-4 — Inbound + Outbound 模块 交付总结

> **交付日期**: 2026-06-30 | **Phase**: A-4 | **模块**: Inbound (BC-09) + Outbound (BC-10) | **优先级**: P0

## TL;DR

完成制造业仓储管理平台的入库和出库两大核心业务模块 — Inbound 模块（43 个 .cs 文件，全 7 层完整）+ Outbound 模块（40 个 .cs 文件，本次补全 5 个缺失层共 19 个新文件），覆盖 4 聚合根、10 领域事件、2 领域服务、2 状态机、24 DTO、59 API 端点、19 个领域测试用例。

## 交付概览

| 指标 | Inbound 模块 | Outbound 模块 | 合计 |
|------|-------------|-------------|------|
| .cs 文件总数 | 43 | 40 | 83 |
| 聚合根 | 2 (InboundOrder + InboundLine) | 2 (OutboundOrder + OutboundLine) | 4 |
| SmartEnum | 3 (InboundStatus/QualityStatus/ErpCallbackStatus) | 4 (OutboundStatus/ErpCallbackStatus/IssueStrategyType + OutboundType[Shared]) | 7 |
| 领域事件 | 6 (DE-008~012) | 7 (DE-014~020) | 13 |
| 仓储接口+实现 | 1+1 | 1+1 | 2+2 |
| 领域服务 | 1 (DS-02, 6方法) | 1 (DS-03, 6方法 + DS-04, 2方法) | 2 |
| 状态机 | 1 (SM-01) | 1 (SM-02) | 2 |
| DTO | 12 | 12 (含嵌套) | 24 |
| AppService | 1 (13 API方法) | 1 (12 API方法) | 2 |
| EventHandler | 2 | 3 | 5 |
| EF Configuration | 2 | 2 | 4 |
| Controller | 1 (13 端点) | 1 (12 端点) | 2 |
| 领域测试 | 1 (13 用例) | 1 (19 用例) | 2 |
| AppService 测试 | 1 | 1 | 2 |
| IS_PASS | YES ✅ | YES ✅ | — |

## Inbound 模块 — 已在 Phase A-3 后完成

Inbound 模块在 Phase A-3 完成后即已全层实现（43 文件），本次确认其完整性：

- **Domain**: InboundOrder (AGG-10) + InboundLine (ENT-08a) + 3 SmartEnum + 6 Domain Event + DS-02 (6方法) + REP-08
- **状态机 SM-01**: Draft → Confirmed → Inspecting → Putaway → Completed (+ Isolated/Cancelled)
- **Application.Contracts**: 12 DTO (Create/Update/Output/Query/Confirm/QualityInspect/Putaway/RecommendLocation) + 9 权限 + IAppService (13方法) + 2 Validator
- **Application**: InboundOrderAppService (13 API方法, 含跨模块调用 IInventoryDomainService.IncreaseInventoryAsync) + 2 EventHandler + AutoMapper
- **EFCore**: 2 Configuration (复合唯一索引 + SmartEnum转换 + decimal精度 + JSON列) + Repository (6自定义查询)
- **HttpApi**: InboundOrderController (API-IN-001~013, 13 REST端点)
- **Tests**: 13 领域测试用例 + 1 AppService 测试

## Outbound 模块 — 本次补全

### 原有 Domain 层 (21 文件, 已在 Phase A-3 期间完成)

- **Domain**: OutboundOrder (AGG-12) + OutboundLine (ENT-09a) + 4 SmartEnum + 7 Domain Event + DS-03 (6方法) + DS-04 (2方法) + REP-09
- **状态机 SM-02**: Draft → Allocated → Picking → Shipped → Completed (+ Cancelled)
- 关键设计：Allocate/Complete/ReleaseAllocation 标注 ⚠️ 提醒调用方同步调用 IInventoryDomainService

### 本次新增 5 层 (19 个新文件 + 1 个更新文件)

#### 1. Application.Contracts (8 新文件)

| 文件 | 说明 |
|------|------|
| OutboundOrderCreateDto.cs | 创建出库单 DTO + 嵌套 OutboundLineCreateDto |
| OutboundOrderUpdateDto.cs | 更新出库单 DTO (仅 Draft 状态) |
| OutboundOrderOutputDto.cs | 输出 DTO + 嵌套 OutboundLineOutputDto (SmartEnum 扁平化) |
| OutboundOrderQueryDto.cs | 分页查询 DTO (类型/状态/仓库/紧急/关键词) |
| OutboundCommandDtos.cs | 3 个命令 DTO: Allocate/Picking/Shipping + 6 嵌套行 DTO |
| WmsOutboundPermissions.cs | 10 权限定义 + PermissionDefinitionProvider |
| IOutboundOrderAppService.cs | 12 方法接口 (API-OB-001~012) |
| OutboundOrderCreateDtoValidator.cs | 4 Validator (Create/Line/Allocate/AllocateLine) |

#### 2. Application (5 新文件)

| 文件 | 说明 |
|------|------|
| OutboundOrderAppService.cs | 12 API 方法实现, 含 3 处跨模块 IInventoryDomainService 调用 |
| OutboundCompletedEventHandler.cs | DE-018 处理, ERP 回传 placeholder |
| OutboundShippedEventHandler.cs | DE-017 处理, 通知 placeholder |
| OverIssueDetectedEventHandler.cs | DE-020 处理, 超发预警 placeholder |
| OutboundAutoMapperProfile.cs | Entity → DTO 映射 (SmartEnum → Value + Description) |

#### 3. EFCore (3 新文件 + 1 更新)

| 文件 | 说明 |
|------|------|
| OutboundOrderConfiguration.cs | TAB-014: 5 索引 (UK + 3 查询 + CreationTime) + SmartEnum转换 + decimal(18,4) + Cascade |
| OutboundLineConfiguration.cs | TAB-014a: 子实体配置 + 索引 + SmartEnum转换 |
| OutboundOrderRepository.cs | REP-09 实现: 5 自定义查询方法 |
| WmsOutboundDbContext.cs | **更新**: 注册 DbSet + ApplyConfiguration |

#### 4. HttpApi (1 新文件)

| 文件 | 说明 |
|------|------|
| OutboundOrderController.cs | API-OB-001~012, 12 REST 端点 (/api/v1/outbound/orders) |

#### 5. Tests (2 新文件)

| 文件 | 说明 |
|------|------|
| OutboundOrderTests.cs | 19 领域测试用例 (创建/行管理/分配/拣货/发货/完成/取消/释放分配/全生命周期) |
| OutboundOrderAppServiceTests.cs | AppService 测试 placeholder (v1.1 集成测试) |

## 核心设计亮点

### 1. 跨模块同步调用 (CROSS-002)

OutboundOrderAppService 包含 3 处关键跨模块调用，均在同一 UnitOfWork 事务内：

| 方法 | 调用 | 说明 |
|------|------|------|
| AllocateAsync | IInventoryDomainService.ReserveInventoryAsync | 分配时预留库存 |
| CompleteAsync | IInventoryDomainService.DecreaseInventoryAsync + ReleaseReservationAsync | 完成时扣减库存 + 释放预留 |
| ReleaseAllocationAsync | IInventoryDomainService.ReleaseReservationAsync | 释放分配时归还预留 |

### 2. 出库状态机 SM-02

```
Draft → Allocated → Picking → Shipped → Completed
  ↓        ↓
Cancelled  ReleaseAllocation → Draft (回退)
```

### 3. 超发控制 (OB-003)

OverIssueRatio 配置化容忍度，分配时校验：
`allocatedQty > requiredQty * (1 + OverIssueRatio)` → 抛出 OverIssueDetectedEvent + BusinessException

### 4. 发货校验 (OB-006)

OutboundLine.SetShippedQuantity 校验：
`shippedQty > pickedQty` → BusinessException

### 5. EF Core 配置

- 复合唯一索引 + HasFilter("[IsDeleted] = 0") 处理软删除
- SmartEnum → int 转换 (HasConversion)
- decimal(18,4) 精度
- 5 个查询索引覆盖常用查询场景
- Cascade delete for Lines → Order

## 领域测试覆盖 (19 用例)

| # | 测试 | 覆盖点 |
|---|------|--------|
| 1 | Create_ShouldHaveDraftStatus | 创建 + 初始状态 |
| 2 | Create_WithoutMaterialRequisition_ShouldThrow | 类型校验 |
| 3 | Create_SalesShipment_WithoutSalesOrder_ShouldThrow | 类型校验 |
| 4 | AddLine_ShouldIncreaseTotalRequiredQuantity | 行管理 |
| 5 | AddLine_WithIssueStrategy_ShouldSetStrategy | 策略设置 |
| 6 | AddLine_WhenNotDraft_ShouldThrow | 状态守卫 |
| 7 | RemoveLine_ShouldDecreaseTotalRequiredQuantity | 行删除 |
| 8 | Allocate_ShouldTransitionToAllocated | SM-02 分配 |
| 9 | Allocate_OverIssueExceeded_ShouldThrow | OB-003 超发 |
| 10 | Allocate_WhenNotDraft_ShouldThrow | 状态守卫 |
| 11 | ConfirmPicking_ShouldTransitionToPicking | SM-02 拣货 |
| 12 | ConfirmPicking_WhenNotAllocated_ShouldThrow | 状态守卫 |
| 13 | ConfirmShipping_ShouldTransitionToShipped | SM-02 发货 |
| 14 | ConfirmShipping_ShippedExceedsPicked_ShouldThrow | OB-006 |
| 15 | Complete_ShouldTransitionToCompleted | SM-02 完成 |
| 16 | Complete_WhenNotShipped_ShouldThrow | 状态守卫 |
| 17 | Cancel_InDraft_ShouldTransitionToCancelled | 取消 |
| 18 | Cancel_InAllocated_ShouldThrow | 取消守卫 |
| 19 | ReleaseAllocation_ShouldTransitionBackToDraft | 释放分配 |
| 20 | ReleaseAllocation_WhenNotAllocated_ShouldThrow | 释放守卫 |
| 21 | FullLifecycle_Draft_To_Completed | 全生命周期 |

## Phase 9 进度

| 子阶段 | 模块 | 状态 |
|--------|------|------|
| Phase A-2 | Warehouse + Material | ✅ 已完成 |
| Phase A-3 | Inventory ⚠️（核心） | ✅ 已完成 |
| Phase A-4 | Inbound + Outbound | ✅ 已完成 |
| Phase A-5 | TaskCenter | ⏳ 下一步 |
| Phase B | Transfer + CycleCount + LineSide + Production | ⏳ |
| Phase C | BarcodeLabel + Workflow + RuleEngine + Notification | ⏳ |
