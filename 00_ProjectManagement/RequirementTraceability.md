# Manufacturing WMS Platform - Requirement Traceability Matrix

> **Purpose**: 建立需求到设计、开发、测试的追踪关系，确保每个需求都有对应的设计和测试覆盖。
>
> **Last Updated**: 2025-07 (Phase 3 DDD 设计完成后填充 BC 和 AGG 列)

---

## 追踪矩阵

> ✅ Phase 2 需求分析已完成，P0 核心模块需求追踪链已填充。✅ Phase 3 DDD 设计已完成，BC 和 AGG 列已填充。后续阶段将逐步填充 DB/API/UI/代码/测试列。

### P0 核心模块（Warehouse / Material / Inventory / Inbound / Outbound / TaskCenter）

| 需求ID | 需求描述 | 优先级 | 来源痛点 | 关联用户故事 | 关联业务规则 | 关联异常规则 | DDD 设计 | 数据库设计 | API 设计 | UI 设计 | 代码实现 | 测试用例 | 状态 |
|--------|---------|--------|----------|------------|------------|------------|---------|-----------|---------|--------|---------|---------|------|
| REQ-WH-001 | 多仓库管理（12+类型） | P0 | P06 | US-WH-001 | BR-033 | — | BC-01, AGG-01 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WH-002 | 三级仓库层级（仓→区→位） | P0 | P01 | US-WH-002 | BR-018 | ER-006 | BC-01, AGG-02, AGG-03 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WH-003 | 多工厂仓库架构 | P0 | P06 | US-WH-003 | BR-003 | ER-011 | BC-01, AGG-01 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WH-004 | 库区按功能/环境分类 | P0 | P01 | US-WH-001 | BR-018 | — | BC-01, AGG-02 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WH-005 | 库位属性校验 | P0 | P01 | US-WH-002 | BR-018 | ER-006 | BC-01, AGG-03 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WH-006 | 库位条码标识 | P0 | P05 | US-WH-002 | — | — | BC-01, AGG-03 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WH-007 | 仓库主数据导入/导出 | P0 | — | US-WH-004 | — | — | BC-01, AGG-01 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WH-008 | 集团统一库存视图 | P1 | P06 | US-WH-003 | BR-002 | — | BC-01, AGG-01 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-001 | 物料主数据管理 | P0 | P05 | US-MT-001 | — | — | BC-02, AGG-04 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-002 | 物料分类体系 | P0 | P05 | US-MT-001 | — | — | BC-02, AGG-05 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-003 | 物料仓储属性 | P0 | P01 | US-MT-001 | BR-018 | — | BC-02, AGG-04, VO-11 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-004 | 批次/序列号/有效期开关 | P0 | P02/P11 | US-MT-002 | BR-013, BR-014 | ER-017 | BC-02, AGG-04, VO-12 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-005 | 安全库存/ABC 分类 | P0 | P03 | US-MT-003 | BR-004 | ER-001 | BC-02, AGG-04, VO-13 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-006 | 发料策略配置 | P0 | P11 | US-MT-004 | BR-021 | ER-007 | BC-02, AGG-04, VO-10 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-007 | 危险品属性 | P1 | — | US-MT-001 | — | — | BC-02, AGG-04, VO-14 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-008 | 替代料关系管理 | P1 | P03 | US-MT-004 | — | ER-001 | BC-02, AGG-04 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-MT-009 | ERP 物料同步 | P0 | P12 | US-MT-001 | BR-020 | ER-014 | BC-02, AGG-04 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-001 | 实时库存总账 | P0 | P01 | US-IV-001 | BR-002 | — | BC-03, AGG-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-002 | 库位级库存 | P0 | P01 | US-IV-002 | BR-002 | — | BC-03, AGG-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-003 | 批次库存追踪 | P0 | P02 | US-IV-003 | BR-013 | — | BC-03, AGG-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-004 | 序列号库存追踪 | P0 | P02 | US-IV-003 | BR-014 | — | BC-03, AGG-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-005 | 库存状态管控 | P0 | P01/P02 | US-IV-004 | BR-002, BR-009, BR-030, BR-031 | ER-005 | BC-03, AGG-06, AGG-09, SM-04 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-006 | 在途库存独立跟踪 | P0 | P06 | US-IV-001 | BR-003 | ER-011 | BC-03, AGG-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-007 | 安全库存预警 | P0 | P03 | US-IV-005 | BR-004 | ER-001 | BC-03, AGG-06, DE-002 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-008 | 临期预警 | P0 | P11 | US-IV-005 | BR-005 | — | BC-03, AGG-06, VO-07, DE-003 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-009 | 库龄预警 | P1 | — | US-IV-001 | BR-006 | — | BC-03, AGG-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-010 | 零库存预警 | P1 | P03 | US-IV-005 | BR-007 | ER-001 | BC-03, AGG-06, DE-007 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-011 | 库存台账报表 | P1 | — | US-IV-001 | — | — | BC-03, AGG-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-012 | 库存流水不可删除 | P0 | P02 | US-IV-006 | BR-010 | — | BC-03, AGG-07, REP-07 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-013 | 库存调整审批 | P0 | P07 | US-IV-006 | BR-008, BR-035 | ER-010 | BC-03, AGG-08, DS-01 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-014 | 库存冻结/解冻 | P0 | P02 | US-IV-004 | BR-009, BR-030 | ER-005 | BC-03, AGG-09, DS-01 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-015 | 库存初始化 | P0 | — | US-IV-006 | — | — | BC-03, AGG-06, DS-01 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IV-016 | 负库存禁止（可配置） | P0 | P01 | US-IV-006 | BR-001 | ER-001 | BC-03, AGG-06, VO-13 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-001 | 采购入库全流程 | P0 | P02 | US-IN-001 | BR-012, BR-020 | ER-002, ER-003 | BC-04, AGG-10, SM-01, DS-02 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-002 | ASN 预到货通知 | P1 | — | US-IN-001 | — | — | BC-04, AGG-10 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-003 | 生产入库（成品/半成品） | P0 | — | US-IN-001 | BR-020 | — | BC-04, AGG-10 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-004 | 退货入库 | P0 | P08 | US-IN-001 | BR-022 | — | BC-04, AGG-10 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-005 | 入库数量校验 | P0 | P02 | US-IN-003 | BR-011, BR-036 | ER-002 | BC-04, AGG-10, DS-02 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-006 | 质检管控 | P0 | P02 | US-IN-004 | BR-015, BR-016, BR-031 | ER-004 | BC-04, AGG-10, SM-01, DS-02 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-007 | 上架库位推荐 | P0 | P01/P11 | US-IN-002 | BR-017, BR-018 | ER-006 | BC-04, AGG-10, VO-17, DS-02 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-008 | PDA 扫码入库 | P0 | P05 | US-IN-001 | — | — | BC-04, AGG-10 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-009 | 入库标签打印 | P0 | P05 | US-IN-005 | BR-019 | ER-016 | BC-04, AGG-10, DE-034, BC-11 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-010 | 批次/序列号录入 | P0 | P02/P11 | US-IN-001 | BR-013, BR-014 | ER-017 | BC-04, AGG-10, VO-05, VO-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-011 | 入库回传 ERP | P0 | P12 | US-IN-001 | BR-020 | ER-014 | BC-04, AGG-10, DE-012 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-IN-012 | 入库差异处理 | P1 | — | US-IN-003 | BR-011 | ER-015 | BC-04, AGG-10, DE-013 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-001 | 生产领料出库 | P0 | P04/P11 | US-OB-001, US-OB-002 | BR-021, BR-022 | ER-001, ER-007 | BC-05, AGG-12, SM-02, DS-03, DS-04 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-002 | 销售出库 | P0 | P12 | US-OB-002, US-OB-005 | BR-022, BR-025 | — | BC-05, AGG-12 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-003 | PDA 扫码出库 | P0 | P05 | US-OB-002 | — | — | BC-05, AGG-12 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-004 | 超领管理 | P0 | P03 | US-OB-001 | BR-023 | — | BC-05, AGG-12, DE-020 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-005 | 替代料领用 | P1 | P03 | US-OB-001 | — | ER-001 | BC-05, AGG-12, DS-04 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-006 | 紧急领料绿色通道 | P0 | P03 | US-OB-003 | BR-026 | — | BC-05, AGG-12, VO-09 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-007 | 发货核对 | P0 | P12 | US-OB-005 | BR-025 | — | BC-05, AGG-12, SM-02 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-008 | 出库回传 ERP | P0 | P12 | US-OB-002 | BR-020 | ER-014 | BC-05, AGG-12, DE-018 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-009 | 出库单据打印 | P0 | P05 | US-OB-002 | BR-019 | ER-016 | BC-05, AGG-12, DE-034, BC-11 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-OB-010 | 退料/退库 | P0 | P08 | US-OB-004 | BR-024 | — | BC-05, AGG-12, DS-03 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-001 | 任务生命周期管理 | P0 | P04 | US-TC-001 | BR-033 | — | BC-10, AGG-14, SM-03, DS-05 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-002 | 任务类型定义 | P0 | P04 | US-TC-001 | — | — | BC-10, AGG-14 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-003 | 任务异常挂起/恢复 | P0 | P04 | US-TC-004 | — | ER-008 | BC-10, AGG-14, SM-03, DE-032 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-004 | 任务优先级管理 | P0 | P03 | US-TC-001, US-OB-003 | BR-026, BR-028 | — | BC-10, AGG-14, VO-09 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-005 | 任务分配策略 | P0 | P04 | US-TC-002 | BR-027 | — | BC-10, AGG-14, DS-05 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-006 | 任务进度追踪 | P0 | P04 | US-TC-001 | — | — | BC-10, AGG-14 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-007 | 任务超时预警 | P1 | — | US-TC-006 | — | ER-009 | BC-10, AGG-14, DE-033 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-008 | PDA 任务执行 | P0 | P04 | US-TC-003 | — | — | BC-10, AGG-14 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TC-009 | 任务批量创建 | P0 | P04 | US-TC-005 | — | — | BC-10, AGG-14, DS-05 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |

