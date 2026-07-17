# Manufacturing WMS Platform - Change Log

> **Purpose**: 记录每次需求变更及影响范围，确保变更可控、可追溯。
>
> **Last Updated**: 2026-06-30

---

## 变更记录

### [2026-06-29] 项目启动

| 项目 | 内容 |
|------|------|
| **变更类型** | 项目启动 |
| **变更描述** | 制造业仓储管理平台项目正式启动，基于 README.md 定义的项目目标、技术栈、12 阶段路线图 |
| **变更原因** | 用户发起项目 |
| **影响范围** | 全项目 |
| **影响阶段** | Phase 1 ~ Phase 12 |
| **决策参考** | ADR-001 ~ ADR-004 |
| **状态** | ✅ 已确认 |

### [2026-06-29] Phase 1 行业调研完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 1 Industry Research 完成，产出 944 行行业调研文档，覆盖 9 大章节（行业分析、12个业务痛点、10个典型场景、10类物料、12种仓库、5大流程体系、5行业差异、扩展性策略、评审清单） |
| **变更原因** | 正常阶段推进 |
| **影响范围** | Phase 2 需求分析 |
| **影响阶段** | Phase 2 |
| **产出文件** | `01_PRD/Phase1_Industry_Research.md` |
| **遗留问题** | 保税仓/海关合规、危化品法规、多语言多货币优先级待确认 |
| **状态** | ✅ 已完成，用户已批准 |

### [2026-06-29] Phase 2 需求分析完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 2 Requirement Analysis 完成，产出 PRD 文档（79条需求+50个用户故事+36条业务规则+17条异常规则+5个核心用例），需求追踪矩阵已同步更新 |
| **变更原因** | Phase 1 用户已批准，正常阶段推进 |
| **影响范围** | Phase 3 DDD 领域设计 |
| **影响阶段** | Phase 3 |
| **产出文件** | `01_PRD/Phase2_PRD.md` + `00_ProjectManagement/RequirementTraceability.md` |
| **关键统计** | P0:44条, P1:19条, P2:9条, v2.0+:7条, 共79条需求 |
| **状态** | ✅ 已完成，等待用户评审批准 |

### [2026-06-29] 用户决策：v1.0 范围裁剪

| 项目 | 内容 |
|------|------|
| **变更类型** | 需求范围调整 |
| **变更描述** | 用户明确裁剪 v1.0 范围：保税仓/海关合规不纳入 v1.0；危化品法规合规模块本次不考虑；多语言/多货币本次不考虑 |
| **变更原因** | 用户决策，聚焦核心仓储业务，减少 v1.0 复杂度 |
| **影响范围** | Phase 2 PRD 中的相关需求需降级或移除；Phase 3 DDD 设计无需覆盖这些领域 |
| **影响阶段** | Phase 2 ~ Phase 12 |
| **受影响需求** | REQ-MT-007（危险品属性→移除）、REQ-IV-009（库龄预警→保留但简化）、保税仓相关预留→移除 |
| **决策参考** | ADR-005 |
| **状态** | ✅ 已确认，将同步更新 PRD 和追踪矩阵 |

### [2026-06-29] Phase 3 DDD 领域设计完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 3 DDD Design 完成，产出 2007 行领域设计文档，153 个编号制品（14 BC + 30 AGG + 20 ENT + 17 VO + 37 DE + 30 REP + 6 DS + 5 SM），需求追踪矩阵的 DDD 列已全部填充 |
| **变更原因** | Phase 2 用户已批准，正常阶段推进 |
| **影响范围** | Phase 4 架构设计 |
| **影响阶段** | Phase 4 ~ Phase 12 |
| **产出文件** | `03_DDD/Phase3_DDD_Design.md` + `00_ProjectManagement/RequirementTraceability.md` |
| **关键设计决策** | Inventory Context 为核心下游；聚合拆分避免锁冲突；v1.0 ABP LocalEventBus → v2.0 RabbitMQ；CQRS 同库读写分离 |
| **状态** | ✅ 已完成，用户已批准 |

