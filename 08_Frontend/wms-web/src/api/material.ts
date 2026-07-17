import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult, ListResultDto } from '@/api/types';

export interface MaterialDto {
  id: string;
  code: string;
  name: string;
  specification?: string;
  unit: string;
  classificationId?: string;
  classificationName?: string;
  issueStrategyId?: string;
  status: number;
  isBatchEnabled?: boolean;
  isSerialEnabled?: boolean;
}

export interface CreateOrUpdateMaterialDto {
  code: string;
  name: string;
  specification?: string;
  unit: string;
  classificationId?: string;
  issueStrategyId?: string;
  isBatchEnabled?: boolean;
  isSerialEnabled?: boolean;
}

export interface MaterialClassificationDto {
  id: string;
  classificationCode: string;
  classificationName: string;
  parentClassificationId?: string;
  parentClassificationName?: string;
  classificationLevel: number;
  attributeTemplateId?: string;
  children?: MaterialClassificationDto[];
}

export interface CreateMaterialClassificationDto {
  classificationCode: string;
  classificationName: string;
  parentClassificationId?: string;
  classificationLevel?: number;
  attributeTemplateId?: string;
}

export interface UpdateMaterialClassificationDto {
  classificationName: string;
  parentClassificationId?: string;
  classificationLevel?: number;
  attributeTemplateId?: string;
}

export interface MaterialIssueStrategyDto {
  id: string;
  code: string;
  name: string;
  strategy: string;
  description?: string;
}

export interface AddSubstituteRequest {
  substituteMaterialId: string;
  substituteMaterialCode: string;
  priority?: number;
  ratio?: number;
}

export interface MaterialSubstituteDto {
  id: string;
  materialId: string;
  substituteMaterialId: string;
  substituteMaterialCode: string;
  substituteMaterialName: string;
  priority: number;
  ratio: number;
}

export function getMaterials(params: PagedParams) {
  return get<PagedResult<MaterialDto>>('/api/v1/material/materials', { params });
}

export function getMaterial(id: string) {
  return get<MaterialDto>(`/api/v1/material/materials/${id}`);
}

export function createMaterial(data: CreateOrUpdateMaterialDto) {
  return post<MaterialDto>('/api/v1/material/materials', data);
}

export function updateMaterial(id: string, data: CreateOrUpdateMaterialDto) {
  return put<MaterialDto>(`/api/v1/material/materials/${id}`, data);
}

export function deleteMaterial(id: string) {
  return del<void>(`/api/v1/material/materials/${id}`);
}

export function enableMaterial(id: string) {
  return patch<void>(`/api/v1/material/materials/${id}/activate`);
}

export function disableMaterial(id: string) {
  return patch<void>(`/api/v1/material/materials/${id}/deactivate`);
}

export function getClassifications(params: PagedParams) {
  return get<PagedResult<MaterialClassificationDto>>('/api/v1/material/classifications', { params });
}

export function getClassification(id: string) {
  return get<MaterialClassificationDto>(`/api/v1/material/classifications/${id}`);
}

export function createClassification(data: CreateMaterialClassificationDto) {
  return post<MaterialClassificationDto>('/api/v1/material/classifications', data);
}

export function updateClassification(id: string, data: UpdateMaterialClassificationDto) {
  return put<MaterialClassificationDto>(`/api/v1/material/classifications/${id}`, data);
}

export function deleteClassification(id: string) {
  return del<void>(`/api/v1/material/classifications/${id}`);
}

export function getIssueStrategies(params: PagedParams) {
  return get<PagedResult<MaterialIssueStrategyDto>>('/api/v1/material/issue-strategies', { params });
}

export function getIssueStrategy(id: string) {
  return get<MaterialIssueStrategyDto>(`/api/v1/material/issue-strategies/${id}`);
}

export function createIssueStrategy(data: MaterialIssueStrategyDto) {
  return post<MaterialIssueStrategyDto>('/api/v1/material/issue-strategies', data);
}

export function updateIssueStrategy(id: string, data: MaterialIssueStrategyDto) {
  return put<MaterialIssueStrategyDto>(`/api/v1/material/issue-strategies/${id}`, data);
}

export function deleteIssueStrategy(id: string) {
  return del<void>(`/api/v1/material/issue-strategies/${id}`);
}

export function getMaterialSubstitutes(id: string) {
  return get<ListResultDto<MaterialSubstituteDto>>(`/api/v1/material/materials/${id}/substitutes`);
}

export function addMaterialSubstitute(id: string, data: AddSubstituteRequest) {
  return post<MaterialSubstituteDto>(`/api/v1/material/materials/${id}/substitutes`, data);
}

export function removeMaterialSubstitute(id: string, substituteRelationId: string) {
  return del<void>(`/api/v1/material/materials/${id}/substitutes/${substituteRelationId}`);
}
