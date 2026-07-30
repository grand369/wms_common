import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult, ListResultDto } from '@/api/types';

export interface SupplierDto {
  id: string;
  supplierCode: string;
  supplierName: string;
  shortName?: string;
  supplierType: number;
  supplierTypeDescription?: string;
  contactName?: string;
  contactPhone?: string;
  contactEmail?: string;
  address?: string;
  city?: string;
  province?: string;
  postalCode?: string;
  taxId?: string;
  bankName?: string;
  bankAccount?: string;
  isActive: boolean;
  remark?: string;
  erpSupplierCode?: string;
  creationTime: string;
  creatorId?: string;
}

export interface CreateSupplierDto {
  supplierCode: string;
  supplierName: string;
  shortName?: string;
  supplierType?: number;
  contactName?: string;
  contactPhone?: string;
  contactEmail?: string;
  address?: string;
  city?: string;
  province?: string;
  postalCode?: string;
  taxId?: string;
  bankName?: string;
  bankAccount?: string;
  isActive?: boolean;
  remark?: string;
  erpSupplierCode?: string;
}

export interface UpdateSupplierDto {
  supplierName: string;
  shortName?: string;
  supplierType?: number;
  contactName?: string;
  contactPhone?: string;
  contactEmail?: string;
  address?: string;
  city?: string;
  province?: string;
  postalCode?: string;
  taxId?: string;
  bankName?: string;
  bankAccount?: string;
  isActive?: boolean;
  remark?: string;
  erpSupplierCode?: string;
}

export interface SupplierQueryDto {
  supplierCode?: string;
  supplierName?: string;
  filter?: string;
  supplierType?: number;
  isActive?: boolean;
  skipCount?: number;
  maxResultCount?: number;
}

export function getSuppliers(params?: SupplierQueryDto & PagedParams) {
  return get<PagedResult<SupplierDto>>('/api/v1/supplier/suppliers', { params });
}

export function getSupplier(id: string) {
  return get<SupplierDto>(`/api/v1/supplier/suppliers/${id}`);
}

export function getSupplierByCode(supplierCode: string) {
  return get<SupplierDto>(`/api/v1/supplier/suppliers/by-code/${supplierCode}`);
}

export function getActiveSuppliers() {
  return get<SupplierDto[] | ListResultDto<SupplierDto>>('/api/v1/supplier/suppliers/active');
}

export function createSupplier(data: CreateSupplierDto) {
  return post<SupplierDto>('/api/v1/supplier/suppliers', data);
}

export function updateSupplier(id: string, data: UpdateSupplierDto) {
  return put<SupplierDto>(`/api/v1/supplier/suppliers/${id}`, data);
}

export function deleteSupplier(id: string) {
  return del<void>(`/api/v1/supplier/suppliers/${id}`);
}

export function enableSupplier(id: string) {
  return patch<SupplierDto>(`/api/v1/supplier/suppliers/${id}/activate`);
}

export function disableSupplier(id: string) {
  return patch<SupplierDto>(`/api/v1/supplier/suppliers/${id}/deactivate`);
}
