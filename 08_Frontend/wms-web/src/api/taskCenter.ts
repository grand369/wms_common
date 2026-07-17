import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface TaskDto {
  id: string;
  taskNo: string;
  taskType: string;
  sourceDocType: string;
  sourceDocId: string;
  status: number;
  priority: number;
  assigneeId?: string;
  assigneeName?: string;
  dueTime?: string;
  completedTime?: string;
}

export interface CreateOrUpdateTaskDto {
  taskType: string;
  sourceDocType: string;
  sourceDocId: string;
  priority?: number;
  dueTime?: string;
  remark?: string;
}

export interface TaskCommentDto {
  id: string;
  taskId: string;
  content: string;
  creatorName: string;
  creationTime: string;
}

export interface TaskMonitorDto {
  pendingCount: number;
  inProgressCount: number;
  completedCount: number;
  exceptionCount: number;
}

export interface TaskStatisticsDto {
  totalCount: number;
  completedCount: number;
  pendingCount: number;
  exceptionCount: number;
  completionRate: number;
}

export function getTasks(params: PagedParams) {
  return get<PagedResult<TaskDto>>('/api/v1/task-center/tasks', { params });
}

export function getTask(id: string) {
  return get<TaskDto>(`/api/v1/task-center/tasks/${id}`);
}

export function createTask(data: CreateOrUpdateTaskDto) {
  return post<TaskDto>('/api/v1/task-center/tasks', data);
}

export function updateTask(id: string, data: CreateOrUpdateTaskDto) {
  return put<TaskDto>(`/api/v1/task-center/tasks/${id}`, data);
}

export function deleteTask(id: string) {
  return del<void>(`/api/v1/task-center/tasks/${id}`);
}

export function assignTask(id: string, data: { assigneeId: string }) {
  return patch<void>(`/api/v1/task-center/tasks/${id}/assign`, data);
}

export function startTask(id: string) {
  return patch<void>(`/api/v1/task-center/tasks/${id}/start`);
}

export function completeTask(id: string, data?: { result?: string }) {
  return patch<void>(`/api/v1/task-center/tasks/${id}/complete`, data);
}

export function suspendTask(id: string, data?: { reason?: string }) {
  return patch<void>(`/api/v1/task-center/tasks/${id}/suspend`, data);
}

export function resumeTask(id: string) {
  return patch<void>(`/api/v1/task-center/tasks/${id}/resume`);
}

export function reportException(id: string, data: { exceptionType: string; description: string }) {
  return patch<void>(`/api/v1/task-center/tasks/${id}/update-progress`, data);
}

export function addTaskComment(id: string, data: { content: string }) {
  return post<TaskCommentDto>(`/api/v1/task-center/tasks/${id}/update-progress`, data);
}

export function getTaskMonitor() {
  return get<TaskMonitorDto>('/api/v1/task-center/tasks');
}

export function getTaskStatistics(params?: { startDate?: string; endDate?: string }) {
  return get<TaskStatisticsDto>('/api/v1/task-center/tasks/statistics', { params });
}
