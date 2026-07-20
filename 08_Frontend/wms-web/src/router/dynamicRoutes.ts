import type { RouteRecordRaw } from 'vue-router';
import router from './index';
import { usePermissionStore } from '@/stores/permission';
import { useAuthStore } from '@/stores/auth';
import { getPermissions } from '@/api/auth';

export interface WmsRouteMeta {
  title: string;
  icon: string;
  permission?: string;
  hidden?: boolean;
  keepAlive?: boolean;
  activeMenu?: string;
  [key: string | number | symbol]: any;
}

declare module 'vue-router' {
  interface RouteMeta extends WmsRouteMeta {}
}

function createRoute(
  path: string,
  name: string,
  component: () => Promise<any>,
  meta: WmsRouteMeta
): RouteRecordRaw {
  return { path, name, component, meta };
}

/**
 * All 45+ module routes — organized by the 14 BC modules.
 * Paths are relative (no leading /) because they are added as children of DefaultLayout.
 * Imports use kebab-case directories matching Phase7 route conventions.
 */
export const asyncRoutes: RouteRecordRaw[] = [
  // ========================
  // BC-01 Warehouse (5 pages)
  // ========================
  createRoute('warehouse/list', 'WarehouseList',
    () => import('@/views/warehouse/List.vue'),
    { title: '仓库列表', icon: 'OfficeBuilding', permission: 'Wms.Warehouse.Read' }),
  createRoute('warehouse/detail/:id', 'WarehouseDetail',
    () => import('@/views/warehouse/Detail.vue'),
    { title: '仓库详情', icon: 'OfficeBuilding', permission: 'Wms.Warehouse.Read', hidden: true }),
  createRoute('warehouse/areas', 'WarehouseAreas',
    () => import('@/views/warehouse/Areas.vue'),
    { title: '库区管理', icon: 'OfficeBuilding', permission: 'Wms.Warehouse.Read' }),
  createRoute('warehouse/locations', 'WarehouseLocations',
    () => import('@/views/warehouse/Locations.vue'),
    { title: '库位管理', icon: 'OfficeBuilding', permission: 'Wms.Warehouse.Read' }),
  createRoute('warehouse/location-map/:id?', 'WarehouseLocationMap',
    () => import('@/views/warehouse/LocationMap.vue'),
    { title: '库位地图', icon: 'OfficeBuilding', permission: 'Wms.Warehouse.Read' }),

  // ========================
  // BC-02 Material (4 pages)
  // ========================
  createRoute('material/list', 'MaterialList',
    () => import('@/views/material/List.vue'),
    { title: '物料列表', icon: 'Goods', permission: 'Wms.Material.Read' }),
  createRoute('material/detail/:id', 'MaterialDetail',
    () => import('@/views/material/Detail.vue'),
    { title: '物料详情', icon: 'Goods', permission: 'Wms.Material.Read', hidden: true }),
  createRoute('material/classifications', 'MaterialClassifications',
    () => import('@/views/material/Classifications.vue'),
    { title: '物料分类', icon: 'Goods', permission: 'Wms.Material.Read' }),
  createRoute('material/issue-strategies', 'MaterialIssueStrategies',
    () => import('@/views/material/IssueStrategies.vue'),
    { title: '发料策略', icon: 'Goods', permission: 'Wms.Material.Update' }),

  // ========================
  // BC-03 Inventory (7 pages)
  // ========================
  createRoute('inventory/balances', 'InventoryBalances',
    () => import('@/views/inventory/Balances.vue'),
    { title: '库存余额', icon: 'DataBoard', permission: 'Wms.Inventory.Read' }),
  createRoute('inventory/balance-detail/:id', 'InventoryBalanceDetail',
    () => import('@/views/inventory/BalanceDetail.vue'),
    { title: '库存明细', icon: 'DataBoard', permission: 'Wms.Inventory.Read', hidden: true }),
  createRoute('inventory/ledger', 'InventoryLedger',
    () => import('@/views/inventory/Ledger.vue'),
    { title: '库存台账', icon: 'DataBoard', permission: 'Wms.Inventory.Read' }),
  createRoute('inventory/alerts', 'InventoryAlerts',
    () => import('@/views/inventory/Alerts.vue'),
    { title: '库存预警', icon: 'DataBoard', permission: 'Wms.Inventory.Read' }),
  createRoute('inventory/freeze', 'InventoryFreeze',
    () => import('@/views/inventory/Freeze.vue'),
    { title: '冻结/解冻', icon: 'DataBoard', permission: 'Wms.Inventory.Freeze.Create' }),
  createRoute('inventory/adjustments', 'InventoryAdjustments',
    () => import('@/views/inventory/Adjustments.vue'),
    { title: '库存调整', icon: 'DataBoard', permission: 'Wms.Inventory.Adjust.Create' }),
  createRoute('inventory/snapshots', 'InventorySnapshots',
    () => import('@/views/inventory/Snapshots.vue'),
    { title: '库存快照', icon: 'DataBoard', permission: 'Wms.Inventory.Snapshot' }),

  // ========================
  // BC-04 Inbound (4 pages)
  // ========================
  createRoute('inbound/list', 'InboundList',
    () => import('@/views/inbound/List.vue'),
    { title: '入库单列表', icon: 'Download', permission: 'Wms.Inbound.Read' }),
  createRoute('inbound/create', 'InboundCreate',
    () => import('@/views/inbound/Create.vue'),
    { title: '创建入库单', icon: 'Download', permission: 'Wms.Inbound.Create' }),
  createRoute('inbound/detail/:id', 'InboundDetail',
    () => import('@/views/inbound/Detail.vue'),
    { title: '入库单详情', icon: 'Download', permission: 'Wms.Inbound.Read', hidden: true }),
  createRoute('inbound/statistics', 'InboundStatistics',
    () => import('@/views/inbound/Statistics.vue'),
    { title: '入库统计', icon: 'Download', permission: 'Wms.Inbound.Read' }),

  // ========================
  // BC-05 Outbound (4 pages)
  // ========================
  createRoute('outbound/list', 'OutboundList',
    () => import('@/views/outbound/List.vue'),
    { title: '出库单列表', icon: 'Upload', permission: 'Wms.Outbound.Read' }),
  createRoute('outbound/create', 'OutboundCreate',
    () => import('@/views/outbound/Create.vue'),
    { title: '创建出库单', icon: 'Upload', permission: 'Wms.Outbound.Create' }),
  createRoute('outbound/detail/:id', 'OutboundDetail',
    () => import('@/views/outbound/Detail.vue'),
    { title: '出库单详情', icon: 'Upload', permission: 'Wms.Outbound.Read', hidden: true }),
  createRoute('outbound/statistics', 'OutboundStatistics',
    () => import('@/views/outbound/Statistics.vue'),
    { title: '出库统计', icon: 'Upload', permission: 'Wms.Outbound.Read' }),

  // ========================
  // BC-10 TaskCenter (3 pages)
  // ========================
  createRoute('task-center/list', 'TaskCenterList',
    () => import('@/views/task-center/List.vue'),
    { title: '任务列表', icon: 'List', permission: 'Wms.TaskCenter.Read' }),
  createRoute('task-center/detail/:id', 'TaskCenterDetail',
    () => import('@/views/task-center/Detail.vue'),
    { title: '任务详情', icon: 'List', permission: 'Wms.TaskCenter.Read', hidden: true }),
  createRoute('task-center/monitor', 'TaskCenterMonitor',
    () => import('@/views/task-center/Monitor.vue'),
    { title: '任务监控', icon: 'List', permission: 'Wms.TaskCenter.Assign' }),

  // ========================
  // BC-06 Transfer (4 pages)
  // ========================
  createRoute('transfer/list', 'TransferList',
    () => import('@/views/transfer/List.vue'),
    { title: '调拨单列表', icon: 'Switch', permission: 'Wms.Transfer.Read' }),
  createRoute('transfer/detail/:id', 'TransferDetail',
    () => import('@/views/transfer/Detail.vue'),
    { title: '调拨单详情', icon: 'Switch', permission: 'Wms.Transfer.Read', hidden: true }),
  createRoute('transfer/create', 'TransferCreate',
    () => import('@/views/transfer/Create.vue'),
    { title: '创建调拨单', icon: 'Switch', permission: 'Wms.Transfer.Create' }),
  createRoute('transfer/tracking', 'TransferTracking',
    () => import('@/views/transfer/Tracking.vue'),
    { title: '在途跟踪', icon: 'Switch', permission: 'Wms.Transfer.Read' }),

  // ========================
  // BC-07 CycleCount (3 pages)
  // ========================
  createRoute('cycle-count/plans', 'CycleCountPlans',
    () => import('@/views/cycle-count/Plans.vue'),
    { title: '盘点计划', icon: 'Finished', permission: 'Wms.CycleCount.Read' }),
  createRoute('cycle-count/execute/:id', 'CycleCountExecute',
    () => import('@/views/cycle-count/Execute.vue'),
    { title: '盘点执行', icon: 'Finished', permission: 'Wms.CycleCount.Execute', hidden: true }),
  createRoute('cycle-count/difference/:id', 'CycleCountDifference',
    () => import('@/views/cycle-count/Difference.vue'),
    { title: '差异处理', icon: 'Finished', permission: 'Wms.CycleCount.Confirm', hidden: true }),

  // ========================
  // BC-08 LineSide (3 pages)
  // ========================
  createRoute('line-side/overview', 'LineSideOverview',
    () => import('@/views/line-side/Overview.vue'),
    { title: '线边仓概览', icon: 'SetUp', permission: 'Wms.LineSide.Read' }),
  createRoute('line-side/kanban/:id?', 'LineSideKanban',
    () => import('@/views/line-side/Kanban.vue'),
    { title: '看板页', icon: 'SetUp', permission: 'Wms.LineSide.Read' }),
  createRoute('line-side/replenishment', 'LineSideReplenishment',
    () => import('@/views/line-side/Replenishment.vue'),
    { title: '补料任务', icon: 'SetUp', permission: 'Wms.LineSide.Replenish' }),

  // ========================
  // BC-09 Production (3 pages)
  // ========================
  createRoute('production/requisitions', 'ProductionRequisitions',
    () => import('@/views/production/Requisitions.vue'),
    { title: '领料单', icon: 'Promotion', permission: 'Wms.Production.Read' }),
  createRoute('production/finished-goods', 'ProductionFinishedGoods',
    () => import('@/views/production/FinishedGoods.vue'),
    { title: '成品入库', icon: 'Promotion', permission: 'Wms.Production.Complete' }),
  createRoute('production/subcontract', 'ProductionSubcontract',
    () => import('@/views/production/Subcontract.vue'),
    { title: '委外追踪', icon: 'Promotion', permission: 'Wms.Production.Read' }),

  // ========================
  // BC-11 BarcodeLabel (3 pages)
  // ========================
  createRoute('barcode-label/rules', 'BarcodeLabelRules',
    () => import('@/views/barcode-label/Rules.vue'),
    { title: '条码规则', icon: 'Ticket', permission: 'Wms.BarcodeLabel.Read' }),
  createRoute('barcode-label/templates', 'BarcodeLabelTemplates',
    () => import('@/views/barcode-label/Templates.vue'),
    { title: '标签模板', icon: 'Ticket', permission: 'Wms.BarcodeLabel.Read' }),
  createRoute('barcode-label/print-jobs', 'BarcodeLabelPrintJobs',
    () => import('@/views/barcode-label/PrintJobs.vue'),
    { title: '打印任务', icon: 'Ticket', permission: 'Wms.BarcodeLabel.Print' }),

  // ========================
  // BC-12 Workflow (2 pages)
  // ========================
  createRoute('workflow/definitions', 'WorkflowDefinitions',
    () => import('@/views/workflow/Definitions.vue'),
    { title: '审批流配置', icon: 'Share', permission: 'Wms.Workflow.Read' }),
  createRoute('workflow/approval', 'WorkflowApproval',
    () => import('@/views/workflow/Approval.vue'),
    { title: '审批页面', icon: 'Share', permission: 'Wms.Workflow.Approve' }),

  // ========================
  // BC-13 RuleEngine (2 pages)
  // ========================
  createRoute('rule-engine/rules', 'RuleEngineRules',
    () => import('@/views/rule-engine/Rules.vue'),
    { title: '规则配置', icon: 'Operation', permission: 'Wms.RuleEngine.Read' }),
  createRoute('rule-engine/test', 'RuleEngineTest',
    () => import('@/views/rule-engine/Test.vue'),
    { title: '规则测试', icon: 'Operation', permission: 'Wms.RuleEngine.Execute' }),

  // ========================
  // BC-14 Notification (2 pages)
  // ========================
  createRoute('notification/logs', 'NotificationLogs',
    () => import('@/views/notification/Logs.vue'),
    { title: '通知列表', icon: 'Bell', permission: 'Wms.Notification.Read' }),
  createRoute('notification/config', 'NotificationConfig',
    () => import('@/views/notification/Config.vue'),
    { title: '通知配置', icon: 'Bell', permission: 'Wms.Notification.Create' }),

  // ========================
  // BC-16 DataDictionary (2 pages)
  // ========================
  createRoute('data-dictionary/list', 'DataDictionaryList',
    () => import('@/views/data-dictionary/List.vue'),
    { title: '数据字典', icon: 'Notebook', permission: 'Wms.DataDictionary.Dictionaries' }),
  createRoute('data-dictionary/items/:id', 'DataDictionaryItems',
    () => import('@/views/data-dictionary/Items.vue'),
    { title: '字典项管理', icon: 'Notebook', permission: 'Wms.DataDictionary.Items', hidden: true }),

  // ========================
  // BC-15 Dashboard (5 pages - P1 仪表盘)
  // ========================
  createRoute('dashboard/index', 'DashboardIndex',
    () => import('@/views/dashboard/Index.vue'),
    { title: '通用仪表盘', icon: 'DataAnalysis', permission: 'Wms.Dashboard.Read' }),
  createRoute('dashboard/warehouse', 'DashboardWarehouse',
    () => import('@/views/dashboard/Warehouse.vue'),
    { title: '仓库仪表盘', icon: 'DataAnalysis', permission: 'Wms.Dashboard.Read' }),
  createRoute('dashboard/inventory', 'DashboardInventory',
    () => import('@/views/dashboard/Inventory.vue'),
    { title: '库存仪表盘', icon: 'DataAnalysis', permission: 'Wms.Dashboard.Read' }),
  createRoute('dashboard/task', 'DashboardTask',
    () => import('@/views/dashboard/Task.vue'),
    { title: '任务仪表盘', icon: 'DataAnalysis', permission: 'Wms.Dashboard.Read' }),
  createRoute('dashboard/inbound-statistics', 'DashboardInboundStatistics',
    () => import('@/views/dashboard/InboundStatistics.vue'),
    { title: '入库统计仪表盘', icon: 'DataAnalysis', permission: 'Wms.Dashboard.Read' }),
];

