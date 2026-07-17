import { get, post, put, del, patch } from '@/api/index';
import type { PagedParams, PagedResult } from '@/api/types';

export interface NotificationLogDto {
  id: string;
  notificationType: string;
  title: string;
  content: string;
  receiverId?: string;
  receiverName?: string;
  isRead: boolean;
  readTime?: string;
  creationTime: string;
}

export interface NotificationRuleDto {
  id: string;
  code: string;
  name: string;
  eventType: string;
  channelType: string;
  templateId?: string;
  isEnabled: boolean;
}

export interface NotificationTemplateDto {
  id: string;
  code: string;
  name: string;
  channelType: string;
  subject?: string;
  body: string;
}

export function getNotifications(params: PagedParams & { isRead?: boolean }) {
  return get<PagedResult<NotificationLogDto>>('/api/v1/notification/logs', { params });
}

export function getNotification(id: string) {
  return get<NotificationLogDto>(`/api/v1/notification/logs/${id}`); 
}

export function markAsRead(id: string) {
  return patch<void>(`/api/v1/notification/logs/${id}/read`);
}

export function markAllAsRead() {
  return patch<void>('/api/v1/notification/logs/mark-all-read'); 
}

export function getNotificationRules(params: PagedParams) {
  return get<PagedResult<NotificationRuleDto>>('/api/v1/notification/rules', { params });
}

export function getNotificationRule(id: string) {
  return get<NotificationRuleDto>(`/api/v1/notification/rules/${id}`);
}

export function createNotificationRule(data: NotificationRuleDto) {
  return post<NotificationRuleDto>('/api/v1/notification/rules', data);
}

export function updateNotificationRule(id: string, data: NotificationRuleDto) {
  return put<NotificationRuleDto>(`/api/v1/notification/rules/${id}`, data);
}

export function deleteNotificationRule(id: string) {
  return del<void>(`/api/v1/notification/rules/${id}`);
}

export function getNotificationTemplates(params: PagedParams) {
  return get<PagedResult<NotificationTemplateDto>>('/api/v1/notification/templates', { params });
}

export function getNotificationTemplate(id: string) {
  return get<NotificationTemplateDto>(`/api/v1/notification/templates/${id}`);           
}

export function createNotificationTemplate(data: NotificationTemplateDto) {
  return post<NotificationTemplateDto>('/api/v1/notification/templates', data);
}

export function updateNotificationTemplate(id: string, data: NotificationTemplateDto) {
  return put<NotificationTemplateDto>(`/api/v1/notification/templates/${id}`, data);
}

export function deleteNotificationTemplate(id: string) {
  return del<void>(`/api/v1/notification/templates/${id}`);
}
