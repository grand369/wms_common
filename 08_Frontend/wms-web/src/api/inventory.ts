import { get, post, del } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface InventoryBalanceDto {
  id: string;
  materialId: string;
  materialCode: string;
  materialName: string;
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  locationId: string;
  locationCode: string;
  locationName: string;
  batchNumber?: string;
  inventoryStatusValue: number;
  inventoryStatusName: string;
  quantity: number;
  reservedQuantity: number;
  frozenQuantity: number;
  inTransitQuantity: number;
  availableQuantity: number;
}

export interface InventoryLedgerDto {
  id: string;
  inventoryBalanceId: string;
  
  // Material info
  materialId: string;
  materialCode: string;
  materialName: string;
  
  // Warehouse/Location info
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  locationId: string;
  locationCode: string;
  locationName: string;
  
  // Operation info
  operationTypeValue: number;
  operationTypeName: string;
  operationQuantity: number;
  inQuantity: number;
  outQuantity: number;
  balanceQuantity: number;
  beforeQuantity: number;
  afterQuantity: number;
  beforeAvailable: number;
  afterAvailable: number;
  operationTime: string;
  operatorId: string;
  operatorName: string;
  
  // Source order info
  sourceOrderType: string;
  sourceOrderId: string;
  sourceOrderNo: string;
  remark?: string;
}

export interface InventoryAlertDto {
  id: string;
  alertType: string;
  materialId: string;
  materialCode: string;
  warehouseId: string;
  threshold: number;
  currentQty: number;
  status: number;
}

export interface InventoryFreezeDto {
  id: string;
  freezeOrderNo: string;
  freezeScopeValue: number;
  freezeScopeName: string;
  freezeReason: string;
  freezeStatusValue: number;
  freezeStatusName: string;
  warehouseId: string;
  warehouseCode: string;
  materialId?: string;
  materialCode?: string;
  freezeQuantity: number;
  isApproved: boolean;
  freezeStartTime: string;
  freezeEndTime?: string;
  remark?: string;
  creationTime: string;
}

export interface InventoryAdjustmentDto {
  id: string;
  balanceId: string;
  materialId: string;
  materialCode: string;
  adjustQty: number;
  reason: string;
  status: number;
}

export interface InventorySnapshotDto {
  id: string;
  snapshotNo: string;
  warehouseId: string;
  snapshotTime: string;
  totalQty: number;
  status: number;
}

export function getBalances(params: PagedParams) {
  return get<PagedResult<InventoryBalanceDto>>('/api/v1/inventory/balances', { params });
}

export function getBalance(id: string) {
  return get<InventoryBalanceDto>(`/api/v1/inventory/balances/${id}`);
}

export function getLedger(params: PagedParams) {
  return get<PagedResult<InventoryLedgerDto>>('/api/v1/inventory/ledger-entries', { params });
}

export function getAlerts(params: PagedParams) {
  return get<PagedResult<InventoryAlertDto>>('/api/v1/inventory/alerts', { params });
}

export function getAlert(id: string) {
  return get<InventoryAlertDto>(`/api/v1/inventory/alerts/${id}`);
}

export function createFreeze(data: InventoryFreezeDto) {
  return post<InventoryFreezeDto>('/api/v1/inventory/freeze-orders', data);
}

export function getFreezeOrders(params: PagedParams) {
  return get<PagedResult<InventoryFreezeDto>>('/api/v1/inventory/freeze-orders', { params });
}

export function releaseFreeze(id: string) {
  return post<void>(`/api/v1/inventory/freeze-orders/${id}/release`);
}

export function cancelFreeze(id: string) {
  return post<void>(`/api/v1/inventory/freeze-orders/${id}/cancel`);
}

export function deleteFreeze(id: string) {
  return del<void>(`/api/v1/inventory/freeze-orders/${id}`);
}

export function createAdjustment(data: InventoryAdjustmentDto) {
  return post<InventoryAdjustmentDto>('/api/v1/inventory/adjustments', data);
}

export function getAdjustments(params: PagedParams) {
  return get<PagedResult<InventoryAdjustmentDto>>('/api/v1/inventory/adjustments', { params });
}

export function getAdjustment(id: string) {
  return get<InventoryAdjustmentDto>(`/api/v1/inventory/adjustments/${id}`);
}

export function confirmAdjustment(id: string) {
  return post<void>(`/api/v1/inventory/adjustments/${id}/approve`);
}

export function getSnapshots(params: PagedParams) {
  return get<PagedResult<InventorySnapshotDto>>('/api/v1/inventory/balances/snapshot', { params });
}

export function getSnapshot(id: string) {
  return get<InventorySnapshotDto>(`/api/v1/inventory/balances/${id}`);
}

export function createSnapshot(data: { warehouseId: string; remark?: string }) {
  return post<InventorySnapshotDto>('/api/v1/inventory/balances/snapshot', data);
}

export function freezeBalance(id: string, data: { qty: number; reason: string }) {
  return post<InventoryBalanceDto>(`/api/v1/inventory/balances/${id}/freeze`, data);
}

export function unfreezeBalance(id: string) {
  return post<InventoryBalanceDto>(`/api/v1/inventory/balances/${id}/unfreeze`);
}

export function getInventoryAgeAnalysis(params: PagedParams) {
  return get<PagedResult<any>>('/api/v1/inventory/balances/age-analysis', { params });
}

export function getInventoryMovement(params: PagedParams) {
  return get<PagedResult<any>>('/api/v1/inventory/ledger-entries/movement', { params });
}
