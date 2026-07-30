import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface OutboundOrderDto {
  id: string;
  outboundOrderNo: string;
  outboundTypeValue: number;
  outboundTypeName: string;
  outboundStatusValue: number;
  outboundStatusName: string;
  warehouseId: string;
  warehouseCode: string;
  materialRequisitionId?: string;
  salesOrderId?: string;
  returnMaterialOrderId?: string;
  overIssueRatio: number;
  isEmergency: boolean;
  totalRequiredQuantity: number;
  totalAllocatedQuantity: number;
  totalPickedQuantity: number;
  totalShippedQuantity: number;
  isCompleted: boolean;
  completionTime?: string;
  erpCallbackStatusValue: number;
  erpCallbackStatusName: string;
  remark?: string;
  creationTime?: string;
}

export interface CreateOrUpdateOutboundOrderDto {
  outboundTypeValue: number;
  warehouseId: string;
  warehouseCode: string;
  materialRequisitionId?: string;
  salesOrderId?: string;
  returnMaterialOrderId?: string;
  overIssueRatio?: number;
  isEmergency?: boolean;
  remark?: string;
  lines: OutboundOrderLineDto[];
}

export interface OutboundOrderLineDto {
  id?: string;
  materialId: string;
  materialCode: string;
  materialName: string;
  requiredQuantity: number;
  issueStrategyValue?: number;
  batchNumber?: string;
  remark?: string;
}

export interface OutboundOrderDetailDto extends OutboundOrderDto {
  lines: OutboundLineOutputDto[];
}

export interface OutboundLineOutputDto {
  id: string;
  outboundOrderId: string;
  lineNo: number;
  materialId: string;
  materialCode: string;
  materialName: string;
  requiredQuantity: number;
  allocatedQuantity: number;
  pickedQuantity: number;
  shippedQuantity: number;
  pickingLocationId?: string;
  pickingLocationCode?: string;
  issueStrategyValue: number;
  issueStrategyName: string;
  batchNumber?: string;
  remark?: string;
}

export interface OutboundStatisticsDto {
  totalCount: number;
  pendingCount: number;
  completedCount: number;
  todayCount: number;
}

export function getOutboundOrders(params: PagedParams) {
  return get<PagedResult<OutboundOrderDto>>('/api/v1/outbound/orders', { params });
}

export function getOutboundOrder(id: string) {
  return get<OutboundOrderDetailDto>(`/api/v1/outbound/orders/${id}`);
}

export function createOutboundOrder(data: CreateOrUpdateOutboundOrderDto) {
  return post<OutboundOrderOutputDto>('/api/v1/outbound/orders', data);
}

export function updateOutboundOrder(id: string, data: CreateOrUpdateOutboundOrderDto) {
  return put<OutboundOrderOutputDto>(`/api/v1/outbound/orders/${id}`, data);
}

export function deleteOutboundOrder(id: string) {
  return del<void>(`/api/v1/outbound/orders/${id}`);
}

export interface OutboundAllocateLineDto {
  lineId: string;
  allocatedQuantity: number;
  locationId?: string;
  locationCode?: string;
}

export interface OutboundAllocateCommandDto {
  idempotencyId: string;
  lines: OutboundAllocateLineDto[];
}

export interface OutboundOrderOutputDto {
  id: string;
  outboundOrderNo: string;
  outboundTypeValue: number;
  outboundTypeName: string;
  outboundStatusValue: number;
  outboundStatusName: string;
  warehouseId: string;
  warehouseCode: string;
  lines: OutboundLineOutputDto[];
}

export function allocateOutbound(id: string, data: OutboundAllocateCommandDto) {
  return patch<OutboundOrderOutputDto>(`/api/v1/outbound/orders/${id}/allocate`, data);
}

export interface OutboundPickingCommandDto {
  idempotencyId: string
  lines: { lineId: string; pickedQuantity: number }[]
}

export function pickOutbound(id: string, data: OutboundPickingCommandDto) {
  return patch<void>(`/api/v1/outbound/orders/${id}/picking`, data)
}

export interface OutboundShippingCommandDto {
  idempotencyId: string
  lines?: { lineId: string; shippedQuantity: number }[]
  trackingNo?: string
}

export function shipOutbound(id: string, data: OutboundShippingCommandDto) {
  return patch<void>(`/api/v1/outbound/orders/${id}/shipping`, data)
}

export function completeOutbound(id: string) {
  return patch<void>(`/api/v1/outbound/orders/${id}/complete`);
}

export function cancelOutbound(id: string, data?: { reason?: string }) {
  return patch<void>(`/api/v1/outbound/orders/${id}/cancel`, data);
}

export function getOutboundOrderDetails(id: string) {
  return get<{ items: OutboundOrderLineDto[] }>(`/api/v1/outbound/orders/${id}/lines`);
}

export function getOutboundStatistics(params?: { startDate?: string; endDate?: string }) {
  return get<OutboundStatisticsDto>('/api/v1/outbound/orders/statistics', { params });
}

export interface OutboundErpCallbackDto {
  erpDocumentNo?: string;
  callbackStatus: number;
  message?: string;
  callbackTime?: string;
}

export function erpCallbackOutbound(id: string, data: OutboundErpCallbackDto) {
  return patch<void>(`/api/v1/outbound/orders/${id}/erp-callback`, data);
}

export enum OutboundPrintType {
  Order = 1,
  PackingList = 2,
  AddressLabel = 3
}

export interface OutboundPrintDto {
  printType: OutboundPrintType;
  includeBarcode?: boolean;
  copies?: number;
}

export function getOutboundPrintData(id: string, params?: OutboundPrintDto) {
  return get<OutboundOrderDetailDto>(`/api/v1/outbound/orders/${id}/print-data`, { params });
}

export function pickOutboundLine(id: string, lineId: string, pickedQty: number) {
  const command: OutboundPickingCommandDto = {
    idempotencyId: `pickline_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
    lines: [{ lineId, pickedQty }],
  };
  return patch<void>(`/api/v1/outbound/orders/${id}/picking`, command);
}
