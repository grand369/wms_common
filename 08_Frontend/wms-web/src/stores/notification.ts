import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export interface NotificationItem {
  id: string;
  title: string;
  content: string;
  type: 'info' | 'warning' | 'success' | 'error';
  read: boolean;
  createdAt: string;
}

export const useNotificationStore = defineStore('notification', () => {
  const notifications = ref<NotificationItem[]>([]);

  const unreadCount = computed(() =>
    notifications.value.filter((n) => !n.read).length
  );

  const unreadNotifications = computed(() =>
    notifications.value.filter((n) => !n.read)
  );

  function setNotifications(items: NotificationItem[]) {
    notifications.value = items;
  }

  function addNotification(item: NotificationItem) {
    notifications.value.unshift(item);
  }

  function markAsRead(id: string) {
    const item = notifications.value.find((n) => n.id === id);
    if (item) {
      item.read = true;
    }
  }

  function markAllAsRead() {
    notifications.value.forEach((n) => {
      n.read = true;
    });
  }

  function clearAll() {
    notifications.value = [];
  }

  // Load mock data for development
  function loadMockData() {
    notifications.value = [
      {
        id: '1',
        title: '库存预警',
        content: '物料 PN-2024001 库存低于安全库存阈值，请及时补货。',
        type: 'warning',
        read: false,
        createdAt: '2025-06-30 10:30:00',
      },
      {
        id: '2',
        title: '入库完成',
        content: '入库单 IN-20240630-001 已完成上架。',
        type: 'success',
        read: false,
        createdAt: '2025-06-30 09:45:00',
      },
      {
        id: '3',
        title: '审核提醒',
        content: '出库单 OUT-20240630-003 等待您审核。',
        type: 'info',
        read: false,
        createdAt: '2025-06-30 08:20:00',
      },
      {
        id: '4',
        title: '系统通知',
        content: '系统将于本周六凌晨 2:00 进行维护升级。',
        type: 'info',
        read: true,
        createdAt: '2025-06-29 14:00:00',
      },
    ];
  }

  return {
    notifications,
    unreadCount,
    unreadNotifications,
    setNotifications,
    addNotification,
    markAsRead,
    markAllAsRead,
    clearAll,
    loadMockData,
  };
});
