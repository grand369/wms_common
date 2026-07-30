import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface TaskDto {
  id: string;
  taskNo: string;
  taskTypeValue: number;
  taskTypeDescription: string;
  taskPriorityValue: number;
  taskPriorityDescription: string;
  taskStatusValue: number;
  taskStatusDescription: string;
  sourceOrderType: string;
  sourceOrderId: string;
  sourceOrderNo: string;
  warehouseId: string;
  warehouseCode: string;
  assignedUserId?: string;
  assignedUserName?: string;
  assignmentStrategyValue: number;
  assignmentStrategyDescription: string;
  expectedCompletionTime?: string;
  actualStartTime?: string;
  actualCompletionTime?: string;
  suspendedReason?: string;
  taskProgress: number;
  remark?: string;
  creationTime: string;
  lastModificationTime?: string;
}

export const TaskStatusEnum = {
  Created: 0,
  Assigned: 1,
  InProgress: 2,
  Suspended: 3,
  Completed: 4,
  Cancelled: 5,
} as const;

export type TaskStatusValue = typeof TaskStatusEnum[keyof TaskStatusEnum];

export interface CreateOrUpdateTaskDto {
  taskNo?: string;
  taskTypeValue: number;
  taskPriorityValue?: number;
  sourceOrderType: string;
  sourceOrderId: string;
  sourceOrderNo: string;
  warehouseId: string;
  warehouseCode: string;
  assignmentStrategyValue?: number;
  expectedCompletionTime?: string;
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
  totalCount?: number;
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

export interface TaskAssignCommandDto {
  userId: string;
  userName: string;
  assignmentStrategyValue?: number;
}

export function assignTask(id: string, data: TaskAssignCommandDto) {
  return patch<TaskDto>(`/api/v1/task-center/tasks/${id}/assign`, data);
}

export function startTask(id: string) {
  return patch<TaskDto>(`/api/v1/task-center/tasks/${id}/start`);
}

export function completeTask(id: string, data?: { remark?: string }) {
  return patch<TaskDto>(`/api/v1/task-center/tasks/${id}/complete`, data);
}

export function suspendTask(id: string, data: { reason: string }) {
  return patch<TaskDto>(`/api/v1/task-center/tasks/${id}/suspend`, data);
}

export function resumeTask(id: string) {
  return patch<TaskDto>(`/api/v1/task-center/tasks/${id}/resume`);
}

export function cancelTask(id: string, data?: { reason?: string }) {
  return patch<TaskDto>(`/api/v1/task-center/tasks/${id}/cancel`, data);
}

export function updateTaskProgress(id: string, data: { progress: number }) {
  return patch<TaskDto>(`/api/v1/task-center/tasks/${id}/update-progress`, data);
}

export function getTaskMonitor() {
  return get<TaskMonitorDto>('/api/v1/task-center/tasks/monitor');
}

export function getTaskStatistics(params?: { startDate?: string; endDate?: string }) {
  return get<TaskStatisticsDto>('/api/v1/task-center/tasks/statistics', { params });
}
