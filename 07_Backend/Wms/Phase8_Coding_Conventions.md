# Phase 8: Manufacturing WMS Platform — Coding Conventions

> **文档版本**: v1.0  
> **撰写日期**: 2025-07  
> **撰写人**: 工程师 寇豆码（Kou）  
> **阶段**: Phase 8 — Foundation Framework (Coding Conventions)

---

## 目录

1. [C# 编码规范](#1-c-编码规范)
2. [Vue3/TypeScript 编码规范](#2-vue3typescript-编码规范)
3. [数据库编码规范 (EF Core)](#3-数据库编码规范-ef-core)
4. [API 编码规范](#4-api-编码规范)
5. [Git 规范](#5-git-规范)
6. [测试规范](#6-测试规范)

---

## 1. C# 编码规范

### 1.1 命名规范

| 类型 | 规则 | 示例 |
|------|------|------|
| 类名 | PascalCase | `InventoryBalance`, `WarehouseAppService` |
| 接口名 | PascalCase + I前缀 | `IInventoryBalanceRepository`, `IWarehouseAppService` |
| 方法名 | PascalCase | `ApplyQuantityChange()`, `GetAvailableQuantity()` |
| 属性名 | PascalCase | `MaterialCode`, `WarehouseId` |
| 私有字段 | camelCase + _前缀 | `_inventoryBalanceRepository`, `_domainService` |
| 参数/局部变量 | camelCase | `materialCode`, `warehouseId` |
| 常量 | PascalCase | `MaxRetryCount`, `DefaultPageSize` |
| 枚举值 | PascalCase | `InventoryStatus.Available`, `TaskPriority.Emergency` |
| ABP Module类 | PascalCase + 模块前缀 | `WmsWarehouseDomainModule`, `WmsInventoryApplicationModule` |
| 命名空间 | PascalCase + 模块层级 | `Wms.Inventory.Domain`, `Wms.Warehouse.Application.Contracts` |
| 项目名 | Wms.{Module}.{Layer} | `Wms.Inventory.Domain`, `Wms.Warehouse.HttpApi` |

### 1.2 文件组织

- **一个类一个文件**：每个类/接口/枚举放在独立文件中
- **文件名与类名一致**：`InventoryBalance.cs` → `InventoryBalance` 类
- **目录结构对应命名空间**：`Wms/Inventory/Domain/Aggregates/` → `Wms.Inventory.Domain.Aggregates`
- **ABP Module注册类**放在项目根目录：`WmsWarehouseDomainModule.cs`

### 1.3 注释规范

```csharp
/// <summary>
/// Inventory Balance Aggregate Root — represents the current stock position
/// for a specific material, warehouse, location, batch, and status combination.
/// (AGG-06, Phase 3 DDD Design)
/// </summary>
public class InventoryBalance : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Applies a quantity change to the balance. 
    /// This is the ONLY method that modifies quantity fields.
    /// (LAYER-D08, Phase 4 Architecture)
    /// </summary>
    /// <param name="quantity">The quantity delta (positive for increase, negative for decrease)</param>
    /// <param name="operationType">The type of inventory operation</param>
    /// <param name="sourceOrderId">The source order that triggered this change</param>
    public void ApplyQuantityChange(decimal quantity, int operationType, Guid sourceOrderId)
    {
        // Business validation
        if (Quantity + quantity < 0)
            throw new BusinessException("WMS:Inventory:NegativeQuantity");
        
        Quantity += quantity;
        AvailableQuantity = Quantity - ReservedQuantity - FrozenQuantity;
        ConcurrencyVersion++;
        
        AddLocalEvent(new InventoryChangedEvent
        {
            BalanceId = Id,
            QuantityDelta = quantity,
            OperationType = operationType,
        });
    }
}
```

- 所有public类和方法必须有XML文档注释
- 注释中引用DDD/架构编号（如 AGG-06, LAYER-D08）便于追溯
- 业务规则注释使用中文补充说明

### 1.4 ABP 规范

- **Module DependsOn声明**：必须与架构设计文档 Section 4.6 一致
- **聚合根继承**：`FullAuditedAggregateRoot<Guid>`（提供审计字段+软删除）
- **应用服务继承**：`ApplicationService` 基类
- **仓储接口继承**：`IRepository<TEntity, Guid>` 基类
- **仓储实现继承**：`EfCoreRepository<TDbContext, TEntity, Guid>` 基类
- **DTO分离**：Contracts项目定义接口+DTO，Application项目实现
- **权限定义**：每个模块的 Contracts 项目定义 `*Permissions` 类
- **模块间通信**：仅通过 Contracts 项目（接口+DTO），不直接引用 Domain/Application

### 1.5 不可变值对象

```csharp
// 值对象使用 readonly record struct（不可变）
public readonly record struct MaterialCode(string Value)
{
    public static MaterialCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Material code cannot be empty.", nameof(code));
        return new MaterialCode(code.Trim());
    }
}
```

### 1.6 Smart Enum

```csharp
// 枚举使用 Smart Enum 模式（含 Description 属性）
public sealed class InventoryStatus : SmartEnum<InventoryStatus, int>
{
    public static readonly InventoryStatus Available = new("Available", 0, "可用");
    public string Description { get; }
    private InventoryStatus(string name, int value, string description) : base(name, value)
    {
        Description = description;
    }
}
```

---

## 2. Vue3/TypeScript 编码规范

### 2.1 命名规范

| 类型 | 规则 | 示例 |
|------|------|------|
| 组件文件名 | PascalCase | `WmsTable.vue`, `MaterialSelector.vue` |
| 组件注册名 | PascalCase | `<WmsTable />`, `<MaterialSelector />` |
| 目录名 | kebab-case | `dashboard/`, `cycle-count/` |
| JS/TS文件名 | camelCase | `useCrud.ts`, `format.ts` |
| 变量/函数 | camelCase | `inventoryBalance`, `formatQuantity()` |
| 类型/接口 | PascalCase | `InventoryBalanceDto`, `WarehouseQuery` |
| 常量 | UPPER_SNAKE_CASE | `API_BASE_URL`, `MAX_PAGE_SIZE` |
| CSS类名 | kebab-case + wms前缀 | `wms-sidebar`, `wms-header` |
| SCSS变量 | kebab-case | `$primary-color`, `$spacing-md` |

### 2.2 组合式 API (Composition API)

```typescript
// 所有组件使用 <script setup lang="ts">
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useTable } from '@/hooks/useTable';
import { useCrud } from '@/hooks/useCrud';

// Props 和 Emits 使用类型定义
const props = defineProps<{
  warehouseId: string;
  isActive?: boolean;
}>();

const emit = defineEmits<{
  submit: [data: WarehouseDto];
  cancel: [];
}>();

// 使用组合式函数
const { loading, tableData, fetchData } = useTable<WarehouseDto>('/api/warehouse');
const { create, update } = useCrud<WarehouseDto>('/api/warehouse');

// 生命周期钩子
onMounted(() => {
  fetchData();
});
</script>
```

### 2.3 组件设计原则

- **业务组件按BC分目录**：`components/business/` → MaterialSelector, WarehouseSelector
- **通用组件独立管理**：`components/common/` → WmsTable, WmsSearch, WmsDialog
- **Props必须声明类型**：使用 TypeScript 泛型 defineProps
- **事件命名语义化**：submit, cancel, search, reset, refresh
- **组件最大行数**：单文件组件不超过 300 行，超过则拆分子组件

### 2.4 TypeScript 规范

- **严格模式**：`strict: true` 在 tsconfig.json
- **禁止any**：除第三方库回调外不使用 `any`
- **接口优于类型别名**：DTO 使用 `interface`，组合函数返回值使用 `type`
- **API响应类型化**：每个 API 调用都有明确的返回类型

---

## 3. 数据库编码规范 (EF Core)

### 3.1 表命名规范

| 规则 | 示例 |
|------|------|
| 表名格式 | `Wms_{Module}_{Entity}` |
| 主键列名 | `Id` (GUID) |
| 外键列名 | `{ReferencedEntity}Id` |
| 冗余字段列名 | `{ReferencedEntity}Code`/`Name` |
| 枚举列名 | 同业务属性名，类型为 `int` |
| JSON列名 | `{AttributeName}` (如 `StorageAttribute`) |
| 审计字段 | ABP自动管理 (CreationTime, CreatorId, etc.) |

### 3.2 EF Core Fluent API 约定

```csharp
// 使用 Fluent API 配置，不用 Data Annotation
builder.ToTable("Wms_Inventory_InventoryBalance");
builder.HasKey(e => e.Id);

// 唯一索引命名规范
builder.HasIndex(e => new { e.MaterialId, e.WarehouseId, e.LocationId, e.BatchNumber, e.InventoryStatus })
    .IsUnique()
    .HasFilter("[IsDeleted] = 0")
    .HasName("UK_IV_Balance_Composite");

// decimal 精度
builder.Property(e => e.Quantity).HasColumnType("decimal(18,4)");

// JSON列值对象映射
builder.Property(e => e.StorageAttribute)
    .HasColumnType("nvarchar(max)")
    .HasConversion(
        vo => JsonSerializer.Serialize(vo, JsonOptions.Default),
        json => JsonSerializer.Deserialize<StorageAttribute>(json, JsonOptions.Default));

// 子实体同表存储
builder.OwnsMany(e => e.Lines, lineBuilder =>
{
    lineBuilder.ToTable("Wms_Inbound_InboundLine");
    lineBuilder.WithOwner().HasForeignKey(l => l.InboundOrderId);
});

// 并发控制
builder.Property(e => e.ConcurrencyVersion).IsConcurrencyToken();
```

### 3.3 仓储约定

- 库存台账不可删除：`InventoryLedgerRepository` 的 Update/Delete 覆盖为 NotSupportedException
- 所有聚合根继承 `FullAuditedAggregateRoot<Guid>`（含软删除）
- 查询使用 `AsNoTracking()` + `Select()` 投影（读侧）

---

## 4. API 编码规范

### 4.1 URL 命名

| 操作 | URL格式 | 示例 |
|------|---------|------|
| 列表查询 | GET /api/{module} | GET /api/warehouse |
| 详情查询 | GET /api/{module}/{id} | GET /api/warehouse/{id} |
| 创建 | POST /api/{module} | POST /api/inbound-order |
| 更新 | PUT /api/{module}/{id} | PUT /api/inbound-order/{id} |
| 删除 | DELETE /api/{module}/{id} | DELETE /api/location/{id} |
| 子资源 | GET /api/{module}/{id}/{sub} | GET /api/inbound-order/{id}/lines |
| 操作型 | POST /api/{module}/{id}/{action} | POST /api/inbound-order/{id}/confirm |

### 4.2 DTO 命名

| 类型 | 规则 | 示例 |
|------|------|------|
| 查询结果DTO | `{Entity}Dto` | `WarehouseDto`, `InventoryBalanceDto` |
| 创建DTO | `Create{Entity}Dto` | `CreateWarehouseDto`, `CreateInboundOrderDto` |
| 更新DTO | `Update{Entity}Dto` | `UpdateWarehouseDto`, `UpdateLocationDto` |
| 查询参数DTO | `{Entity}QueryDto` / `Get{Entity}ListDto` | `GetWarehouseListDto` |
| 子实体DTO | `{Entity}LineDto` | `InboundLineDto`, `OutboundLineDto` |

### 4.3 Swagger 规范

- ABP `AbpSwaggerOptions` 配置模块分组
- 每个模块的 API 使用统一标签：`Warehouse`, `Inventory`, `Inbound` 等
- API 描述使用中文注释
- 请求/响应示例在 Swagger 中展示

---

## 5. Git 规范

### 5.1 分支策略

| 分支 | 用途 | 说明 |
|------|------|------|
| `main` | 生产发布 | 合并后触发 CI/CD |
| `develop` | 开发集成 | 日常开发合并到此 |
| `feature/{module}-{desc}` | 功能开发 | 如 `feature/inventory-balance-crud` |
| `bugfix/{desc}` | Bug修复 | 如 `bugfix/inventory-available-calculation` |
| `hotfix/{desc}` | 紧急修复 | 从 main 分出，修复后合并回 main + develop |

### 5.2 Commit 格式

```
<type>(<scope>): <subject>

<body>
```

| type | 说明 |
|------|------|
| `feat` | 新功能 |
| `fix` | Bug修复 |
| `refactor` | 重构（不改变功能） |
| `docs` | 文档变更 |
| `test` | 测试相关 |
| `chore` | 构建/配置变更 |
| `perf` | 性能优化 |

示例：
- `feat(inventory): add InventoryBalance CRUD endpoints`
- `fix(outbound): correct AvailableQuantity calculation`
- `refactor(material): split MaterialDto into Create/Update DTOs`

---

## 6. 测试规范

### 6.1 测试分层

| 层级 | 项目位置 | 测试类型 | 覆盖目标 |
|------|---------|---------|---------|
| Domain | `{Module}.Tests/Domain/` | 单元测试 | 聚合根方法、值对象、领域服务 |
| Application | `{Module}.Tests/Application/` | 单元测试 | AppService、EventHandler、权限 |
| Integration | `Wms.IntegrationTests/` | 集成测试 | 跨模块流程、数据库操作 |
| Performance | `Wms.PerformanceTests/` | 性能测试 | 库存余额高频更新 |

### 6.2 测试命名

```csharp
// 测试类命名：{被测类}Tests
public class InventoryBalanceTests { }

// 测试方法命名：{方法名}_{场景}_{预期结果}
[Fact]
public void ApplyQuantityChange_PositiveDelta_IncreasesQuantity()
{
    // Arrange
    var balance = new InventoryBalance { Quantity = 100, ReservedQuantity = 10, FrozenQuantity = 5 };
    
    // Act
    balance.ApplyQuantityChange(50, 1, Guid.NewGuid());
    
    // Assert
    balance.Quantity.ShouldBe(150);
    balance.AvailableQuantity.ShouldBe(135);
}
```

### 6.3 测试覆盖目标

| 模块优先级 | 单元测试覆盖率目标 | 关键测试项 |
|-----------|------------------|-----------|
| P0 (Inventory) | ≥ 80% | 库存增减、冻结/解冻、预警、乐观锁重试 |
| P0 (Inbound/Outbound) | ≥ 75% | 状态机流转、分配策略、ERP回传 |
| P1 (Transfer/CycleCount) | ≥ 60% | 调拨流程、盘点差异处理 |
| P2 (Workflow/RuleEngine) | ≥ 50% | 审批流、规则执行 |

---

> **文档结束** — Phase 8 Coding Conventions v1.0