### P1 重要模块（Transfer / CycleCount / LineSideWarehouse / Production / Barcode/Label/Print）

| 需求ID | 需求描述 | 优先级 | 来源痛点 | 关联用户故事 | 关联业务规则 | 关联异常规则 | DDD 设计 | 数据库设计 | API 设计 | UI 设计 | 代码实现 | 测试用例 | 状态 |
|--------|---------|--------|----------|------------|------------|------------|---------|-----------|---------|--------|---------|---------|------|
| REQ-TF-001 | 调拨全流程闭环 | P1 | P06 | US-TF-001 | BR-003, BR-033 | ER-011 | BC-06, AGG-15, SM-05, DS-06 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TF-002 | 三种调拨类型 | P1 | P06 | US-TF-001 | — | — | BC-06, AGG-15 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TF-003 | 在途库存跟踪 | P1 | P06 | US-TF-002 | BR-003 | ER-011 | BC-06, AGG-15, DE-021, DE-022 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-TF-004 | 调拨审批流配置 | P1 | — | US-TF-003 | BR-034 | — | BC-06, BC-12, AGG-15 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-CC-001 | 盘点计划制定 | P1 | P07 | US-CC-001 | — | — | BC-07, AGG-16 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-CC-002 | 盘点期间冻结 | P1 | P07 | US-CC-002 | BR-032 | — | BC-07, AGG-16 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-CC-003 | 差异阈值审批 | P1 | P07 | US-CC-003 | BR-008, BR-035 | ER-010 | BC-07, AGG-17, DE-025, BC-12 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-CC-004 | PDA 扫码盘点 | P1 | P07 | US-CC-002 | — | — | BC-07, AGG-17 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-CC-005 | 盘盈盘亏调整单 | P1 | P07 | US-CC-004 | BR-008 | — | BC-07, AGG-17, DE-024 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-LS-001 | 线边仓独立库位管理 | P1 | P09 | US-LS-001 | — | — | BC-08, AGG-18 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-LS-002 | 最小/最大库存控制 | P1 | P09 | US-LS-002 | BR-029 | ER-012 | BC-08, AGG-18, VO-15, DE-026 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-LS-003 | 看板补料触发 | P1 | P09 | US-LS-004 | BR-029 | ER-012 | BC-08, AGG-18, VO-15, DE-026 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-LS-004 | 消耗倒推模式 | P1 | P09 | US-LS-003 | — | — | BC-08, AGG-18, DE-027 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-PD-001 | 领料单自动生成 | P1 | P03/P04 | US-PD-001 | — | — | BC-09, AGG-19 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-PD-002 | 成品入库关联工单 | P1 | — | US-PD-002 | BR-033 | — | BC-09, AGG-19 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-PD-003 | 委外加工追踪 | P1 | P10 | US-PD-003 | — | ER-011 | BC-09, AGG-20 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-BL-001 | 统一条码生成规则 | P1 | P05 | US-BL-001 | — | — | BC-11, AGG-21 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-BL-002 | 标签模板引擎 | P1 | P05 | US-BL-002 | — | — | BC-11, AGG-22 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-BL-003 | 打印服务集成 | P1 | P05 | US-BL-003 | BR-019 | ER-016 | BC-11, AGG-23 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |

### P2 支撑模块（Workflow / RuleEngine / Notification）

| 需求ID | 需求描述 | 优先级 | 来源痛点 | 关联用户故事 | 关联业务规则 | 关联异常规则 | DDD 设计 | 数据库设计 | API 设计 | UI 设计 | 代码实现 | 测试用例 | 状态 |
|--------|---------|--------|----------|------------|------------|------------|---------|-----------|---------|--------|---------|---------|------|
| REQ-WF-001 | 可视化审批流配置 | P2 | P08 | US-WF-001 | BR-034 | — | BC-12, AGG-24 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WF-002 | 审批运行时 | P2 | P08 | US-WF-002 | BR-034 | — | BC-12, AGG-25, DE-036 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-WF-003 | 审批通知推送 | P2 | P08 | US-WF-002 | BR-034 | — | BC-12, AGG-25, DE-035, BC-14 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-RE-001 | 规则可视化配置 | P2 | 行业差异 | US-RE-001 | — | — | BC-13, AGG-26 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-RE-002 | 规则版本管理 | P2 | — | US-RE-003 | — | — | BC-13, AGG-26 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-RE-003 | 行业配置包 | P2 | 行业差异 | US-RE-002 | — | — | BC-13, AGG-27 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-NT-001 | 多渠道通知 | P2 | P03/P07 | US-NT-001 | — | — | BC-14, AGG-28 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-NT-002 | 通知模板配置 | P2 | — | US-NT-002 | — | — | BC-14, AGG-29 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |
| REQ-NT-003 | 通知规则配置 | P2 | — | US-NT-002 | — | — | BC-14, AGG-30 | _(Phase 5)_ | _(Phase 6)_ | _(Phase 7)_ | _(Phase 9)_ | _(Phase 10)_ | ✅ Phase 3 |