### [2026-06-29] Phase 4 架构设计完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 4 Architecture Design 完成，产出完整架构设计文档，10 大章节，40+ 编号制品（ARCH/MOD/LAYER/DEP/PLUG/AIW），~118 个后端项目结构，37 NuGet 包 + 15 前端包 + 6 PDA 包 |
| **变更原因** | Phase 3 用户已批准，正常阶段推进 |
| **影响范围** | Phase 5 数据库设计、Phase 6 API 设计、Phase 8-9 代码实现 |
| **影响阶段** | Phase 5 ~ Phase 12 |
| **产出文件** | `02_Architecture/Phase4_Architecture_Design.md` |
| **关键架构决策** | Modular Monolith 三阶段交付（23周）；14 BC→14 ABP Module；4层 Clean Architecture；v1.0 LocalEventBus→v2.0 RabbitMQ；10扩展点+6事件钩子+4行业配置包 |
| **状态** | ✅ 已完成，用户已批准 |

### [2026-06-29] Phase 5 数据库设计完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 5 Database Design 完成，产出 1865 行数据库设计文档，~150 编号制品（15 ER图 + 42表 + 28+索引 + 2分区表 + 15+唯一约束 + 17 VO存储策略 + 26性能优化项） |
| **变更原因** | Phase 4 用户已批准，正常阶段推进 |
| **影响范围** | Phase 6 API 设计、Phase 8-9 代码实现 |
| **影响阶段** | Phase 6 ~ Phase 12 |
| **产出文件** | `04_Database/Phase5_Database_Design.md` |
| **关键数据库决策** | v1.0 不使用物理外键（应用层逻辑外键）；InventoryBalance 5字段复合唯一索引；InventoryLedger 三层不可删除；乐观锁+Polly重试；CQRS同库读写分离 |
| **状态** | ✅ 已完成，用户已批准 |

### [2026-06-29] Phase 6 API 设计完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 6 API Design 完成，产出 2310 行 API 设计文档，180+ REST API + 4 SignalR Hub + 20+ PDA API + 9 外部集成 API；5层DTO策略；30+错误码；85权限定义；7角色 |
| **变更原因** | Phase 5 用户已批准，正常阶段推进 |
| **影响范围** | Phase 7 UI 设计、Phase 8-9 代码实现 |
| **影响阶段** | Phase 7 ~ Phase 12 |
| **产出文件** | `05_API/Phase6_API_Design.md` |
| **关键API决策** | RESTful+SignalR双通道架构；5层DTO策略（Create/Update/Output/Query/Command）；幂等性设计（IdempotencyId+Redis缓存）；JWT认证+7角色+85权限+仓库级数据权限；URL版本控制/api/v1/；跨模块通信DI/EventBus/HTTP三通道 |
| **状态** | ✅ 已完成，用户已批准 |

### [2026-06-29] Phase 7 UI 设计完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 7 UI Design 完成，产出完整 UI 设计文档，46 桌面页面 + 8 PDA 页面 + 5 Dashboard + 18 自定义组件 + 4 核心交互流程 Mermaid 图；Design System 含 20+ CSS 变量 + 6 库存状态色 + 5 单据状态色；85 权限→路由映射；7 角色设计 |
| **变更原因** | Phase 6 用户已批准，正常阶段推进 |
| **影响范围** | Phase 8 前端开发、Phase 9 业务模块开发 |
| **影响阶段** | Phase 8 ~ Phase 12 |
| **产出文件** | `06_UI/Phase7_UI_Design.md` |
| **关键UI决策** | 18 自定义业务组件（WmsTable/WmsSearch/WmsBarcodeInput等）；4标准页面模式（List/Detail/Form/Dashboard）；PDA 扫码驱动工作流；SignalR 轻量提示→用户控制刷新策略；5层导航结构 |
| **状态** | ✅ 已完成，用户已批准 |

