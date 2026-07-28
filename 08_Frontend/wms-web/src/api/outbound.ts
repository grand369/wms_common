import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface OutboundOrderDto {
  id: string;
  orderNo: string;
  orderType: string;
  customerId?: string;
  customerName?: string;
  warehouseId: string;
  warehouseName: string;
  status: number;
  planDate?: string;
  shipDate?: string;
}

export interface CreateOrUpdateOutboundOrderDto {
  orderType: string;
  customerId?: string;
  warehouseId: string;
  planDate?: string;
  lines: OutboundOrderLineDto[];
}

export interface OutboundOrderLineDto {
  id?: string;
  materialId: string;
  materialCode?: string;
  materialName?: string;
  qty: number;
  batchNo?: string;
}

export interface OutboundOrderDetailDto extends OutboundOrderDto {
  lines: OutboundOrderLineDto[];
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
  return post<OutboundOrderDto>('/api/v1/outbound/orders', data);
}

export function updateOutboundOrder(id: string, data: CreateOrUpdateOutboundOrderDto) {
  return put<OutboundOrderDto>(`/api/v1/outbound/orders/${id}`, data);
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
  lines: OutboundAllocateLineDto[];
}

export function allocateOutbound(id: string, data?: OutboundAllocateCommandDto) {
  return patch<void>(`/api/v1/outbound/orders/${id}/allocate`, data);
}

export function pickOutbound(id: string, data?: { lines?: { lineId: string; pickedQty: number }[] }) {
  return patch<void>(`/api/v1/outbound/orders/${id}/picking`, data);
}

export function shipOutbound(id: string, data?: { trackingNo?: string }) {
  return patch<void>(`/api/v1/outbound/orders/${id}/shipping`, data);
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

export function pickOutboundLine(id: string, data: { pickedQty: number }) {
  return patch<void>(`/api/v1/outbound/orders/${id}/picking`, data);
}