### v2.0+ 未来模块需求预留

| 需求ID | 需求描述 | 优先级 | 接口预留 | 状态 |
|--------|---------|--------|----------|------|
| REQ-MES-001 | MES 集成 | v2.0 | IProductionOrderService; InventoryChangedEvent, MaterialConsumedEvent | ⏳ |
| REQ-QMS-001 | QMS 集成 | v2.0 | QualityInspectionCompletedEvent | ⏳ |
| REQ-TMS-001 | TMS 集成 | v2.0 | IShipmentService | ⏳ |
| REQ-OMS-001 | OMS 集成 | v2.0 | ISalesOrderService; OutboundCompletedEvent | ⏳ |
| REQ-EQP-001 | Equipment 集成 | v2.0 | IEquipmentService | ⏳ |
| REQ-DT-001 | Digital Twin | v3.0+ | InventoryChangedEvent, LocationStatusChangedEvent | ⏳ |
| REQ-AI-001 | AI 预测 | v3.0+ | IInventorySnapshotService, IConsumptionPredictionService | ⏳ |

---

## 追踪关系说明

```
行业调研 (Phase 1)
    ↓ 产出业务痛点（P01~P12）、典型场景
需求分析 (Phase 2) ✅ 已完成
    ↓ 产出 REQ-xxx (79条), US-xxx (50个), BR-xxx (36条), ER-xxx (17条), UC-xxx (5个)
DDD 设计 (Phase 3) ✅ 已完成
    ↓ 映射到 BC-xxx (14个), AGG-xxx (30个), ENT-xxx (20个), VO-xxx (17个), DE-xxx (37个), REP-xxx (30个), DS-xxx (6个), SM-xxx (5个)
数据库设计 (Phase 5)
    ↓ 映射到 T-xxx (Table)
API 设计 (Phase 6)
    ↓ 映射到 API-xxx
UI 设计 (Phase 7)
    ↓ 映射到 UI-xxx
代码实现 (Phase 9)
    ↓ 映射到 MOD-xxx (Module)
测试 (Phase 10)
    ↓ 映射到 TC-xxx (Test Case)
```

