import { get, post, put, del } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface DictionaryDto {
  id: string;
  dictionaryCode: string;
  dictionaryName: string;
  description?: string;
  sortOrder: number;
  isActive: boolean;
  creationTime: string;
}

export interface DictionaryCreateDto {
  dictionaryCode: string;
  dictionaryName: string;
  description?: string;
  sortOrder?: number;
  isActive?: boolean;
}

export interface DictionaryUpdateDto {
  dictionaryName: string;
  description?: string;
  sortOrder?: number;
  isActive?: boolean;
}

export interface DictionaryItemDto {
  id: string;
  dictionaryId: string;
  dictionaryCode: string;
  dictionaryName: string;
  itemCode: string;
  itemName: string;
  itemValue?: string;
  description?: string;
  sortOrder: number;
  isActive: boolean;
}

export interface DictionaryItemCreateDto {
  dictionaryId: string;
  itemCode: string;
  itemName: string;
  itemValue?: string;
  description?: string;
  sortOrder?: number;
  isActive?: boolean;
}

export interface DictionaryItemUpdateDto {
  itemName: string;
  itemValue?: string;
  description?: string;
  sortOrder?: number;
  isActive?: boolean;
}

export function getDictionaries(params?: PagedParams) {
  return get<PagedResult<DictionaryDto>>('/api/v1/data-dictionary/dictionaries', { params });
}

export function getDictionary(id: string) {
  return get<DictionaryDto>(`/api/v1/data-dictionary/dictionaries/${id}`);
}

export function createDictionary(data: DictionaryCreateDto) {
  return post<DictionaryDto>('/api/v1/data-dictionary/dictionaries', data);
}

export function updateDictionary(id: string, data: DictionaryUpdateDto) {
  return put<DictionaryDto>(`/api/v1/data-dictionary/dictionaries/${id}`, data);
}

export function deleteDictionary(id: string) {
  return del<void>(`/api/v1/data-dictionary/dictionaries/${id}`);
}

export function getDictionaryItems(dictionaryId: string) {
  return get<DictionaryItemDto[]>(`/api/v1/data-dictionary/items/by-dictionary/${dictionaryId}`);
}

export function getDictionaryItemsByCode(dictionaryCode: string) {
  return get<DictionaryItemDto[]>(`/api/v1/data-dictionary/items/by-code/${dictionaryCode}`);
}

export function getDictionaryItem(id: string) {
  return get<DictionaryItemDto>(`/api/v1/data-dictionary/items/${id}`);
}

export function createDictionaryItem(data: DictionaryItemCreateDto) {
  return post<DictionaryItemDto>('/api/v1/data-dictionary/items', data);
}

export function updateDictionaryItem(id: string, data: DictionaryItemUpdateDto) {
  return put<DictionaryItemDto>(`/api/v1/data-dictionary/items/${id}`, data);
}

export function deleteDictionaryItem(id: string) {
  return del<void>(`/api/v1/data-dictionary/items/${id}`);
}
