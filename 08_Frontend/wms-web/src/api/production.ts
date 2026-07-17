import { get, post, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface ProductionRequisitionDto {
  id: string;
  requisitionNo: string;
  workOrderId?: string;
  workOrderNo?: string;
  productionLineId?: string;
  productionLineName?: string;
  status: number;
  planDate?: string;
}

export interface CreateProductionRequisitionDto {
  workOrderId?: string;
  productionLineId?: string;
  planDate?: string;
  lines: ProductionRequisitionLineDto[];
}

export interface ProductionRequisitionLineDto {
  materialId: string;
  materialCode?: string;
  materialName?: string;
  qty: number;
  batchNo?: string;
}

export interface FinishedGoodsDto {
  id: string;
  inboundNo: string;
  workOrderId?: string;
  workOrderNo?: string;
  warehouseId: string;
  warehouseName: string;
  status: number;
  inboundDate?: string;
}

export interface CreateFinishedGoodsDto {
  workOrderId?: string;
  warehouseId: string;
  inboundDate?: string;
  lines: FinishedGoodsLineDto[];
}

export interface FinishedGoodsLineDto {
  materialId: string;
  materialCode?: string;
  materialName?: string;
  qty: number;
  batchNo?: string;
}

export interface SubcontractOrderDto {
  id: string;
  orderNo: string;
  supplierId?: string;
  supplierName?: string;
  status: number;
  planDate?: string;
}

export interface CreateSubcontractOrderDto {
  supplierId?: string;
  planDate?: string;
  lines: SubcontractOrderLineDto[];
}

export interface SubcontractOrderLineDto {
  materialId: string;
  materialCode?: string;
  materialName?: string;
  qty: number;
}

export function getRequisitions(params: PagedParams) {
  return get<PagedResult<ProductionRequisitionDto>>('/api/v1/production/requisitions', { params });
}

export function getRequisition(id: string) {
  return get<ProductionRequisitionDto>(`/api/v1/production/requisitions/${id}`);
}

export function createRequisition(data: CreateProductionRequisitionDto) {
  return post<ProductionRequisitionDto>('/api/v1/production/requisitions', data);
}

export function issueRequisition(id: string) {
  return patch<void>(`/api/v1/production/requisitions/${id}/issue`);
}

export function getFinishedGoods(params: PagedParams) {
  return get<PagedResult<FinishedGoodsDto>>('/api/v1/production/finished-goods', { params });
}

export function getFinishedGoodsInbound(id: string) {
  return get<FinishedGoodsDto>(`/api/v1/production/finished-goods/${id}`);
}

export function createFinishedGoodsInbound(data: CreateFinishedGoodsDto) {
  return post<FinishedGoodsDto>('/api/v1/production/finished-goods', data);
}

export function getSubcontractOrders(params: PagedParams) {
  return get<PagedResult<SubcontractOrderDto>>('/api/v1/production/subcontract-orders', { params });
}

export function createSubcontractOrder(data: CreateSubcontractOrderDto) {
  return post<SubcontractOrderDto>('/api/v1/production/subcontract-orders', data);
}