### [2026-06-29] Phase 8 基础框架启动

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段启动 |
| **变更描述** | Phase 8 Foundation Framework 启动，工程师寇豆码将创建完整 .NET 解决方案结构（~118 后端项目）、Vue3 前端项目骨架、UniApp PDA 项目骨架、编码规范文档 |
| **变更原因** | Phase 7 用户已批准，正常阶段推进 |
| **影响范围** | Phase 9 业务模块开发（所有模块基于此框架开发） |
| **影响阶段** | Phase 9 ~ Phase 12 |
| **产出文件** | `07_Backend/Wms/` + `08_Frontend/wms-web/` + 编码规范文档 |
| **退出条件** | 解决方案结构完整 + 项目可编译 + 编码规范文档就绪 |
| **状态** | ✅ 已完成，用户已批准 |

### [2026-06-29] Phase 8 基础框架完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 8 Foundation Framework 完成，产出 104 个 .NET 后端项目骨架（14 模块 × 7 层 + 3 Host + TestBase + Shared Kernel）、Vue3 前端项目骨架（29 文件）、编码规范文档。所有模块间依赖声明与架构设计文档一致 |
| **变更原因** | Phase 7 用户已批准，正常阶段推进 |
| **影响范围** | Phase 9 业务模块开发 |
| **影响阶段** | Phase 9 |
| **产出文件** | `07_Backend/Wms/` + `08_Frontend/wms-web/` + `Phase8_Coding_Conventions.md` |
| **关键统计** | 104 .csproj + 104 ABP Module 类 + 15 Shared Kernel 文件 + 29 前端文件 + 编码规范文档 |
| **状态** | ✅ 已完成 |

### [2026-06-29] Phase 9 业务模块开发启动

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段启动 |
| **变更描述** | Phase 9 Business Modules 启动，按架构设计 Phase A/B/C 分阶段交付。Phase A-2（Warehouse + Material）为首个子阶段 |
| **变更原因** | Phase 8 用户已批准，正常阶段推进 |
| **影响范围** | 全部 14 个业务模块 |
| **影响阶段** | Phase 9 ~ Phase 12 |
| **分阶段计划** | Phase A-2: Warehouse+Material ✓ → A-3: Inventory ✓ → A-4: Inbound+Outbound ✓ → A-5: TaskCenter ✓ → Phase B: Transfer+CycleCount+LineSide+Production ✓ → Phase C |
| **状态** | 🔄 Phase B 已完成 |

### [2026-06-30] Phase A-4 完成（Inbound + Outbound）

| 项目 | 内容 |
|------|------|
| **变更类型** | 子阶段交付 |
| **变更描述** | Phase A-4 完成：Inbound 模块（43 个 .cs 文件，全 7 层完整）+ Outbound 模块（40 个 .cs 文件，本次补全 5 个缺失层共 19 个新文件），覆盖 4 聚合根、13 领域事件、2 领域服务、2 状态机、24 DTO、25 API 端点、32 领域测试用例，IS_PASS: YES |
| **变更原因** | Phase A-3 用户已批准，正常阶段推进 |
| **影响范围** | Phase A-5（TaskCenter 依赖 Inbound/Outbound 的 Contracts） |
| **产出统计** | Outbound 新增: 8 DTO + 10 权限 + 1 IAppService(12方法) + 4 Validator + 1 AppService(12方法,3处跨模块调用) + 3 EventHandler + 1 AutoMapper + 2 EFConfig + 1 Repository + 1 Controller(12端点) + 2 测试类(19用例); Inbound: 确认已有43文件完整性 |
| **关键设计决策** | OutboundAppService 3处CROSS-002同步调用(Allocate→Reserve/Complete→Decrease+Release/ReleaseAllocation→Release); 超发控制OB-003; 发货校验OB-006; SM-02状态机Draft→Allocated→Picking→Shipped→Completed |
| **状态** | ✅ 已完成 |

### [2026-06-30] Phase A-5 完成（TaskCenter）

