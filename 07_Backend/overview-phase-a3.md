# Phase A-3 — Inventory 核心模块 ⚠️平台心脏 交付总结

> **交付日期**: 2026-06-29 | **Phase**: A-3 | **模块**: Inventory (BC-03) | **优先级**: P0 ⚠️核心

## TL;DR

完成制造业仓储管理平台最核心的 Inventory 模块 — 96 个 C# 文件，覆盖 6 聚合根、7 SmartEnum、7 领域事件、5 仓储接口、核心领域服务（DS-01 严格9方法）、状态机、33 API 端点、29 测试用例。全局一致性审查 IS_PASS: YES，13项关键约束全部通过。

## 交付概览

| 指标 | 数量 | 状态 |
|------|------|------|
| .cs 文件 | 96 | ✅ |
| 聚合根 | 6 | ✅ |
| SmartEnum | 7 | ✅ |
| 领域事件 | 7 (DE-001~007) | ✅ |
| 仓储接口 | 5 | ✅ |
| 领域服务 | 2 (DS-01 9方法 + AlertService 3方法) | ✅ |
| 状态机 | 1 (SM-04) | ✅ |
| 值对象 | 2 | ✅ |
| DTO | 17 | ✅ |
| Service 接口 | 5 | ✅ |
| AppService | 5 | ✅ |
| EventHandler | 5 | ✅ |
| EF Configuration | 6 | ✅ |
| Repository 实现 | 5 | ✅ |
| Controller | 5 (33 API 端点) | ✅ |
| 领域测试 | 4 (29 测试用例) | ✅ |
| AppService 测试 | 1 | ✅ |
| IS_PASS | ✅ YES | 13项约束全部通过 |

## 核心设计亮点

### 1. InventoryBalance — 全平台最核心聚合 (AGG-06)
- **唯一键**: `(MaterialId, WarehouseId, LocationId, BatchNumber, InventoryStatus)` — 同一组合只有一条记录
- **ApplyQuantityChange()**: 所有库存增减操作的唯一入口，自动计算 AvailableQuantity
- **乐观锁**: ConcurrencyVersion (IsConcurrencyToken) 防止并发冲突
- **负库存校验**: allowNegativeInventory 参数控制是否允许扣减后 Quantity < 0

### 2. InventoryLedgerEntry — 不可变实体 (AGG-07) ⚠️
- 继承 `Entity<Guid>` + `IHasCreationTime`（非 FullAuditedAggregateRoot）
- 所有属性 `private set`，构造器一次性设置
- Repository Update/Delete 覆盖为 `NotSupportedException`
- 数据库层仅 SELECT + INSERT 权限

### 3. InventoryDomainService — 严格9方法 (DS-01)
- Increase/Decrease/Reserve/ReleaseReservation/Freeze/Unfreeze/Adjust/CheckSafetyStock/CheckExpiry
- FindOrCreateBalance 模式：不存在时自动创建

### 4. EF Core 配置
- 复合唯一索引 + HasFilter("[IsDeleted] = 0") 处理 nullable BatchNumber
- SmartEnum → int 转换（HasConversion）
- decimal(18,4) 精度
- 7个查询索引覆盖常用查询场景

## 审查修复记录（5项）

| # | 问题 | 修复 |
|---|------|------|
| F1 | SourceOrderNo 不可变但外部赋值 | 添加 sourceOrderNo 参数到构造器 |
| F2 | FreezeRange 缺 WarehouseId | 补充字段 |
| F3 | ByWarehouse 查询用错字段 | 改为 WarehouseId |
| F4 | DTO 缺字段 | 补充 WarehouseId/WarehouseCode |
| F5 | 映射缺字段 | 补充 FreezeRange 映射 |

## Phase 9 进度

| 子阶段 | 模块 | 状态 |
|--------|------|------|
| Phase A-2 | Warehouse + Material | ✅ 已完成 |
| Phase A-3 | Inventory ⚠️（核心） | ✅ 已完成 |
| Phase A-4 | Inbound + Outbound | ⏳ 下一步 |
| Phase A-5 | TaskCenter | ⏳ |
| Phase B | Transfer + CycleCount + LineSide + Production | ⏳ |
| Phase C | BarcodeLabel + Workflow + RuleEngine + Notification | ⏳ |
