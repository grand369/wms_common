import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface BarcodeRuleDto {
  id: string;
  code: string;
  name: string;
  ruleType: string;
  pattern: string;
  prefix?: string;
  suffix?: string;
  segmentRules: BarcodeRuleSegmentDto[];
  status: number;
}

export interface BarcodeRuleSegmentDto {
  segmentType: string;
  length: number;
  format?: string;
  description?: string;
}

export interface LabelTemplateDto {
  id: string;
  code: string;
  name: string;
  templateType: string;
  width: number;
  height: number;
  content: string;
  status: number;
}

export interface PrintJobDto {
  id: string;
  jobNo: string;
  jobType: string;
  status: number;
  printerName?: string;
  printedTime?: string;
}

export function getBarcodeRules(params: PagedParams) {
  return get<PagedResult<BarcodeRuleDto>>('/api/v1/barcode-label/rules', { params });
}

export function getBarcodeRule(id: string) {
  return get<BarcodeRuleDto>(`/api/v1/barcode-label/rules/${id}`);
}

export function createRule(data: BarcodeRuleDto) {
  return post<BarcodeRuleDto>('/api/v1/barcode-label/rules', data);
}

export function updateRule(id: string, data: BarcodeRuleDto) {
  return put<BarcodeRuleDto>(`/api/v1/barcode-label/rules/${id}`, data);
}

export function deleteRule(id: string) {
  return del<void>(`/api/v1/barcode-label/rules/${id}`);
}

export function getLabelTemplates(params: PagedParams) {
  return get<PagedResult<LabelTemplateDto>>('/api/v1/barcode-label/templates', { params });
}

export function getLabelTemplate(id: string) {
  return get<LabelTemplateDto>(`/api/v1/barcode-label/templates/${id}`);
}

export function createTemplate(data: LabelTemplateDto) {
  return post<LabelTemplateDto>('/api/v1/barcode-label/templates', data);
}

export function updateTemplate(id: string, data: LabelTemplateDto) {
  return put<LabelTemplateDto>(`/api/v1/barcode-label/templates/${id}`, data);
}

export function deleteTemplate(id: string) {
  return del<void>(`/api/v1/barcode-label/templates/${id}`);
}

export function getPrintJobs(params: PagedParams) {
  return get<PagedResult<PrintJobDto>>('/api/v1/barcode-label/print-jobs', { params });
}

export function getPrintJob(id: string) {
  return get<PrintJobDto>(`/api/v1/barcode-label/print-jobs/${id}`);
}

export function createPrintJob(data: { templateId: string; quantity: number; data?: any }) {
  return post<PrintJobDto>('/api/v1/barcode-label/print-jobs', data);
}

export function retryPrint(id: string) {
  return patch<PrintJobDto>(`/api/v1/barcode-label/print-jobs/${id}/retry`);
}

export function cancelPrintJob(id: string) {
  return patch<void>(`/api/v1/barcode-label/print-jobs/${id}/cancel`);
}

export function deletePrintJob(id: string) {
  return del<void>(`/api/v1/barcode-label/print-jobs/${id}`);
}
