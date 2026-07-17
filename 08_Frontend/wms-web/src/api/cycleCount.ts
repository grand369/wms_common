import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface CycleCountPlanDto {
  id: string;
  planNo: string;
  warehouseId: string;
  warehouseName: string;
  countType: string;
  status: number;
  planDate?: string;
}

export interface CreateOrUpdateCycleCountPlanDto {
  warehouseId: string;
  countType: string;
  planDate?: string;
  locationIds?: string[];
  materialIds?: string[];
}

export interface CycleCountRecordDto {
  id: string;
  planId: string;
  materialId: string;
  materialCode: string;
  locationId?: string;
  systemQty: number;
  countQty: number;
  differenceQty: number;
}

export function getCycleCountPlans(params: PagedParams) {
  return get<PagedResult<CycleCountPlanDto>>('/api/v1/cycle-count/plans', { params });
}

export function getCycleCountPlan(id: string) {
  return get<CycleCountPlanDto>(`/api/v1/cycle-count/plans/${id}`);
}

export function createCycleCountPlan(data: CreateOrUpdateCycleCountPlanDto) {
  return post<CycleCountPlanDto>('/api/v1/cycle-count/plans', data);
}

export function updateCycleCountPlan(id: string, data: CreateOrUpdateCycleCountPlanDto) {
  return put<CycleCountPlanDto>(`/api/v1/cycle-count/plans/${id}`, data);
}

export function deleteCycleCountPlan(id: string) {
  return del<void>(`/api/v1/cycle-count/plans/${id}`);
}

export function startCounting(id: string) {
  return patch<void>(`/api/v1/cycle-count/plans/${id}/start`);
}

export function getCycleCountRecords(planId: string) {
  return get<{ items: CycleCountRecordDto[] }>(`/api/v1/cycle-count/plans/${planId}/records`);
}

export function submitCount(planId: string, data: { recordId: string; countQty: number }) {
  return patch<void>(`/api/v1/cycle-count/plans/${planId}/submit-count`, data);
}

export function getCycleCountDifferences(planId: string) {
  return get<{ items: CycleCountRecordDto[] }>(`/api/v1/cycle-count/plans/${planId}/differences`);
}

export function confirmDifference(planId: string, data?: { reason?: string }) {
  return patch<void>(`/api/v1/cycle-count/plans/${planId}/confirm-difference`, data);
}

export function generateAdjustment(planId: string) {
  return patch<void>(`/api/v1/cycle-count/plans/${planId}/generate-adjustment`);
}

export function completeCycleCount(id: string) {
  return patch<void>(`/api/v1/cycle-count/plans/${id}/complete`);
}
