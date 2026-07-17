import { get, post, put, del } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface BusinessRuleDto {
  id: string;
  code: string;
  name: string;
  ruleType: string;
  description?: string;
  expression?: string;
  priority: number;
  isEnabled: boolean;
}

export interface RuleExecutionRequest {
  ruleType: string;
  input: any;
}

export interface RuleExecutionResult {
  success: boolean;
  output: any;
  messages: string[];
}

export interface IndustryPackageDto {
  id: string;
  code: string;
  name: string;
  industry: string;
  version: string;
  description?: string;
}

export function getBusinessRules(params: PagedParams) {
  return get<PagedResult<BusinessRuleDto>>('/api/v1/rule-engine/rules', { params });
}

export function getBusinessRule(id: string) {
  return get<BusinessRuleDto>(`/api/v1/rule-engine/rules/${id}`);
}

export function createRule(data: BusinessRuleDto) {
  return post<BusinessRuleDto>('/api/v1/rule-engine/rules', data);
}

export function updateRule(id: string, data: BusinessRuleDto) {
  return put<BusinessRuleDto>(`/api/v1/rule-engine/rules/${id}`, data);
}

export function deleteRule(id: string) {
  return del<void>(`/api/v1/rule-engine/rules/${id}`);
}

export function executeRule(data: RuleExecutionRequest) {
  return post<RuleExecutionResult>('/api/v1/rule-engine/executions', data);
}

export function getIndustryPackages(params: PagedParams) {
  return get<PagedResult<IndustryPackageDto>>('/api/v1/rule-engine/industry-packages', { params });
}

export function getIndustryPackage(id: string) {
  return get<IndustryPackageDto>(`/api/v1/rule-engine/industry-packages/${id}`);
}

export function importPackage(id: string, data?: { overwrite?: boolean }) {
  return post<IndustryPackageDto>(`/api/v1/rule-engine/industry-packages/${id}/import`, data);
}
