import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface WorkflowDefinitionDto {
  id: string;
  code: string;
  name: string;
  entityType: string;
  version: number;
  isPublished: boolean;
  nodes: WorkflowNodeDto[];
}

export interface WorkflowNodeDto {
  id: string;
  nodeType: string;
  approverId?: string;
  approverName?: string;
  nextNodeId?: string;
}

export interface ApprovalInstanceDto {
  id: string;
  workflowDefinitionId: string;
  workflowName: string;
  businessEntityType: string;
  businessEntityId: string;
  status: number;
  currentNodeId?: string;
  currentNodeName?: string;
  applicantName?: string;
  creationTime: string;
}

export interface ApprovalHistoryDto {
  id: string;
  instanceId: string;
  nodeName: string;
  action: string;
  comment?: string;
  operatorName?: string;
  operationTime: string;
}

export function getApprovalFlowDefinitions(params: PagedParams) {
  return get<PagedResult<WorkflowDefinitionDto>>('/api/v1/workflow/definitions', { params });
}

export function getApprovalFlowDefinition(id: string) {
  return get<WorkflowDefinitionDto>(`/api/v1/workflow/definitions/${id}`);
}

export function createDefinition(data: WorkflowDefinitionDto) {
  return post<WorkflowDefinitionDto>('/api/v1/workflow/definitions', data);
}

export function updateDefinition(id: string, data: WorkflowDefinitionDto) {
  return put<WorkflowDefinitionDto>(`/api/v1/workflow/definitions/${id}`, data);
}

export function deleteDefinition(id: string) {
  return del<void>(`/api/v1/workflow/definitions/${id}`);
}

export function publishDefinition(id: string) {
  return patch<void>(`/api/v1/workflow/definitions/${id}/publish`);
}

export function getApprovalInstances(params: PagedParams) {
  return get<PagedResult<ApprovalInstanceDto>>('/api/v1/workflow/instances', { params });
}

export function getApprovalInstance(id: string) {
  return get<ApprovalInstanceDto>(`/api/v1/workflow/instances/${id}`);
}

export function createApprovalInstance(data: {
  workflowDefinitionId: string;
  businessEntityType: string;
  businessEntityId: string;
}) {
  return post<ApprovalInstanceDto>('/api/v1/workflow/approvals', data);
}

export function approveInstance(id: string, data?: { comment?: string }) {
  return patch<void>(`/api/v1/workflow/instances/${id}/approve`, data);
}

export function rejectInstance(id: string, data?: { comment?: string }) {
  return patch<void>(`/api/v1/workflow/instances/${id}/reject`, data);
}

export function getApprovalHistory(instanceId: string) {
  return get<{ items: ApprovalHistoryDto[] }>(`/api/v1/workflow/instances/${instanceId}/history`);
}