// ── Permission Filtering ───────────────────────────────────────────

function hasPermission(route: RouteRecordRaw, permissions: string[]): boolean {
  const permission = route.meta?.permission as string | undefined;
  if (!permission) return true;

  // Wms.All wildcard or admin role — pass all checks
  if (permissions.includes('Wms.All') || permissions.includes('admin')) return true;

  // Exact match
  if (permissions.includes(permission)) return true;

  // Module prefix match: e.g. route permission "Wms.Warehouse.Read" matches
  // any granted permission that is a prefix (e.g. "Wms.Warehouse")
  for (const p of permissions) {
    if (permission.startsWith(p + '.') || p.startsWith(permission + '.')) return true;
  }

  return false;
}

export function filterAsyncRoutes(
  routes: RouteRecordRaw[],
  permissions: string[]
): RouteRecordRaw[] {
  const result: RouteRecordRaw[] = [];
  for (const route of routes) {
    if (!hasPermission(route, permissions)) continue;
    const cloned: RouteRecordRaw = { ...route };
    if (cloned.children && cloned.children.length > 0) {
      cloned.children = filterAsyncRoutes(cloned.children, permissions);
    }
    result.push(cloned);
  }
  return result;
}

// ── Dynamic Route Registration ─────────────────────────────────────

const NOT_FOUND_ROUTE_NAME = 'DefaultLayoutNotFound';

export async function setupDynamicRoutes(): Promise<void> {
  const permissionStore = usePermissionStore();
  if (permissionStore.dynamicRoutesLoaded) return;

  const permissions = await getPermissions();
  const grantedRoutes = filterAsyncRoutes(asyncRoutes, permissions);

  for (const route of grantedRoutes) {
    router.addRoute('DefaultLayout', route);
  }

  // Fallback route for un-granted paths
  router.addRoute('DefaultLayout', {
    path: ':pathMatch(.*)',
    name: NOT_FOUND_ROUTE_NAME,
    redirect: '/',
    meta: { title: 'Not Found', icon: 'Warning', hidden: true },
  });

  permissionStore.setGrantedRoutes(grantedRoutes, permissions);

  const authStore = useAuthStore();
  authStore.setPermissions(permissions);
}

export function resetDynamicRoutes(): void {
  const permissionStore = usePermissionStore();

  if (router.hasRoute(NOT_FOUND_ROUTE_NAME)) {
    router.removeRoute(NOT_FOUND_ROUTE_NAME);
  }

  permissionStore.grantedRoutes.forEach((route) => {
    if (route.name) router.removeRoute(route.name);
  });

  permissionStore.reset();
}
