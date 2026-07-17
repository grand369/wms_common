# Phase 9 — Phase A-2 交付总结

> **阶段**: Phase 9 Business Modules → Phase A-2（Warehouse + Material）
> **日期**: 2026-06-29
> **负责人**: 工程师寇豆码（Kou）

## TL;DR

完成制造业仓储管理平台的两个基础主数据模块 — Warehouse（仓库主数据）和 Material（物料主数据）的全部业务代码实现，137 个 C# 文件覆盖 Domain/Application.Contracts/Application/EntityFrameworkCore/HttpApi/Tests 全层。

## 交付概览

| 类别 | Warehouse 模块 | Material 模块 | 合计 |
|------|---------------|--------------|------|
| .cs 文件数 | 63 | 74 | 137 |
| 聚合根 | 3 (Warehouse/WarehouseArea/Location) | 3 (Material/MaterialClassification/UnitOfMeasure) | 6 |
| SmartEnum | 5 (WarehouseType/AreaFunction/StorageEnvironment/LocationType/StorageConditionType) | 10 (MaterialType/QualityInspectionMode/ABCClassificationType/IssueStrategyType/StrategyScope/ErpSyncStatus/DangerLevelType/StorageConditionType/UnitType) | 15 |
| 值对象 | 3 (OrganizationUnit/WarehouseAddress/AreaCapacity) | 5 (StorageAttribute/QualityAttribute/InventoryAttribute/IssueStrategy/DangerAttribute) | 8 |
| 领域事件 | 4 | 4 | 8 |
| 仓储接口+实现 | 3+3 | 3+3 | 6+6 |
| DTO | 12 (4×3) | 14 (5+4+4+1) | 26 |
| 服务接口+实现 | 3+3 | 3+3 | 6+6 |
| 验证器 | 3 | 3 | 6 |
| Controller | 3 | 3 | 6 |
| 领域测试 | 3 | 3 | 6 |
| IS_PASS | YES ✅ | YES ✅ | — |

## 关键设计决策

1. **Material 值对象用 JSON 列存储** — StorageAttribute/QualityAttribute/InventoryAttribute/IssueStrategy/DangerAttribute 存为 nvarchar(max) + JsonSerializer(camelCase)，避免表列膨胀
2. **MaterialSubstituteRelation 作为 Material 子实体** — 嵌套聚合模式，而非独立表（EF Core cascade delete 配置）
3. **所有聚合属性 private set + domain method** — 严格 DDD 原则，外部不能直接修改属性
4. **WarehouseArea/Location 为独立聚合而非嵌套** — 减少锁冲突、支持大量库位操作
5. **冗余字段策略** — WarehouseArea/Location 引用 WarehouseCode/AreaCode 冗余存储

## 下一步

- **Phase A-3**: Inventory 核心模块（AGG-06~AGG-09，库存余额/流水/调整/冻结）
- **Phase A-4**: Inbound + Outbound 模块
- **Phase A-5**: TaskCenter 模块
- Phase B: Transfer + CycleCount + LineSide + Production
- Phase C: BarcodeLabel + Workflow + RuleEngine + Notification
