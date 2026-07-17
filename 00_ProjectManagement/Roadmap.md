# Manufacturing WMS Platform - Roadmap

> **Purpose**: 记录产品路线图，避免方向漂移。每个阶段的范围、目标、退出条件均在此明确。
>
> **Last Updated**: 2026-06-30

---

## 项目愿景

构建一个**可复用的制造业仓储管理平台**，不是为单一客户定制，而是通过配置化和模块扩展支持未来定制。架构至少支持 5 年演进。

## 核心原则

| 原则 | 说明 |
|------|------|
| Platformization | 平台化，非项目化 |
| Modularization | 每个业务能力成为可复用模块 |
| DDD | 领域驱动设计，围绕业务领域建模 |
| Clean Architecture | 分层清晰，依赖方向正确 |
| Configuration-first | 配置优先，减少硬编码 |
| Plugin-based Extensions | 插件式扩展，支持客户定制 |
| Modular Monolith | 当前单体模块化，未来可拆分为独立服务 |

## 技术栈

| 层 | 技术选型 |
|----|---------|
| Backend | .NET 8, ABP Framework (Open Source), EF Core, SQL Server, Redis, SignalR |
| Frontend | Vue3, TypeScript, Vite, Element Plus, Pinia, Axios |
| PDA | UniApp, REST API |
| Deployment | Windows Server, IIS, Docker (Optional) |

## 12 阶段路线图

| Phase | 名称 | 负责人 | 退出条件 | 状态 |
|-------|------|--------|---------|------|
| 1 | Industry Research（行业调研） | 产品经理 | 行业分析报告通过评审 | ✅ 已完成 |
| 2 | Requirement Analysis（需求分析） | 产品经理 | PRD + 需求追踪矩阵通过评审 | ✅ 已完成 |
| 3 | DDD Design（领域设计） | 架构师 | 限界上下文 + 聚合设计通过评审 | ✅ 已完成 |
| 4 | Architecture Design（架构设计） | 架构师 | 系统架构 + ABP 模块结构通过评审 | ✅ 已完成 |
| 5 | Database Design（数据库设计） | 架构师 | ER 图 + 表设计通过评审 | ✅ 已完成 |
| 6 | API Design（API 设计） | 架构师 | REST API + Swagger 通过评审 | ✅ 已完成 |
| 7 | UI Design（UI 设计） | 产品经理 | 桌面端 + PDA 线框图通过评审 | ✅ 已完成 |
| 8 | Foundation Framework（基础框架） | 工程师 | 解决方案结构 + 编码规范就绪 | ✅ 已完成 |
| 9 | Business Modules（业务模块） | 工程师 | 各模块逐一开发并通过单元测试 | ✅ 已完成（Phase A-2~C 后端: 675 .cs 文件; 前端: 118 源码文件, 57 页面, 18 组件） |
| 10 | Testing（测试） | QA 工程师 | 单元/集成/性能/安全测试通过 | ⏳ 待启动 |
| 11 | Deployment（部署） | 工程师 | 部署指南 + 监控方案就绪 | ⏳ 待启动 |
| 12 | Documentation（文档） | 全员 | 所有手册编写完成 | ⏳ 待启动 |

## 当前业务模块

Warehouse, Inventory, Inbound, Outbound, Transfer, Cycle Count, Production, Line-side Warehouse, Material, Barcode, Label, Task Center, Workflow, Rule Engine, Print, Notification

## 未来扩展模块

MES, QMS, TMS, OMS, Equipment, Digital Twin, AI

## 演进路径

```
Module (当前) → Independent Module (中期) → Service (远期)
```

> **规则**: 每个阶段完成后，必须等待用户批准才能进入下一阶段。不允许自动推进。
