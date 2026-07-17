import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface InboundOrderDto {
  id: string;
  orderNo: string;
  orderType: string;
  supplierId?: string;
  supplierName?: string;
  warehouseId: string;
  warehouseName: string;
  status: number;
  planDate?: string;
  arrivalDate?: string;
}

export interface CreateOrUpdateInboundOrderDto {
  orderType: string;
  supplierId?: string;
  warehouseId: string;
  planDate?: string;
  lines: InboundOrderLineDto[];
}

export interface InboundOrderLineDto {
  id?: string;
  materialId: string;
  materialCode?: string;
  materialName?: string;
  qty: number;
  batchNo?: string;
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

export function confirmInbound(id: string) {
  return patch<void>(`/api/v1/inbound/orders/${id}/confirm`);
}

export function qualityInspectInbound(id: string, data: { passed: boolean; remark?: string }) {
  return patch<void>(`/api/v1/inbound/orders/${id}/quality-inspect`, data);
}

export function putawayInbound(id: string) {
  return patch<void>(`/api/v1/inbound/orders/${id}/putaway`);
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

export function receiveInboundLine(id: string, data: { qty: number }) {
  return patch<void>(`/api/v1/inbound/orders/${id}/confirm`, data);
}