| 项目 | 内容 |
|------|------|
| **变更类型** | 子阶段交付 |
| **变更描述** | Phase A-5 完成：TaskCenter 模块（38 个 .cs 文件，8脚手架+30新增），覆盖 1 聚合根(WarehouseTask, AGG-14)、2 SmartEnum(TaskStatus+AssignmentStrategy)、5 领域事件(DE-029~033)、1 领域服务(DS-05, 7方法)、1 状态机(SM-03)、1 仓储接口(8查询)、6 DTO、17 权限、14 API 方法、5 EventHandler、6 EF索引、26 领域测试用例。同时新增 Shared Kernel ITaskDomainService 跨模块接口(CROSS-003) |
| **变更原因** | Phase A-4 用户已批准，正常阶段推进 |
| **影响范围** | Phase B（Transfer/CycleCount 等模块可调用 ITaskDomainService 创建任务） |
| **产出统计** | TaskCenter新增: 1聚合+2枚举+5事件+1仓储接口+1领域服务(7方法)+6DTO+17权限+1IAppService(14方法)+4Validator+1AppService(14方法)+5EventHandler+1AutoMapper+1DbContext更新+1Repository(8查询)+1Controller(14端点)+2测试类(26用例); Shared Kernel: 1 ITaskDomainService |
| **关键设计决策** | SM-03状态机(Created→Assigned→InProgress→Completed+Suspended+Cancelled); 多态关联(SourceOrderType+SourceOrderId); CROSS-003 ITaskDomainService跨模块接口; BR-028优先级排序; DE-033超时预警 |
| **状态** | ✅ 已完成 |

### [2026-06-30] Phase B 完成（Transfer + CycleCount + LineSide + Production）

| 项目 | 内容 |
|------|------|
| **变更类型** | 子阶段交付 |
| **变更描述** | Phase B 完成：Transfer(35)+CycleCount(30)+LineSide(28)+Production(27)=120 .cs 文件，覆盖 6聚合根(AGG-15~20)、5 SmartEnum、11 领域事件(DE-021~028)、4 领域服务(DS-06~09)、1 状态机(SM-05)、6 仓储接口(REP-11~16)、23 权限、33 API 端点、32 领域测试用例，IS_PASS: YES |
| **变更原因** | Phase A-5 用户已批准，正常阶段推进 |
| **影响范围** | Phase C（BarcodeLabel/Workflow/RuleEngine/Notification 依赖 Transfer/Production Contracts） |
| **产出统计** | Transfer: 10 Domain+8 Contracts+5 App+3 EFCore+1 HttpApi+2 Tests=35; CycleCount: 9+6+2+3+1+2=30; LineSide: 8+6+2+3+1+2=28; Production: 8+5+2+3+1+2=27 |
| **关键设计决策** | SM-05调拨状态机(Draft→Approved→InTransit→Received→Completed); 3盘点方式(Full/Cycle/Spot)+盲盘+差异阈值; Kanban参数(Min/Max)触发补料+消耗倒推; BOM自动展开生成领料单+超领10%审批; 4模块各有跨模块Inventory/TaskCenter同步调用 |
| **状态** | ✅ 已完成 |

### [2026-06-30] Phase C 完成（BarcodeLabel + Workflow + RuleEngine + Notification）

