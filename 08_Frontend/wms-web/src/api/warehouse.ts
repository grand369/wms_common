import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult, ListResultDto } from '@/api/types';

export interface WarehouseDto {
  id: string;
  warehouseCode: string;
  warehouseName: string;
  warehouseType: number;
  warehouseTypeDescription: string;
  organizationUnitId: string;
  organizationUnitName: string;
  plantId: string;
  plantName: string;
  responsibleUserId?: string;
  responsibleUserName?: string;
  address?: string;
  storageConditionType: number;
  storageConditionTypeDescription: string;
  locationLevelCount: number;
  isActive: boolean;
  remark?: string;
}

export interface CreateOrUpdateWarehouseDto {
  warehouseCode: string;
  warehouseName: string;
  warehouseType: number;
  organizationUnitId: string;
  organizationUnitName: string;
  plantId: string;
  plantName: string;
  responsibleUserId?: string;
  responsibleUserName?: string;
  address?: string;
  storageConditionType?: number;
  locationLevelCount?: number;
  isActive?: boolean;
  remark?: string;
}

export interface AreaDto {
  id: string;
  areaCode: string;
  areaName: string;
  warehouseId: string;
  warehouseCode: string;
  areaFunction: number;
  storageEnvironment: number;
  maxCapacity?: number;
  currentCapacity?: number;
  isActive: boolean;
}

export interface CreateOrUpdateAreaDto {
  areaCode: string;
  areaName: string;
  warehouseId: string;
  warehouseCode: string;
  areaFunction: number;
  storageEnvironment?: number;
  maxCapacity?: number;
  currentCapacity?: number;
  isActive?: boolean;
}

export interface LocationDto {
  id: string;
  locationCode: string;
  warehouseId: string;
  warehouseCode: string;
  areaId: string;
  areaCode: string;
  barcodeId: string;
  locationType: number;
  storageCondition: number;
  maxWeight?: number;
  maxCapacity?: number;
  currentCapacity?: number;
  currentWeight?: number;
  row?: string;
  column?: string;
  layer?: string;
  isActive: boolean;
}

export interface CreateLocationDto {
  locationCode: string;
  warehouseId: string;
  warehouseCode: string;
  areaId: string;
  areaCode: string;
  barcodeId: string;
  locationType?: number;
  storageCondition?: number;
  maxWeight?: number;
  maxCapacity?: number;
  row?: string;
  column?: string;
  layer?: string;
  isActive?: boolean;
}

export interface LocationUpdateDto {
  locationType?: number;
  storageCondition?: number;
  maxWeight?: number;
  maxCapacity?: number;
  row?: string;
  column?: string;
  layer?: string;
  isActive?: boolean;
}

export type CreateOrUpdateLocationDto = CreateLocationDto;

export function getWarehouses(params: PagedParams) {
  return get<PagedResult<WarehouseDto>>('/api/v1/warehouse/warehouses', { params });
}

export function getWarehouse(id: string) {
  return get<WarehouseDto>(`/api/v1/warehouse/warehouses/${id}`);
}

export function createWarehouse(data: CreateOrUpdateWarehouseDto) {
  return post<WarehouseDto>('/api/v1/warehouse/warehouses', data);
}

export function updateWarehouse(id: string, data: CreateOrUpdateWarehouseDto) {
  return put<WarehouseDto>(`/api/v1/warehouse/warehouses/${id}`, data);
}

export function deleteWarehouse(id: string) {
  return del<void>(`/api/v1/warehouse/warehouses/${id}`);
}

export function enableWarehouse(id: string) {
  return patch<void>(`/api/v1/warehouse/warehouses/${id}/activate`);
}

export function disableWarehouse(id: string) {
  return patch<void>(`/api/v1/warehouse/warehouses/${id}/deactivate`);
}

export function getAreas(params: PagedParams & { warehouseId?: string }) {
  return get<PagedResult<AreaDto>>('/api/v1/warehouse/areas', { params });
}

export function getArea(id: string) {
  return get<AreaDto>(`/api/v1/warehouse/areas/${id}`);
}

export function createArea(data: CreateOrUpdateAreaDto) {
  return post<AreaDto>('/api/v1/warehouse/areas', data);
}

export function updateArea(id: string, data: CreateOrUpdateAreaDto) {
  return put<AreaDto>(`/api/v1/warehouse/areas/${id}`, data);
}

export function deleteArea(id: string) {
  return del<void>(`/api/v1/warehouse/areas/${id}`);
}

export function getLocations(params: PagedParams & { warehouseId?: string; areaId?: string }) {
  return get<PagedResult<LocationDto>>('/api/v1/warehouse/locations', { params });
}

export function getLocation(id: string) {
  return get<LocationDto>(`/api/v1/warehouse/locations/${id}`);
}

export function createLocation(data: CreateOrUpdateLocationDto) {
  return post<LocationDto>('/api/v1/warehouse/locations', data);
}

export function updateLocation(id: string, data: LocationUpdateDto) {
  return put<LocationDto>(`/api/v1/warehouse/locations/${id}`, data);
}

export function deleteLocation(id: string) {
  return del<void>(`/api/v1/warehouse/locations/${id}`);
}

export function getLocationMap(warehouseId?: string) {
  if (!warehouseId) {
    return get<any>('/api/v1/warehouse/locations/available');
  }
  return get<any>(`/api/v1/warehouse/locations/available/${warehouseId}`);
}

export function batchCreateLocations(data: CreateOrUpdateLocationDto[]) {
  return post<LocationDto[]>('/api/v1/warehouse/locations', data);
}

export function getLocationsByArea(areaId: string) {
  return get<ListResultDto<LocationDto>>(`/api/v1/warehouse/locations/by-area/${areaId}`);
}
