import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface TransferDto {
  id: string;
  transferNo: string;
  fromWarehouseId: string;
  fromWarehouseName: string;
  toWarehouseId: string;
  toWarehouseName: string;
  status: number;
  planDate?: string;
}

export interface CreateOrUpdateTransferDto {
  fromWarehouseId: string;
  toWarehouseId: string;
  planDate?: string;
  lines: TransferLineDto[];
}

export interface TransferLineDto {
  id?: string;
  materialId: string;
  materialCode?: string;
  materialName?: string;
  qty: number;
  batchNo?: string;
}

export interface TransferDetailDto extends TransferDto {
  lines: TransferLineDto[];
}

export interface TransferTrackingDto {
  transferId: string;
  transferNo: string;
  status: number;
  events: TransferTrackingEventDto[];
}

export interface TransferTrackingEventDto {
  eventTime: string;
  eventType: string;
  operatorName?: string;
  remark?: string;
}

export function getTransfers(params: PagedParams) {
  return get<PagedResult<TransferDto>>('/api/v1/transfer/orders', { params });
}

export function getTransfer(id: string) {
  return get<TransferDetailDto>(`/api/v1/transfer/orders/${id}`);
}

export function createTransfer(data: CreateOrUpdateTransferDto) {
  return post<TransferDto>('/api/v1/transfer/orders', data);
}

export function updateTransfer(id: string, data: CreateOrUpdateTransferDto) {
  return put<TransferDto>(`/api/v1/transfer/orders/${id}`, data);
}

export function deleteTransfer(id: string) {
  return del<void>(`/api/v1/transfer/orders/${id}`);
}

export function approveTransfer(id: string) {
  return patch<void>(`/api/v1/transfer/orders/${id}/approve`);
}

export function outboundConfirmTransfer(id: string, data?: { operatorId?: string }) {
  return patch<void>(`/api/v1/transfer/orders/${id}/outbound-confirm`, data);
}

export function inboundConfirmTransfer(id: string, data?: { operatorId?: string }) {
  return patch<void>(`/api/v1/transfer/orders/${id}/inbound-confirm`, data);
}

export function completeTransfer(id: string) {
  return patch<void>(`/api/v1/transfer/orders/${id}/complete`);
}

export function cancelTransfer(id: string, data?: { reason?: string }) {
  return patch<void>(`/api/v1/transfer/orders/${id}/cancel`, data);
}

export function getTransferTracking(id: string) {
  return get<TransferTrackingDto>(`/api/v1/transfer/orders/${id}`);
}