| 项目 | 内容 |
|------|------|
| **变更类型** | 子阶段交付 |
| **变更描述** | Phase C 完成：BarcodeLabel(55)+Workflow(38)+RuleEngine(40)+Notification(65)=198 .cs 文件 + 2 Shared Kernel 接口，覆盖 8聚合根(AGG-21~30)、16 SmartEnum、15 领域事件(DE-034~036+12事件存根)、3 领域服务(DS-10~13)、10 仓储接口(REP-17~26)、18 权限、36 API 端点、69 领域测试用例，IS_PASS: YES |
| **变更原因** | Phase B 用户已批准，正常阶段推进 |
| **影响范围** | Phase 9 全部完成，可进入 Phase 10（测试） |
| **产出统计** | BarcodeLabel: 14 Domain+19 Contracts+4 App+7 EFCore+1 HttpApi+3 Tests=55(47新增); Workflow: 12+7+4+5+1+3=38(30新增); RuleEngine: 8+14+3+5+1+3=40(32新增); Notification: 22+7+14+7+1+4=65(57新增); Shared Kernel: INotificationService+INotificationChannelProvider |
| **关键设计决策** | 条码规则引擎CodePattern({PREFIX}{DATE}{SEQ})+SeqCounter自增; 审批流OwnsMany子实体(ApprovalNode/ActionLog)+多级审批; RuleEngine OHS同步调用模式(IRuleEngineService实现); Notification 12 EventHandler订阅各BC事件+多渠道(5)+模板引擎+INotificationService跨模块接口(CROSS-004) |
| **状态** | ✅ 已完成，Phase 9 全部14模块开发完成 |

### [2026-06-30] Phase 9 前端模块完整开发完成

| 项目 | 内容 |
|------|------|
| **变更类型** | 阶段交付 |
| **变更描述** | Phase 9 前端模块完整开发完成，按照 Phase 7 UI Design 规范实现全部 46 桌面页面 + 5 Dashboard + 18 自定义组件 + 14 API 服务 + 完整 Design System + 动态权限路由 + 4 SignalR Hub 实时集成。总计 118 个前端源码文件，TypeScript 编译 0 错误 |
| **变更原因** | 前端模块在 Phase 9 中遗漏，补充完成 |
| **影响范围** | Phase 9 全面完成，可进入 Phase 10（测试） |
| **影响阶段** | Phase 10 ~ Phase 12 |
| **产出统计** | 57 Views(.vue) + 18 Components(.vue) + 18 API Modules(.ts) + 5 Stores(.ts) + 2 Router(.ts) + 3 Layouts(.vue) + 3 Styles(.scss) + 1 Service(.ts) + 3 root .ts = 118 源码文件 |
| **模块明细** | Warehouse(5页), Material(4页), Inventory(7页), Inbound(4页), Outbound(4页), TaskCenter(3页), Transfer(4页), CycleCount(3页), LineSide(3页), Production(3页), BarcodeLabel(3页), Workflow(2页), RuleEngine(2页), Notification(2页), Dashboard(5页), Login(1页), System(1页) = 57 业务页面 |
| **关键设计决策** | 18 自定义组件（WmsTable/WmsForm/WmsStatusTag等）封装 Element Plus；Design System 20+ CSS变量 + 深色侧边栏(#1E293B) + 主色(#2563EB)；动态路由 45 routes + ABP风格权限字符串；Pinia 4 Store + JWT token 管理；4 SignalR Hub（Inventory/Alert/Task/Notification）自动重连；4 标准页面布局（List/Detail/Form/Dashboard） |
| **技术验证** | `vue-tsc --noEmit` 通过，0 类型错误 |
| **状态** | ✅ 已完成，Phase 9 全面交付 |

---
1. **提出变更**: 任何阶段的需求/设计/技术变更均需在此记录
2. **影响评估**: 评估变更对已完成阶段和后续阶段的影响
3. **决策记录**: 重要变更需新增 ADR 记录到 DecisionLog.md
4. **追踪更新**: 更新 RequirementTraceability.md 中的追踪关系
5. **用户确认**: 变更需经用户确认后方可执行

## 变更类型说明

| 类型 | 说明 | 示例 |
|------|------|------|
| 新增需求 | 新增功能或模块 | 新增 AGV 对接模块 |
| 修改需求 | 修改已有需求 | 修改入库流程增加质检环节 |
| 删除需求 | 删除已有需求 | 移除某客户的定制功能 |
| 技术变更 | 技术栈或架构变更 | 从 SQL Server 切换到 PostgreSQL |
| 范围调整 | 调整阶段范围 | Phase 9 新增模块或调整顺序 |
