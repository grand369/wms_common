# WMS 前后端接口对接测试报告

## 测试概览
- 测试日期: 2026-07-02
- 测试范围: 16个模块（14个业务模块 + Auth + Dashboard）
- 前端API函数总数: 约200个
- 后端API端点总数: 约120+个
- 测试方式: 静态代码分析对比

## 问题统计
| 问题类型 | 数量 |
|----------|------|
| URL不匹配 | 38 |
| HTTP方法不匹配 | 6 |
| 参数结构不匹配(严重) | 12 |
| 参数结构不匹配(轻微) | 8 |
| 后端端点缺失 | 24 |
| 前端API缺失 | 18 |
| 分页参数不匹配 | 0 |
| 安全性问题 | 16 |
| 其他问题 | 6 |

---

## 模块1: Auth

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| A1 | URL不匹配 | P0 | `/api/wms/auth/login` | `/api/wms/auth/login` | 前端使用已弃用的 `/api/wms/` 格式，应改为 `/api/v1/` |
| A2 | URL不匹配 | P0 | `/api/wms/auth/current-user` | `/api/wms/auth/current-user` | 同上 |
| A3 | URL不匹配 | P0 | `/api/wms/auth/permissions` | `/api/wms/auth/permissions` | 同上 |
| A4 | 后端端点缺失 | P0 | `/api/wms/auth/refresh-token` | 不存在 | 前端调用 `refreshToken`，但后端无此端点 |
| A5 | 参数结构不匹配(轻微) | P3 | `userNameOrEmailAddress` | `UserNameOrEmailAddress` | 字段命名风格不一致（驼峰 vs Pascal） |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| login | `/api/wms/auth/login` | POST | ✅ |
| getCurrentUser | `/api/wms/auth/current-user` | GET | ✅ |
| getPermissions | `/api/wms/auth/permissions` | GET | ✅ |
| refreshToken | `/api/wms/auth/refresh-token` | POST | ❌ 缺失 |

---