## 编号规则

| 前缀 | 含义 | 起始阶段 | 当前数量 |
|------|------|---------|---------|
| REQ- | 需求项 | Phase 2 | 79 条（P0:44, P1:19, P2:9, v2.0+:7） |
| US- | 用户故事 | Phase 2 | 50 个（P0:24, P1:17, P2:9） |
| BR- | 业务规则 | Phase 2 | 36 条 |
| ER- | 异常规则 | Phase 2 | 17 条 |
| UC- | 用例 | Phase 2 | 5 个 |
| BC- | 限界上下文 (Bounded Context) | Phase 3 | 14 条 |
| AGG- | 聚合 (Aggregate) | Phase 3 | 30 条 |
| T- | 数据表 (Table) | Phase 5 | _(待填充)_ |
| API- | API 接口 | Phase 6 | _(待填充)_ |
| UI- | UI 页面/组件 | Phase 7 | _(待填充)_ |
| MOD- | 业务模块 | Phase 9 | _(待填充)_ |
| TC- | 测试用例 (Test Case) | Phase 10 | _(待填充)_ |

## Phase 2 统计摘要

| 维度 | 数量 |
|------|------|
| P0 需求 | 44 条 |
| P1 需求 | 19 条 |
| P2 需求 | 9 条 |
| v2.0+ 需求预留 | 7 条 |
| **需求总计** | **79 条** |
| P0 用户故事 | 24 个 |
| P1 用户故事 | 17 个 |
| P2 用户故事 | 9 个 |
| **用户故事总计** | **50 个** |
| 业务规则 | 36 条 |
| 异常规则 | 17 条 |
| 核心用例 | 5 个 |

## Phase 3 统计摘要

| 维度 | 数量 |
|------|------|
| 限界上下文 (BC) | 14 个 |
| 聚合 (AGG) | 30 个 (P0:14, P1:9, P2:7) |
| 实体 (ENT) | 20 个 |
| 值对象 (VO) | 17 个 |
| 领域事件 (DE) | 37 个 |
| 仓储接口 (REP) | 30 个 |
| 领域服务 (DS) | 6 个 |
| 状态机 (SM) | 5 个 |
| **设计制品总计** | **153 个** |
