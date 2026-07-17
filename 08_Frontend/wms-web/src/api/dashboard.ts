import { get } from './index';

export interface DashboardStats {
  inventoryValue: number;
  todayInbound: number;
  todayOutbound: number;
  pendingTasks: number;
  alertCount: number;
}

export interface InboundTrend {
  date: string;
  quantity: number;
}

export interface OutboundTrend {
  date: string;
  quantity: number;
}

export interface InventoryDistribution {
  category: string;
  value: number;
}

export interface TaskExecutionRate {
  name: string;
  rate: number;
  total: number;
  completed: number;
}

export interface DashboardAlert {
  id: string;
  type: 'safety' | 'expiry' | 'timeout';
  level: 'danger' | 'warning';
  message: string;
  timestamp: string;
}

export interface WarehouseDashboardData {
  occupancyRate: number;
  inboundCount: number;
  outboundCount: number;
  taskRate: number;
  inboundTrend: InboundTrend[];
  outboundTrend: OutboundTrend[];
  locationHeatmap: { zone: string; rate: number }[];
}

export interface InventoryDashboardData {
  distribution: InventoryDistribution[];
  alertTrend: { date: string; count: number }[];
  frozenStats: { status: string; count: number }[];
  adjustmentTrend: { date: string; count: number }[];
}

export interface TaskDashboardData {
  executionRate: TaskExecutionRate[];
  efficiencyHeatmap: { period: string; value: number }[];
  personnelLoad: { name: string; taskCount: number; rate: number }[];
  abnormalRate: number;
}

export interface InboundStatsDashboardData {
  inboundCount: number;
  supplierDistribution: { supplier: string; count: number }[];
  qualityRate: number;
  typeDistribution: { type: string; count: number }[];
  inboundTrend: InboundTrend[];
}

export function getDashboardStats(): Promise<DashboardStats> {
  return get<DashboardStats>('/api/v1/dashboard/stats');
}

export function getInboundTrend(): Promise<InboundTrend[]> {
  return get<InboundTrend[]>('/api/v1/dashboard/inbound-trend');
}

export function getOutboundTrend(): Promise<OutboundTrend[]> {
  return get<OutboundTrend[]>('/api/v1/dashboard/outbound-trend');
}

export function getInventoryDistribution(): Promise<InventoryDistribution[]> {
  return get<InventoryDistribution[]>('/api/v1/dashboard/inventory-distribution');
}

export function getTaskExecutionRate(): Promise<TaskExecutionRate[]> {
  return get<TaskExecutionRate[]>('/api/v1/dashboard/task-execution-rate');
}

export function getDashboardAlerts(): Promise<DashboardAlert[]> {
  return get<DashboardAlert[]>('/api/v1/dashboard/alerts');
}

export function getWarehouseDashboard(warehouseId?: string): Promise<WarehouseDashboardData> {
  return get<WarehouseDashboardData>('/api/v1/dashboard/warehouse', {
    params: { warehouseId },
  });
}

export function getInventoryDashboard(): Promise<InventoryDashboardData> {
  return get<InventoryDashboardData>('/api/v1/dashboard/inventory');
}

export function getTaskDashboard(): Promise<TaskDashboardData> {
  return get<TaskDashboardData>('/api/v1/dashboard/task');
}

export function getInboundStatsDashboard(): Promise<InboundStatsDashboardData> {
  return get<InboundStatsDashboardData>('/api/v1/dashboard/inbound-stats');
}