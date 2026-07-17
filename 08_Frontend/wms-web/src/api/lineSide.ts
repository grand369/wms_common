import { get, post, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface LineSideStationDto {
  id: string;
  code: string;
  name: string;
  productionLineId?: string;
  productionLineName?: string;
  warehouseId: string;
  status: number;
}

export interface KanbanDataDto {
  stationId: string;
  stationName: string;
  materials: KanbanMaterialDto[];
  lastUpdated: string;
}

export interface KanbanMaterialDto {
  materialId: string;
  materialCode: string;
  materialName: string;
  requiredQty: number;
  currentQty: number;
  status: string;
}

export interface ReplenishmentTaskDto {
  id: string;
  taskNo: string;
  stationId: string;
  stationName: string;
  materialId: string;
  materialCode: string;
  qty: number;
  status: number;
}

export function getLineSideStations(params: PagedParams) {
  return get<PagedResult<LineSideStationDto>>('/api/v1/line-side/stations', { params });
}

export function getLineSideStation(id: string) {
  return get<LineSideStationDto>(`/api/v1/line-side/stations/${id}`);
}

export function getKanbanData(stationId?: string) {
  return get<KanbanDataDto>(`/api/v1/line-side/stations/${stationId}/kanban-items`);
}

export function triggerReplenishment(data: { stationId: string; materialId: string; qty: number }) {
  return post<ReplenishmentTaskDto>(`/api/v1/line-side/stations/${data.stationId}/trigger-replenishment`, { materialId: data.materialId, qty: data.qty });
}

export function getReplenishmentTasks(params: PagedParams) {
  return get<PagedResult<ReplenishmentTaskDto>>('/api/v1/line-side/replenishment-tasks', { params });
}

export function getReplenishmentTask(id: string) {
  return get<ReplenishmentTaskDto>(`/api/v1/line-side/replenishment-tasks/${id}`);
}

export function completeReplenishment(id: string) {
  return patch<void>(`/api/v1/line-side/replenishment-tasks/${id}/complete`);
}