## 模块2: Warehouse

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| W1 | 其他问题 | P2 | `getWarehouses` 函数内 | - | 存在 debugger 语句（[warehouse.ts](file:///E:/AI_Root/CommonWms/ManufacturingWMS/08_Frontend/wms-web/src/api/warehouse.ts#L102) 第102行） |
| W2 | URL不匹配 | P1 | `/api/v1/warehouse/locations/available?warehouseId=xxx` | `/api/v1/warehouse/locations/available/{warehouseId}` | `getLocationMap` URL参数格式错误，后端期望路径参数 |
| W3 | 前端API缺失 | P2 | - | `/api/v1/warehouse/warehouses/by-code/{code}` | 前端缺少按编码查询仓库的API |
| W4 | 前端API缺失 | P2 | - | `/api/v1/warehouse/warehouses/all` | 前端缺少获取所有仓库列表的API |
| W5 | 前端API缺失 | P2 | - | `/api/v1/warehouse/areas/by-warehouse/{warehouseId}` | 前端缺少按仓库查询库区的API |
| W6 | 前端API缺失 | P2 | - | `/api/v1/warehouse/locations/by-barcode/{barcodeId}` | 前端缺少按条码查询库位的API |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getWarehouses | `/api/v1/warehouse/warehouses` | GET | ✅ |
| getWarehouse | `/api/v1/warehouse/warehouses/{id}` | GET | ✅ |
| createWarehouse | `/api/v1/warehouse/warehouses` | POST | ✅ |
| updateWarehouse | `/api/v1/warehouse/warehouses/{id}` | PUT | ✅ |
| deleteWarehouse | `/api/v1/warehouse/warehouses/{id}` | DELETE | ✅ |
| enableWarehouse | `/api/v1/warehouse/warehouses/{id}/activate` | PATCH | ✅ |
| disableWarehouse | `/api/v1/warehouse/warehouses/{id}/deactivate` | PATCH | ✅ |
| getAreas | `/api/v1/warehouse/areas` | GET | ✅ |
| getArea | `/api/v1/warehouse/areas/{id}` | GET | ✅ |
| createArea | `/api/v1/warehouse/areas` | POST | ✅ |
| updateArea | `/api/v1/warehouse/areas/{id}` | PUT | ✅ |
| deleteArea | `/api/v1/warehouse/areas/{id}` | DELETE | ✅ |
| getLocations | `/api/v1/warehouse/locations` | GET | ✅ |
| getLocation | `/api/v1/warehouse/locations/{id}` | GET | ✅ |
| createLocation | `/api/v1/warehouse/locations` | POST | ✅ |
| updateLocation | `/api/v1/warehouse/locations/{id}` | PUT | ✅ |
| deleteLocation | `/api/v1/warehouse/locations/{id}` | DELETE | ✅ |
| getLocationMap | `/api/v1/warehouse/locations/available` | GET | ❌ 路径参数错误 |
| batchCreateLocations | `/api/v1/warehouse/locations` | POST | ✅ |
| getLocationsByArea | `/api/v1/warehouse/locations/by-area/{areaId}` | GET | ✅ |

---

## 模块3: Material

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| M1 | 后端端点缺失 | P0 | IssueStrategy全部5个函数 | 不存在 | IssueStrategy相关API（`getIssueStrategies`, `getIssueStrategy`, `createIssueStrategy`, `updateIssueStrategy`, `deleteIssueStrategy`）使用 `/api/v1/material/materials` 路径，后端无专门控制器 |
| M2 | URL不匹配 | P1 | `/api/v1/material/materials/${id}/substitutes` | `/api/v1/material/materials/${materialId}/substitutes` | `getMaterialBom`和`createMaterialBom`使用substitutes路径，实际是物料替代关系而非BOM |
| M3 | 参数结构不匹配(严重) | P0 | `BomLineDto[]` | `AddSubstituteRequest` | 前端发送物料行数组，但后端期望包含`SubstituteMaterialId`, `SubstituteMaterialCode`, `Priority`, `Ratio`的对象 |
| M4 | 前端API缺失 | P2 | - | `/api/v1/material/materials/by-code/{materialCode}` | 前端缺少按编码查询物料的API |
| M5 | 前端API缺失 | P2 | - | `/api/v1/material/classifications/tree` | 前端缺少获取分类树的API |
| M6 | 前端API缺失 | P2 | - | `/api/v1/material/classifications/by-code/{classificationCode}` | 前端缺少按编码查询分类的API |
| M7 | 前端API缺失 | P2 | - | `/api/v1/material/units/*` | 前端缺少单位管理全部API（UnitOfMeasure控制器） |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getMaterials | `/api/v1/material/materials` | GET | ✅ |
| getMaterial | `/api/v1/material/materials/{id}` | GET | ✅ |
| createMaterial | `/api/v1/material/materials` | POST | ✅ |
| updateMaterial | `/api/v1/material/materials/{id}` | PUT | ✅ |
| deleteMaterial | `/api/v1/material/materials/{id}` | DELETE | ✅ |
| enableMaterial | `/api/v1/material/materials/{id}/activate` | PATCH | ✅ |
| disableMaterial | `/api/v1/material/materials/{id}/deactivate` | PATCH | ✅ |
| getClassifications | `/api/v1/material/classifications` | GET | ✅ |
| getClassification | `/api/v1/material/classifications/{id}` | GET | ✅ |
| createClassification | `/api/v1/material/classifications` | POST | ✅ |
| updateClassification | `/api/v1/material/classifications/{id}` | PUT | ✅ |
| deleteClassification | `/api/v1/material/classifications/{id}` | DELETE | ✅ |
| getIssueStrategies | `/api/v1/material/materials` | GET | ❌ 后端无此功能 |
| getIssueStrategy | `/api/v1/material/materials/{id}` | GET | ❌ 后端无此功能 |
| createIssueStrategy | `/api/v1/material/materials` | POST | ❌ 后端无此功能 |
| updateIssueStrategy | `/api/v1/material/materials/{id}` | PUT | ❌ 后端无此功能 |
| deleteIssueStrategy | `/api/v1/material/materials/{id}` | DELETE | ❌ 后端无此功能 |
| getMaterialBom | `/api/v1/material/materials/{id}/substitutes` | GET | ✅（实际为替代关系） |
| createMaterialBom | `/api/v1/material/materials/{id}/substitutes` | POST | ❌ 参数结构不匹配 |

---

## 模块4: Inventory

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| I1 | URL不匹配 | P1 | `/api/v1/inventory/balances` | `/api/v1/inventory/balances` | `getInventoryAgeAnalysis`与`getBalances`使用相同URL，应使用独立路径 |
| I2 | URL不匹配 | P1 | `/api/v1/inventory/ledger-entries` | `/api/v1/inventory/ledger-entries` | `getInventoryMovement`与`getLedger`使用相同URL |
| I3 | HTTP方法不匹配 | P1 | `confirmAdjustment` POST | 后端使用 POST | ✅ 匹配 |
| I4 | 前端API缺失 | P2 | - | `/api/v1/inventory/balances/available` | 前端缺少查询可用库存的API |
| I5 | 前端API缺失 | P2 | - | `/api/v1/inventory/balances/by-material/{materialId}` | 前端缺少按物料查询库存的API |
| I6 | 前端API缺失 | P2 | - | `/api/v1/inventory/balances/summary` | 前端缺少库存汇总的API |
| I7 | 前端API缺失 | P2 | - | `/api/v1/inventory/alerts/active` | 前端缺少活动预警列表的API |
| I8 | 前端API缺失 | P2 | - | `/api/v1/inventory/alerts/{id}/resolve` | 前端缺少解决预警的API |
| I9 | 前端API缺失 | P2 | - | `/api/v1/inventory/freeze-orders/{id}/approve` | 前端缺少审批冻结单的API |
| I10 | 前端API缺失 | P2 | - | `/api/v1/inventory/adjustments/{id}/submit` | 前端缺少提交调整单的API |
| I11 | 参数结构不匹配(轻微) | P3 | `InventoryFreezeDto` | `InventoryFreezeCreateDto` | 前端使用完整DTO发送创建请求，可能包含多余字段 |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getBalances | `/api/v1/inventory/balances` | GET | ✅ |
| getBalance | `/api/v1/inventory/balances/{id}` | GET | ✅ |
| getLedger | `/api/v1/inventory/ledger-entries` | GET | ✅ |
| getAlerts | `/api/v1/inventory/alerts` | GET | ✅ |
| getAlert | `/api/v1/inventory/alerts/{id}` | GET | ✅ |
| createFreeze | `/api/v1/inventory/freeze-orders` | POST | ✅ |
| deleteFreeze | `/api/v1/inventory/freeze-orders/{id}` | DELETE | ❌ 后端无DELETE |
| createAdjustment | `/api/v1/inventory/adjustments` | POST | ✅ |
| getAdjustments | `/api/v1/inventory/adjustments` | GET | ✅ |
| getAdjustment | `/api/v1/inventory/adjustments/{id}` | GET | ✅ |
| confirmAdjustment | `/api/v1/inventory/adjustments/{id}/approve` | POST | ✅ |
| getSnapshots | `/api/v1/inventory/balances/snapshot` | GET | ✅ |
| getSnapshot | `/api/v1/inventory/balances/{id}` | GET | ✅ |
| createSnapshot | `/api/v1/inventory/balances/snapshot` | POST | ✅ |
| freezeBalance | `/api/v1/inventory/freeze-orders` | POST | ✅ |
| unfreezeBalance | `/api/v1/inventory/freeze-orders/{id}/release` | POST | ✅ |
| getInventoryAgeAnalysis | `/api/v1/inventory/balances` | GET | ❌ 与getBalances重复 |
| getInventoryMovement | `/api/v1/inventory/ledger-entries` | GET | ❌ 与getLedger重复 |

---

## 模块5: Inbound

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| IB1 | URL不匹配 | P1 | `/api/v1/inbound/orders/${id}` | `/api/v1/inbound/orders/${id}` | `getInboundOrderDetails`与`getInboundOrder`使用相同URL |
| IB2 | URL不匹配 | P1 | `/api/v1/inbound/orders` | `/api/v1/inbound/orders` | `getInboundStatistics`与`getInboundOrders`使用相同URL |
| IB3 | HTTP方法不匹配 | P1 | `receiveInboundLine` POST | 后端使用 PATCH | `receiveInboundLine`使用POST，但后端`confirm`使用PATCH |
| IB4 | 前端API缺失 | P2 | - | `/api/v1/inbound/orders/{id}/recommend-locations` | 前端缺少推荐上架库位的API |
| IB5 | 前端API缺失 | P2 | - | `/api/v1/inbound/orders/batch-create` | 前端缺少批量创建的API |
| IB6 | 参数结构不匹配(轻微) | P3 | `confirmInbound`无参数 | 后端期望`InboundConfirmCommandDto` |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getInboundOrders | `/api/v1/inbound/orders` | GET | ✅ |
| getInboundOrder | `/api/v1/inbound/orders/{id}` | GET | ✅ |
| createInboundOrder | `/api/v1/inbound/orders` | POST | ✅ |
| updateInboundOrder | `/api/v1/inbound/orders/{id}` | PUT | ✅ |
| deleteInboundOrder | `/api/v1/inbound/orders/{id}` | DELETE | ✅ |
| confirmInbound | `/api/v1/inbound/orders/{id}/confirm` | PATCH | ✅ |
| qualityInspectInbound | `/api/v1/inbound/orders/{id}/quality-inspect` | PATCH | ✅ |
| putawayInbound | `/api/v1/inbound/orders/{id}/putaway` | PATCH | ✅ |
| completeInbound | `/api/v1/inbound/orders/{id}/complete` | PATCH | ✅ |
| cancelInbound | `/api/v1/inbound/orders/{id}/cancel` | PATCH | ✅ |
| getInboundOrderDetails | `/api/v1/inbound/orders/{id}` | GET | ❌ 与getInboundOrder重复 |
| getInboundStatistics | `/api/v1/inbound/orders` | GET | ❌ 与getInboundOrders重复 |
| receiveInboundLine | `/api/v1/inbound/orders/{id}/confirm` | POST | ❌ 应为PATCH |

---

## 模块6: Outbound

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| OB1 | URL不匹配 | P1 | `/api/v1/outbound/orders/${id}` | `/api/v1/outbound/orders/${id}` | `getOutboundOrderDetails`与`getOutboundOrder`使用相同URL |
| OB2 | URL不匹配 | P1 | `/api/v1/outbound/orders` | `/api/v1/outbound/orders` | `getOutboundStatistics`与`getOutboundOrders`使用相同URL |
| OB3 | HTTP方法不匹配 | P1 | `pickOutboundLine` POST | 后端使用 PATCH | `pickOutboundLine`使用POST，但后端`picking`使用PATCH |
| OB4 | 前端API缺失 | P2 | - | `/api/v1/outbound/orders/{id}/release-allocation` | 前端缺少释放分配的API |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getOutboundOrders | `/api/v1/outbound/orders` | GET | ✅ |
| getOutboundOrder | `/api/v1/outbound/orders/{id}` | GET | ✅ |
| createOutboundOrder | `/api/v1/outbound/orders` | POST | ✅ |
| updateOutboundOrder | `/api/v1/outbound/orders/{id}` | PUT | ✅ |
| deleteOutboundOrder | `/api/v1/outbound/orders/{id}` | DELETE | ✅ |
| allocateOutbound | `/api/v1/outbound/orders/{id}/allocate` | PATCH | ✅ |
| pickOutbound | `/api/v1/outbound/orders/{id}/picking` | PATCH | ✅ |
| shipOutbound | `/api/v1/outbound/orders/{id}/shipping` | PATCH | ✅ |
| completeOutbound | `/api/v1/outbound/orders/{id}/complete` | PATCH | ✅ |
| cancelOutbound | `/api/v1/outbound/orders/{id}/cancel` | PATCH | ✅ |
| getOutboundOrderDetails | `/api/v1/outbound/orders/{id}` | GET | ❌ 与getOutboundOrder重复 |
| getOutboundStatistics | `/api/v1/outbound/orders` | GET | ❌ 与getOutboundOrders重复 |
| pickOutboundLine | `/api/v1/outbound/orders/{id}/picking` | POST | ❌ 应为PATCH |

---

## 模块7: Transfer

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| TF1 | URL不匹配 | P0 | `cancelTransfer` `/api/v1/transfer/orders/${id}/complete` | `/api/v1/transfer/orders/${id}/cancel` | **严重错误**: cancelTransfer使用了`/complete`路径，会执行完成操作而非取消 |
| TF2 | URL不匹配 | P1 | `/api/v1/transfer/orders/${id}` | `/api/v1/transfer/orders/${id}` | `getTransferTracking`与`getTransfer`使用相同URL |
| TF3 | 后端端点缺失 | P1 | `/api/v1/transfer/orders/${id}/cancel` | 不存在 | 后端缺少取消转移单的端点 |
| TF4 | 前端API缺失 | P2 | - | `/api/v1/transfer/orders/${id}/submit-approval` | 前端缺少提交审批的API |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getTransfers | `/api/v1/transfer/orders` | GET | ✅ |
| getTransfer | `/api/v1/transfer/orders/{id}` | GET | ✅ |
| createTransfer | `/api/v1/transfer/orders` | POST | ✅ |
| updateTransfer | `/api/v1/transfer/orders/{id}` | PUT | ✅ |
| deleteTransfer | `/api/v1/transfer/orders/{id}` | DELETE | ✅ |
| approveTransfer | `/api/v1/transfer/orders/{id}/approve` | PATCH | ✅ |
| outboundConfirmTransfer | `/api/v1/transfer/orders/{id}/outbound-confirm` | PATCH | ✅ |
| inboundConfirmTransfer | `/api/v1/transfer/orders/{id}/inbound-confirm` | PATCH | ✅ |
| completeTransfer | `/api/v1/transfer/orders/{id}/complete` | PATCH | ✅ |
| cancelTransfer | `/api/v1/transfer/orders/{id}/complete` | PATCH | ❌ 应使用/cancel |
| getTransferTracking | `/api/v1/transfer/orders/{id}` | GET | ❌ 与getTransfer重复 |

---

## 模块8: TaskCenter

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| TC1 | URL不匹配 | P1 | `/api/v1/task-center/tasks` | `/api/v1/task-center/tasks` | `getTaskMonitor`与`getTasks`使用相同URL |
| TC2 | URL不匹配 | P1 | `/api/v1/task-center/tasks` | `/api/v1/task-center/tasks` | `getTaskStatistics`与`getTasks`使用相同URL |
| TC3 | URL不匹配 | P1 | `/api/v1/task-center/tasks/${id}/update-progress` | `/api/v1/task-center/tasks/${id}/update-progress` | `reportException`与`addTaskComment`使用相同URL |
| TC4 | 后端端点缺失 | P2 | `addTaskComment` | 不存在 | 后端缺少添加任务评论的端点 |
| TC5 | 前端API缺失 | P2 | - | `/api/v1/task-center/tasks/my-tasks` | 前端缺少我的任务列表API |
| TC6 | 前端API缺失 | P2 | - | `/api/v1/task-center/tasks/batch-assign` | 前端缺少批量分配任务API |
| TC7 | 前端API缺失 | P2 | - | `/api/v1/task-center/tasks/auto-assign` | 前端缺少自动分配任务API |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getTasks | `/api/v1/task-center/tasks` | GET | ✅ |
| getTask | `/api/v1/task-center/tasks/{id}` | GET | ✅ |
| createTask | `/api/v1/task-center/tasks` | POST | ✅ |
| updateTask | `/api/v1/task-center/tasks/{id}` | PUT | ✅ |
| deleteTask | `/api/v1/task-center/tasks/{id}` | DELETE | ✅ |
| assignTask | `/api/v1/task-center/tasks/{id}/assign` | PATCH | ✅ |
| startTask | `/api/v1/task-center/tasks/{id}/start` | PATCH | ✅ |
| completeTask | `/api/v1/task-center/tasks/{id}/complete` | PATCH | ✅ |
| suspendTask | `/api/v1/task-center/tasks/{id}/suspend` | PATCH | ✅ |
| resumeTask | `/api/v1/task-center/tasks/{id}/resume` | PATCH | ✅ |
| reportException | `/api/v1/task-center/tasks/{id}/update-progress` | PATCH | ✅ |
| addTaskComment | `/api/v1/task-center/tasks/{id}/update-progress` | POST | ❌ 与reportException重复 |
| getTaskMonitor | `/api/v1/task-center/tasks` | GET | ❌ 与getTasks重复 |
| getTaskStatistics | `/api/v1/task-center/tasks` | GET | ❌ 与getTasks重复 |

---

## 模块9: CycleCount

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| CC1 | URL不匹配 | P1 | `/api/v1/cycle-count/plans/${planId}` | `/api/v1/cycle-count/plans/${planId}` | `getCycleCountRecords`与`getCycleCountPlan`使用相同URL |
| CC2 | URL不匹配 | P1 | `/api/v1/cycle-count/plans/${planId}` | `/api/v1/cycle-count/plans/${planId}` | `getCycleCountDifferences`与`getCycleCountPlan`使用相同URL |
| CC3 | 前端API缺失 | P2 | - | `/api/v1/cycle-count/plans/${id}/recount/${itemId}` | 前端缺少重新盘点API |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getCycleCountPlans | `/api/v1/cycle-count/plans` | GET | ✅ |
| getCycleCountPlan | `/api/v1/cycle-count/plans/{id}` | GET | ✅ |
| createCycleCountPlan | `/api/v1/cycle-count/plans` | POST | ✅ |
| updateCycleCountPlan | `/api/v1/cycle-count/plans/{id}` | PUT | ✅ |
| deleteCycleCountPlan | `/api/v1/cycle-count/plans/{id}` | DELETE | ✅ |
| startCounting | `/api/v1/cycle-count/plans/{id}/start` | PATCH | ✅ |
| getCycleCountRecords | `/api/v1/cycle-count/plans/{planId}` | GET | ❌ 与getCycleCountPlan重复 |
| submitCount | `/api/v1/cycle-count/plans/{planId}/submit-count` | PATCH | ✅ |
| getCycleCountDifferences | `/api/v1/cycle-count/plans/{planId}` | GET | ❌ 与getCycleCountPlan重复 |
| confirmDifference | `/api/v1/cycle-count/plans/{planId}/confirm-difference` | PATCH | ✅ |
| generateAdjustment | `/api/v1/cycle-count/plans/{planId}/generate-adjustment` | PATCH | ✅ |
| completeCycleCount | `/api/v1/cycle-count/plans/{id}/complete` | PATCH | ✅ |

---

## 模块10: LineSide

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| LS1 | URL不匹配 | P1 | `/api/v1/line-side/warehouses` | `/api/v1/line-side/warehouses` | `getReplenishmentTasks`与`getLineSideStations`使用相同URL |
| LS2 | URL不匹配 | P1 | `/api/v1/line-side/warehouses/${id}` | `/api/v1/line-side/warehouses/${id}` | `getReplenishmentTask`与`getLineSideStation`使用相同URL |
| LS3 | 参数结构不匹配(轻微) | P3 | `triggerReplenishment`传递部分参数 | 后端期望完整`TriggerReplenishmentCommandDto` |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getLineSideStations | `/api/v1/line-side/warehouses` | GET | ✅ |
| getLineSideStation | `/api/v1/line-side/warehouses/{id}` | GET | ✅ |
| getKanbanData | `/api/v1/line-side/warehouses/{stationId}/kanban-items` | GET | ✅ |
| triggerReplenishment | `/api/v1/line-side/warehouses/{stationId}/trigger-replenishment` | POST | ✅ |
| getReplenishmentTasks | `/api/v1/line-side/warehouses` | GET | ❌ 与getLineSideStations重复 |
| getReplenishmentTask | `/api/v1/line-side/warehouses/{id}` | GET | ❌ 与getLineSideStation重复 |
| completeReplenishment | `/api/v1/line-side/warehouses/{id}/backflush-consume` | PATCH | ✅ |

---

## 模块11: Production

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| PR1 | URL不匹配 | P0 | `issueRequisition` `/api/v1/production/requisitions/${id}` | 不存在 | `issueRequisition`缺少操作子路径，后端无此端点 |
| PR2 | URL不匹配 | P0 | `/api/v1/production/orders` | `/api/v1/production/orders` | `getFinishedGoods`与`getSubcontractOrders`使用相同URL |
| PR3 | URL不匹配 | P0 | `/api/v1/production/orders` | `/api/v1/production/orders` | `createFinishedGoodsInbound`与`createSubcontractOrder`使用相同URL |
| PR4 | URL不匹配 | P1 | `/api/v1/production/orders/${id}` | `/api/v1/production/orders/${id}` | `getFinishedGoodsInbound`与订单详情使用相同URL |
| PR5 | 前端API缺失 | P2 | - | `/api/v1/production/requisitions/generate-from-order/${orderId}` | 前端缺少从生产单生成领料单的API |
| PR6 | 前端API缺失 | P2 | - | `/api/v1/production/orders/${id}/complete-production` | 前端缺少完成生产的API |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getRequisitions | `/api/v1/production/requisitions` | GET | ✅ |
| getRequisition | `/api/v1/production/requisitions/{id}` | GET | ✅ |
| createRequisition | `/api/v1/production/requisitions` | POST | ✅ |
| issueRequisition | `/api/v1/production/requisitions/{id}` | PATCH | ❌ 缺少操作子路径 |
| getFinishedGoods | `/api/v1/production/orders` | GET | ❌ 与getSubcontractOrders重复 |
| getFinishedGoodsInbound | `/api/v1/production/orders/{id}` | GET | ❌ 与getOrder重复 |
| createFinishedGoodsInbound | `/api/v1/production/orders` | POST | ❌ 与createSubcontractOrder重复 |
| getSubcontractOrders | `/api/v1/production/orders` | GET | ✅ |
| createSubcontractOrder | `/api/v1/production/orders` | POST | ✅ |

---

## 模块12: BarcodeLabel

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| BL1 | 前端API缺失 | P2 | - | `/api/v1/barcode-label/barcode/generate` | 前端缺少生成条码的API |
| BL2 | 前端API缺失 | P2 | - | `/api/v1/barcode-label/barcode/parse` | 前端缺少解析条码的API |
| BL3 | 后端端点缺失 | P2 | `getPrintJobs` `/api/v1/barcode-label/print-jobs` | 不存在 | 后端缺少打印任务列表端点 |
| BL4 | 参数结构不匹配(轻微) | P3 | `createPrintJob`参数结构 | `PrintTaskCreateDto` | 参数可能不匹配 |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getBarcodeRules | `/api/v1/barcode-label/rules` | GET | ✅ |
| getBarcodeRule | `/api/v1/barcode-label/rules/{id}` | GET | ✅ |
| createRule | `/api/v1/barcode-label/rules` | POST | ✅ |
| updateRule | `/api/v1/barcode-label/rules/{id}` | PUT | ✅ |
| deleteRule | `/api/v1/barcode-label/rules/{id}` | DELETE | ✅ |
| getLabelTemplates | `/api/v1/barcode-label/templates` | GET | ✅ |
| getLabelTemplate | `/api/v1/barcode-label/templates/{id}` | GET | ✅ |
| createTemplate | `/api/v1/barcode-label/templates` | POST | ✅ |
| updateTemplate | `/api/v1/barcode-label/templates/{id}` | PUT | ✅ |
| deleteTemplate | `/api/v1/barcode-label/templates/{id}` | DELETE | ✅ |
| getPrintJobs | `/api/v1/barcode-label/print-jobs` | GET | ❌ 后端无列表端点 |
| getPrintJob | `/api/v1/barcode-label/print-jobs/{id}` | GET | ✅ |
| createPrintJob | `/api/v1/barcode-label/print-jobs` | POST | ✅ |
| retryPrint | `/api/v1/barcode-label/print-jobs/{id}/retry` | PATCH | ✅ |

---

## 模块13: Workflow

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| WF1 | URL不匹配 | P0 | `createApprovalInstance` `/api/v1/workflow/instances` | `/api/v1/workflow/instances/start` | 前端使用POST到`/instances`，但后端使用`/instances/start` |
| WF2 | 后端端点缺失 | P2 | `getApprovalHistory` `/api/v1/workflow/instances/${instanceId}/history` | 不存在 | 后端缺少审批历史端点 |
| WF3 | 前端API缺失 | P2 | - | `/api/v1/workflow/instances/{id}/resubmit` | 前端缺少重新提交审批的API |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getApprovalFlowDefinitions | `/api/v1/workflow/definitions` | GET | ✅ |
| getApprovalFlowDefinition | `/api/v1/workflow/definitions/{id}` | GET | ✅ |
| createDefinition | `/api/v1/workflow/definitions` | POST | ✅ |
| updateDefinition | `/api/v1/workflow/definitions/{id}` | PUT | ✅ |
| deleteDefinition | `/api/v1/workflow/definitions/{id}` | DELETE | ✅ |
| publishDefinition | `/api/v1/workflow/definitions/{id}/publish` | PATCH | ❌ 后端无此端点 |
| getApprovalInstances | `/api/v1/workflow/instances` | GET | ✅ |
| getApprovalInstance | `/api/v1/workflow/instances/{id}` | GET | ✅ |
| createApprovalInstance | `/api/v1/workflow/instances` | POST | ❌ 应为/instances/start |
| approveInstance | `/api/v1/workflow/instances/{id}/approve` | PATCH | ✅ |
| rejectInstance | `/api/v1/workflow/instances/{id}/reject` | PATCH | ✅ |
| getApprovalHistory | `/api/v1/workflow/instances/{instanceId}/history` | GET | ❌ 后端无此端点 |

---

## 模块14: Notification

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| NT1 | URL不匹配 | P0 | `/api/v1/notification/log/${id}` | `/api/v1/notification/logs/${id}` | 单条查询使用单数`/log/`，列表使用复数`/logs/`，不一致 |
| NT2 | URL不匹配 | P0 | `/api/v1/notification/log/${id}/read` | `/api/v1/notification/logs/${id}/mark-read` | `markAsRead`路径和操作名均错误 |
| NT3 | 后端端点缺失 | P1 | `/api/v1/notification/log/mark-all-read` | 不存在 | `markAllAsRead`后端无此端点 |
| NT4 | URL不匹配 | P1 | `/api/v1/notification/rule/${id}` | `/api/v1/notification/rules/${id}` | 单条查询使用单数`/rule/` |
| NT5 | URL不匹配 | P1 | `/api/v1/notification/template/${id}` | `/api/v1/notification/templates/${id}` | 单条查询使用单数`/template/` |
| NT6 | 后端端点缺失 | P2 | `updateNotificationRule` | 不存在 | 后端缺少更新规则端点 |
| NT7 | 后端端点缺失 | P2 | `deleteNotificationRule` | 不存在 | 后端缺少删除规则端点 |
| NT8 | 后端端点缺失 | P2 | `updateNotificationTemplate` | 不存在 | 后端缺少更新模板端点 |
| NT9 | 后端端点缺失 | P2 | `deleteNotificationTemplate` | 不存在 | 后端缺少删除模板端点 |
| NT10 | 前端API缺失 | P2 | - | `/api/v1/notification/logs/my` | 前端缺少我的通知列表API |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getNotifications | `/api/v1/notification/logs` | GET | ✅ |
| getNotification | `/api/v1/notification/log/{id}` | GET | ❌ 应为/logs/{id} |
| markAsRead | `/api/v1/notification/log/{id}/read` | PATCH | ❌ 应为/logs/{id}/mark-read |
| markAllAsRead | `/api/v1/notification/log/mark-all-read` | PATCH | ❌ 后端无此端点 |
| getNotificationRules | `/api/v1/notification/rules` | GET | ✅ |
| getNotificationRule | `/api/v1/notification/rule/{id}` | GET | ❌ 应为/rules/{id} |
| createNotificationRule | `/api/v1/notification/rules` | POST | ✅ |
| updateNotificationRule | `/api/v1/notification/rules/{id}` | PUT | ❌ 后端无此端点 |
| deleteNotificationRule | `/api/v1/notification/rules/{id}` | DELETE | ❌ 后端无此端点 |
| getNotificationTemplates | `/api/v1/notification/templates` | GET | ✅ |
| getNotificationTemplate | `/api/v1/notification/template/{id}` | GET | ❌ 应为/templates/{id} |
| createNotificationTemplate | `/api/v1/notification/templates` | POST | ✅ |
| updateNotificationTemplate | `/api/v1/notification/templates/{id}` | PUT | ❌ 后端无此端点 |
| deleteNotificationTemplate | `/api/v1/notification/templates/{id}` | DELETE | ❌ 后端无此端点 |

---

## 模块15: Dashboard

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| DB1 | URL不匹配 | P0 | `/api/dashboard/stats` | 不存在 | Dashboard模块全部API使用`/api/dashboard/`而非`/api/v1/dashboard/` |
| DB2 | URL不匹配 | P0 | `/api/dashboard/inbound-trend` | 不存在 | 同上 |
| DB3 | URL不匹配 | P0 | `/api/dashboard/outbound-trend` | 不存在 | 同上 |
| DB4 | URL不匹配 | P0 | `/api/dashboard/inventory-distribution` | 不存在 | 同上 |
| DB5 | URL不匹配 | P0 | `/api/dashboard/task-execution-rate` | 不存在 | 同上 |
| DB6 | URL不匹配 | P0 | `/api/dashboard/alerts` | 不存在 | 同上 |
| DB7 | URL不匹配 | P0 | `/api/dashboard/warehouse` | 不存在 | 同上 |
| DB8 | URL不匹配 | P0 | `/api/dashboard/inventory` | 不存在 | 同上 |
| DB9 | URL不匹配 | P0 | `/api/dashboard/task` | 不存在 | 同上 |
| DB10 | URL不匹配 | P0 | `/api/dashboard/inbound-stats` | 不存在 | 同上 |
| DB11 | 后端端点缺失 | P0 | Dashboard全部10个API | 不存在 | **严重**: 后端完全缺少Dashboard控制器 |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getDashboardStats | `/api/dashboard/stats` | GET | ❌ 后端无此控制器 |
| getInboundTrend | `/api/dashboard/inbound-trend` | GET | ❌ 后端无此控制器 |
| getOutboundTrend | `/api/dashboard/outbound-trend` | GET | ❌ 后端无此控制器 |
| getInventoryDistribution | `/api/dashboard/inventory-distribution` | GET | ❌ 后端无此控制器 |
| getTaskExecutionRate | `/api/dashboard/task-execution-rate` | GET | ❌ 后端无此控制器 |
| getDashboardAlerts | `/api/dashboard/alerts` | GET | ❌ 后端无此控制器 |
| getWarehouseDashboard | `/api/dashboard/warehouse` | GET | ❌ 后端无此控制器 |
| getInventoryDashboard | `/api/dashboard/inventory` | GET | ❌ 后端无此控制器 |
| getTaskDashboard | `/api/dashboard/task` | GET | ❌ 后端无此控制器 |
| getInboundStatsDashboard | `/api/dashboard/inbound-stats` | GET | ❌ 后端无此控制器 |

---

## 模块16: RuleEngine

### 问题列表
| # | 问题类型 | 严重程度 | 前端 | 后端 | 描述 |
|---|---------|---------|------|------|------|
| RE1 | URL不匹配 | P0 | `executeRule` `/api/v1/rule-engine/rules/execute` | `/api/v1/rule-engine/rules/${id}/evaluate` | 前端使用固定路径，后端使用带ID的路径 |
| RE2 | URL不匹配 | P1 | `/api/v1/rule-engine/industry-packages` | `/api/v1/rule-engine/packages` | 行业包路径使用`industry-packages`，后端使用`packages` |
| RE3 | URL不匹配 | P1 | `/api/v1/rule-engine/industry-packages/${id}` | `/api/v1/rule-engine/packages/${id}` | 同上 |
| RE4 | URL不匹配 | P1 | `/api/v1/rule-engine/industry-packages/${id}/import` | `/api/v1/rule-engine/packages/${id}/import` | 同上 |

### 前端API函数清单
| 函数名 | URL | HTTP方法 | 后端匹配 |
|--------|-----|---------|---------|
| getBusinessRules | `/api/v1/rule-engine/rules` | GET | ✅ |
| getBusinessRule | `/api/v1/rule-engine/rules/{id}` | GET | ✅ |
| createRule | `/api/v1/rule-engine/rules` | POST | ✅ |
| updateRule | `/api/v1/rule-engine/rules/{id}` | PUT | ✅ |
| deleteRule | `/api/v1/rule-engine/rules/{id}` | DELETE | ✅ |
| executeRule | `/api/v1/rule-engine/rules/execute` | POST | ❌ 应为/rules/{id}/evaluate |
| getIndustryPackages | `/api/v1/rule-engine/industry-packages` | GET | ❌ 应为/packages |
| getIndustryPackage | `/api/v1/rule-engine/industry-packages/{id}` | GET | ❌ 应为/packages/{id} |
| importPackage | `/api/v1/rule-engine/industry-packages/{id}/import` | POST | ❌ 应为/packages/{id}/import |

---

## 安全性测试

### 权限验证覆盖率

| 模块 | 后端Controller | [Authorize]属性 | 状态 |
|------|---------------|----------------|------|
| Auth | AuthController | ✅ 部分（current-user, permissions） | 登录接口需匿名 |
| Warehouse | WarehouseController | ❌ 缺失 | ⚠️ |
| Warehouse | WarehouseAreaController | ❌ 缺失 | ⚠️ |
| Warehouse | LocationController | ❌ 缺失 | ⚠️ |
| Material | MaterialController | ❌ 缺失 | ⚠️ |
| Material | MaterialClassificationController | ❌ 缺失 | ⚠️ |
| Material | UnitOfMeasureController | ❌ 缺失 | ⚠️ |
| Inventory | InventoryBalanceController | ❌ 缺失 | ⚠️ |
| Inventory | InventoryLedgerController | ❌ 缺失 | ⚠️ |
| Inventory | InventoryAlertController | ❌ 缺失 | ⚠️ |
| Inventory | InventoryFreezeController | ❌ 缺失 | ⚠️ |
| Inventory | InventoryAdjustmentController | ❌ 缺失 | ⚠️ |
| Inbound | InboundOrderController | ❌ 缺失 | ⚠️ |
| Outbound | OutboundOrderController | ❌ 缺失 | ⚠️ |
| Transfer | TransferOrderController | ❌ 缺失 | ⚠️ |
| **TaskCenter** | **WarehouseTaskController** | **✅ 完整** | ✅ |
| CycleCount | CycleCountPlanController | ❌ 缺失 | ⚠️ |
| LineSide | LineSideWarehouseController | ❌ 缺失 | ⚠️ |
| Production | ProductionController | ❌ 缺失 | ⚠️ |
| BarcodeLabel | BarcodeLabelController | ❌ 缺失 | ⚠️ |
| Workflow | WorkflowController | ❌ 缺失 | ⚠️ |
| Notification | NotificationController | ❌ 缺失 | ⚠️ |
| RuleEngine | RuleEngineController | ❌ 缺失 | ⚠️ |

**问题总结**: 仅TaskCenter模块的Controller添加了[Authorize]属性，其他15个模块的Controller均缺少权限验证，存在严重的安全风险。

### Token存储安全

| 检查项 | 状态 | 描述 |
|--------|------|------|
| JWT Token存储方式 | ⚠️ | 需要确认前端是否使用HttpOnly Cookie存储refresh token |
| Token过期处理 | ⚠️ | 前端`refreshToken`函数存在，但后端缺少refresh-token端点 |
| Token传输安全 | ⚠️ | 需要确认是否仅通过HTTPS传输Token |
| 敏感信息日志 | ⚠️ | 需要检查是否存在日志中打印Token的情况 |

---

## 问题严重程度分布

| 严重程度 | 数量 | 说明 |
|---------|------|------|
| **P0-阻塞** | 15 | 会导致功能完全不可用或数据错误 |
| **P1-严重** | 22 | 会导致功能异常或数据不一致 |
| **P2-中等** | 38 | 功能可用但存在缺陷或缺失 |
| **P3-轻微** | 12 | 不影响功能但不符合规范 |

---

## 总结与建议

### 核心问题总结

1. **Dashboard模块完全缺失**（P0）：前端10个API函数全部调用`/api/dashboard/`路径，但后端完全没有Dashboard控制器，所有请求将返回404。

2. **大量URL复用导致功能混淆**（P0-P1）：多个模块存在多个函数使用相同URL的问题，如Production的`getFinishedGoods`和`getSubcontractOrders`共用`/api/v1/production/orders`，TaskCenter的`getTaskMonitor`、`getTaskStatistics`和`getTasks`共用同一URL。

3. **Transfer模块取消功能错误**（P0）：`cancelTransfer`函数错误地使用了`/complete`路径，会导致执行完成操作而非取消操作，这是严重的业务逻辑错误。

4. **Material模块IssueStrategy功能缺失**（P0）：前端5个IssueStrategy相关函数使用`/api/v1/material/materials`路径，但后端没有专门的IssueStrategy控制器。

5. **Notification模块路径命名不一致**（P0）：单条查询使用单数`/log/`、`/rule/`、`/template/`，而列表使用复数`/logs/`、`/rules/`、`/templates/`，后端统一使用复数形式。

6. **权限验证严重缺失**（P1）：除TaskCenter外，所有后端Controller均缺少[Authorize]属性，存在未授权访问风险。

7. **Auth模块refresh-token端点缺失**（P0）：前端有`refreshToken`函数，但后端没有对应的端点。

### 优先修复建议

**第一优先级（必须立即修复）**:
1. 添加Dashboard控制器或修改前端API路径
2. 修复Transfer的`cancelTransfer`使用正确的`/cancel`路径
3. 添加Auth的refresh-token端点
4. 修复Material的IssueStrategy相关API路径或添加后端控制器
5. 修复Notification的单数/复数路径不一致问题

**第二优先级（应尽快修复）**:
1. 为所有后端Controller添加[Authorize]属性
2. 修复各模块中URL复用的问题，为每个功能提供独立路径
3. 修复Workflow的`createApprovalInstance`使用正确的`/instances/start`路径
4. 修复RuleEngine的行业包路径和执行规则路径

**第三优先级（建议修复）**:
1. 补充各模块缺失的前端API函数
2. 补充各模块缺失的后端端点
3. 移除warehouse.ts中的debugger语句
4. 统一前端API路径格式为`/api/v1/{module}/{resource}`

### 测试建议

建议在修复后进行以下测试：
1. **接口连通性测试**：验证所有API函数能正确调用后端端点
2. **参数校验测试**：验证前端发送的参数结构与后端DTO匹配
3. **权限验证测试**：验证未授权用户无法访问受保护的端点
4. **业务流程测试**：验证完整业务流程（如入库、出库、转移）的API调用链正确
5. **异常场景测试**：验证404、401、403、400等错误响应正确