# Manufacturing WMS Platform - Decision Log (ADR)

> **Purpose**: 记录每一个重要架构决策及其原因（Architecture Decision Record），方便未来维护和重构。
>
> **Last Updated**: 2026-06-29

---

## ADR-001: 采用 Modular Monolith 架构

| 项目 | 内容 |
|------|------|
| **日期** | 2026-06-29 |
| **状态** | Accepted |
| **背景** | 平台需要支持多客户、长期演进（5年+）。微服务在初期带来过多运维复杂度，单体又缺乏模块边界。 |
| **决策** | 采用 Modular Monolith（模块化单体）架构，每个业务模块独立打包，通过 ABP Module System 管理依赖。 |
| **理由** | 1) 初期降低运维复杂度；2) ABP Module System 天然支持模块化；3) 未来可逐步将模块拆为 Independent Module 再拆为 Service。 |
| **替代方案** | 1) 纯单体 — 模块边界不清，难演进；2) 微服务 — 初期运维成本过高。 |
| **影响** | 所有模块必须遵循 ABP Module 规范，模块间通过接口而非直接引用通信。 |
| **未来演进** | Module → Independent Module → Service |

## ADR-002: 技术栈选型

| 项目 | 内容 |
|------|------|
| **日期** | 2026-06-29 |
| **状态** | Accepted |
| **背景** | 需要选择后端、前端、PDA、部署的技术栈。 |
| **决策** | Backend: .NET 8 + ABP + EF Core + SQL Server + Redis + SignalR；Frontend: Vue3 + TypeScript + Element Plus + Pinia；PDA: UniApp；Deployment: Windows Server + IIS + Docker (Optional) |
| **理由** | 1) ABP Framework 原生支持 DDD 和模块化；2) .NET 8 是 LTS 版本；3) Vue3 + Element Plus 生态成熟；4) UniApp 跨平台 PDA 开发效率高。 |
| **替代方案** | 1) Java + Spring Boot — 团队 .NET 经验更丰富；2) React — Vue3 学习曲线更平。 |
| **影响** | 确定了整个技术栈，后续所有设计基于此。 |

## ADR-003: 采用 DDD + Clean Architecture

| 项目 | 内容 |
|------|------|
| **日期** | 2026-06-29 |
| **状态** | Accepted |
| **背景** | README 明确要求 "Never generate CRUD-oriented architecture. Always design around Domain Driven Design." |
| **决策** | 全量采用 DDD 战略设计（限界上下文、上下文映射）和战术设计（聚合、实体、值对象、领域事件、领域服务）。分层采用 Clean Architecture（Domain → Application → Infrastructure → Presentation）。 |
| **理由** | 1) 制造业仓储领域复杂度高，DDD 能有效管理复杂度；2) Clean Architecture 确保依赖方向正确；3) 配合 CQRS 处理读写分离场景。 |
| **替代方案** | 传统三层架构 — 无法有效管理领域复杂度。 |
| **影响** | 每个模块必须包含 Domain、Application、Infrastructure、Presentation 四层。业务逻辑只能在 Domain 层。 |

## ADR-004: 配置优先 + 插件扩展策略

| 项目 | 内容 |
|------|------|
| **日期** | 2026-06-29 |
| **状态** | Accepted |
| **背景** | 平台需要支持多客户定制，但不能为每个客户 fork 代码。 |
| **决策** | 1) 配置优先：通过配置文件/数据库配置控制业务行为；2) 插件扩展：定义扩展点（Extension Point），客户通过插件实现定制逻辑。 |
| **理由** | 1) 配置化减少代码分支；2) 插件机制保持核心代码纯净；3) ABP 的 Module System 支持插件式加载。 |
| **影响** | 架构设计需明确定义扩展点，模块开发需预留配置项。 |

## ADR-005: v1.0 范围裁剪 — 移除保税仓/危化品合规/多语言多货币

| 项目 | 内容 |
|------|------|
| **日期** | 2026-06-29 |
| **状态** | Accepted |
| **背景** | Phase 1 遗留3项待确认问题。用户明确 v1.0 聚焦核心仓储业务，降低首版复杂度。 |
| **决策** | 1) 保税仓/海关合规 — 不纳入 v1.0，架构预留接口（IBondedWarehouseService），未来通过行业配置包实现；2) 危化品法规合规深度模块 — 本次不考虑，物料主数据保留基础危险品属性字段，法规合规后续通过插件包实现；3) 多语言/多货币 — 本次不考虑，架构层面 UI 标签采用 key-value 映射预留国际化能力，数据层不增加货币字段 |
| **理由** | 1) 聚焦核心仓储功能（入库/出库/库存/任务），确保 v1.0 高质量交付；2) 保税仓和危化品合规均涉及深度行业法规，独立行业包更灵活；3) 多语言/多货币为国际化需求，当前目标客户为国内制造企业，优先级低 |
| **替代方案** | 1) 保税仓纳入 v1.0 — 增加约 15% 开发量，且法规复杂度高风险；2) 危化品深度覆盖 — 需要 MSDS/GHS 等法规知识，开发周期长 |
| **影响** | REQ-MT-007（危险品属性）从 P1 降级为预留字段；追踪矩阵中保税仓相关行移除；Phase 3 DDD 设计时无需设计保税仓限界上下文 |
| **未来演进** | v2.0 通过行业配置包新增保税仓模块；v2.0+ 危化品合规插件包；v3.0+ 多语言/多货币支持 |

---

> **规则**: 每个重要决策都应新增 ADR 记录，包含背景、决策、理由、替代方案、影响。
