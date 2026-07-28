import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface InboundOrderDto {
  id: string;
  inboundOrderNo: string;
  inboundTypeValue: number;
  inboundTypeName: string;
  inboundStatusValue: number;
  inboundStatusName: string;
  supplierId?: string;
  supplierName?: string;
  warehouseId: string;
  warehouseCode: string;
  purchaseOrderId?: string;
  purchaseOrderNo?: string;
  productionOrderId?: string;
  returnOrderId?: string;
  overReceiptRatio: number;
  qualityInspectionRequired: boolean;
  totalPlanQuantity: number;
  totalReceivedQuantity: number;
  isCompleted: boolean;
  completionTime?: string;
  remark?: string;
  creationTime: string;
}

export interface CreateOrUpdateInboundOrderDto {
  inboundTypeValue: number;
  warehouseId: string;
  warehouseCode: string;
  supplierId?: string;
  supplierName?: string;
  purchaseOrderId?: string;
  purchaseOrderNo?: string;
  productionOrderId?: string;
  returnOrderId?: string;
  overReceiptRatio?: number;
  qualityInspectionRequired?: boolean;
  remark?: string;
  lines: InboundOrderLineDto[];
}

export interface InboundOrderLineDto {
  id?: string;
  inboundOrderId?: string;
  lineNo: number;
  materialId: string;
  materialCode: string;
  materialName: string;
  unit?: string;
  planQuantity: number;
  receivedQuantity: number;
  batchNumber?: string;
  qualityStatusValue?: number;
  qualityStatusName?: string;
  putawayWarehouseId?: string;
  putawayWarehouseCode?: string;
  putawayAreaId?: string;
  putawayAreaCode?: string;
  putawayLocationId?: string;
  putawayLocationCode?: string;
  expiryDate?: string;
  productionDate?: string;
  remark?: string;
}

export interface InboundOrderDetailDto extends InboundOrderDto {
  lines: InboundOrderLineDto[];
}

export interface InboundStatisticsDto {
  totalCount: number;
  pendingCount: number;
  completedCount: number;
  todayCount: number;
}

export function getInboundOrders(params: PagedParams) {
  return get<PagedResult<InboundOrderDto>>('/api/v1/inbound/orders', { params });
}

export function getInboundOrder(id: string) {
  return get<InboundOrderDetailDto>(`/api/v1/inbound/orders/${id}`);
}

export function createInboundOrder(data: CreateOrUpdateInboundOrderDto) {
  return post<InboundOrderDto>('/api/v1/inbound/orders', data);
}

export function updateInboundOrder(id: string, data: CreateOrUpdateInboundOrderDto) {
  return put<InboundOrderDto>(`/api/v1/inbound/orders/${id}`, data);
}

export function deleteInboundOrder(id: string) {
  return del<void>(`/api/v1/inbound/orders/${id}`);
}

export interface InboundConfirmLineDto {
  lineId: string;
  receivedQuantity: number;
  batchNumber?: string;
}

export interface InboundConfirmCommandDto {
  idempotencyId: string;
  lines: InboundConfirmLineDto[];
}

export function confirmInbound(id: string, data: InboundConfirmCommandDto) {
  return patch<void>(`/api/v1/inbound/orders/${id}/confirm`, data);
}

export interface InboundQualityInspectLineDto {
  lineId: string;
  qualityResultValue: number;
}

export interface InboundQualityInspectCommandDto {
  idempotencyId: string;
  lines: InboundQualityInspectLineDto[];
}

export function qualityInspectInbound(id: string, data: InboundQualityInspectCommandDto) {
  return patch<void>(`/api/v1/inbound/orders/${id}/quality-inspect`, data);
}

export interface InboundPutawayLineDto {
  lineId: string;
  putawayWarehouseId: string;
  putawayWarehouseCode: string;
  putawayAreaId: string;
  putawayAreaCode: string;
  putawayLocationId: string;
  putawayLocationCode: string;
  quantity: number;
}

export interface InboundPutawayCommandDto {
  idempotencyId: string;
  lines: InboundPutawayLineDto[];
}

export function putawayInbound(id: string, data: InboundPutawayCommandDto) {
  return patch<void>(`/api/v1/inbound/orders/${id}/putaway`, data);
}

export function completeInbound(id: string) {
  return patch<void>(`/api/v1/inbound/orders/${id}/complete`);
}

export function cancelInbound(id: string, data?: { reason?: string }) {
  return patch<void>(`/api/v1/inbound/orders/${id}/cancel`, data);
}

export function getInboundOrderDetails(id: string) {
  return get<{ items: InboundOrderLineDto[] }>(`/api/v1/inbound/orders/${id}/lines`);
}

export function getInboundStatistics(params?: { startDate?: string; endDate?: string }) {
  return get<InboundStatisticsDto>('/api/v1/inbound/orders/statistics', { params });
}

export interface InboundErpCallbackDto {
  erpDocumentNo?: string;
  callbackStatus: number;
  message?: string;
  callbackTime?: string;
}

export function erpCallbackInbound(id: string, data: InboundErpCallbackDto) {
  return patch<void>(`/api/v1/inbound/orders/${id}/erp-callback`, data);
}

export function receiveInboundLine(id: string, data: { qty: number }) {
  return patch<void>(`/api/v1/inbound/orders/${id}/confirm`, data);
}
