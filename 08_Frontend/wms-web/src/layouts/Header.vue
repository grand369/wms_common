<template>
  <el-header class="wms-header">
    <div class="header-left">
      <!-- Collapse Toggle -->
      <div class="collapse-toggle" @click="handleToggleCollapse">
        <el-icon :size="18">
          <Fold v-if="!isCollapsed" />
          <Expand v-else />
        </el-icon>
      </div>

      <!-- Breadcrumb -->
      <el-breadcrumb separator="/" class="header-breadcrumb">
        <el-breadcrumb-item :to="{ path: '/' }">
          <el-icon><HomeFilled /></el-icon>
          <span>首页</span>
        </el-breadcrumb-item>
        <el-breadcrumb-item
          v-for="crumb in breadcrumbs"
          :key="crumb.path"
          :to="crumb.path ? { path: crumb.path } : undefined"
        >
          {{ crumb.label }}
        </el-breadcrumb-item>
      </el-breadcrumb>
    </div>

    <div class="header-right">
      <!-- Global Search -->
      <div class="global-search">
        <el-input
          v-model="searchKeyword"
          placeholder="搜索菜单、单据..."
          :prefix-icon="Search"
          size="default"
          class="search-input"
          clearable
          @keyup.enter="handleSearch"
        />
      </div>

      <!-- Notification Bell -->
      <el-popover
        placement="bottom-end"
        :width="360"
        trigger="click"
        popper-class="notification-popover"
      >
        <template #reference>
          <div class="header-action notification-bell">
            <el-badge :value="unreadCount" :max="99" :hidden="unreadCount === 0">
              <el-icon :size="18"><Bell /></el-icon>
            </el-badge>
          </div>
        </template>
        <div class="notification-panel">
          <div class="notification-header">
            <span class="notification-title">消息通知</span>
            <el-button link type="primary" size="small" @click="handleMarkAllRead">
              全部已读
            </el-button>
          </div>
          <div class="notification-list" v-if="unreadCount > 0">
            <div
              v-for="item in unreadNotifications"
              :key="item.id"
              class="notification-item"
              :class="`type-${item.type}`"
              @click="handleMarkRead(item.id)"
            >
              <div class="notify-icon">
                <el-icon v-if="item.type === 'warning'"><WarningFilled /></el-icon>
                <el-icon v-else-if="item.type === 'success'"><SuccessFilled /></el-icon>
                <el-icon v-else-if="item.type === 'error'"><CircleCloseFilled /></el-icon>
                <el-icon v-else><InfoFilled /></el-icon>
              </div>
              <div class="notify-content">
                <div class="notify-title">{{ item.title }}</div>
                <div class="notify-text">{{ item.content }}</div>
                <div class="notify-time">{{ item.createdAt }}</div>
              </div>
            </div>
          </div>
          <div v-else class="notification-empty">暂无未读消息</div>
        </div>
      </el-popover>

      <!-- User Dropdown -->
      <el-dropdown trigger="click" class="user-dropdown" @command="handleUserCommand">
        <div class="header-action user-info">
          <el-avatar :size="28" class="user-avatar">
            {{ userInitial }}
          </el-avatar>
          <span class="user-name">{{ displayName }}</span>
          <el-icon :size="12" class="dropdown-arrow"><ArrowDown /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="profile">
              <el-icon><User /></el-icon>
              个人设置
            </el-dropdown-item>
            <el-dropdown-item divided command="logout">
              <el-icon><SwitchButton /></el-icon>
              退出登录
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </el-header>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAppStore } from '@/stores/app';
import { useAuthStore } from '@/stores/auth';
import { useNotificationStore } from '@/stores/notification';
import { ElMessage } from 'element-plus';
import {
  Fold,
  Expand,
  HomeFilled,
  Search,
  Bell,
  WarningFilled,
  SuccessFilled,
  CircleCloseFilled,
  InfoFilled,
  ArrowDown,
  User,
  SwitchButton,
} from '@element-plus/icons-vue';

// ── Stores ────────────────────────────────────────────────────────
const appStore = useAppStore();
const authStore = useAuthStore();
const notificationStore = useNotificationStore();
const route = useRoute();
const router = useRouter();

// ── State ─────────────────────────────────────────────────────────
const searchKeyword = ref('');

// Initialize mock notifications in dev
notificationStore.loadMockData();

// ── Computed ──────────────────────────────────────────────────────
const isCollapsed = computed(() => appStore.isCollapsed);
const unreadCount = computed(() => notificationStore.unreadCount);
const unreadNotifications = computed(() => notificationStore.unreadNotifications);

const displayName = computed(() => authStore.username || '管理员');
const userInitial = computed(() =>
  (displayName.value || '管').charAt(0).toUpperCase()
);

// Breadcrumb from route meta
interface BreadcrumbItem {
  label: string;
  path?: string;
}

const breadcrumbs = computed<BreadcrumbItem[]>(() => {
  const items: BreadcrumbItem[] = [];

  // Current route title
  const currentTitle = (route.meta.title as string) || undefined;
  if (currentTitle && route.path !== '/') {
    items.push({ label: currentTitle, path: undefined });
  }

  return items;
});

// ── Methods ───────────────────────────────────────────────────────
function handleToggleCollapse() {
  appStore.toggleCollapse();
}

function handleSearch() {
  const keyword = searchKeyword.value.trim();
  if (!keyword) return;
  ElMessage.info(`搜索: ${keyword}`);
  // Placeholder: future implementation will navigate to global search page
}

function handleMarkRead(id: string) {
  notificationStore.markAsRead(id);
}

function handleMarkAllRead() {
  notificationStore.markAllAsRead();
  ElMessage.success('已标记全部已读');
}

function handleUserCommand(command: string) {
  if (command === 'logout') {
    authStore.logout();
    router.push('/login');
  } else if (command === 'profile') {
    // Placeholder for profile page
    ElMessage.info('个人设置页面开发中');
  }
}
</script>

<style scoped lang="scss">
.wms-header {
  height: $wms-header-height;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: $wms-bg-content;
  border-bottom: 1px solid $wms-border-base;
  padding: 0 $wms-spacing-md;
  z-index: $wms-z-header;
  flex-shrink: 0;
}

// ── Left Section ──────────────────────────────────────────────────
.header-left {
  display: flex;
  align-items: center;
  gap: $wms-spacing-sm;
  flex: 1;
  min-width: 0;
}

.collapse-toggle {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: $wms-radius-md;
  cursor: pointer;
  color: $wms-text-regular;
  transition: all $wms-transition-hover;
  flex-shrink: 0;

  &:hover {
    background-color: $wms-color-primary-light-9;
    color: $wms-color-primary;
  }
}

.header-breadcrumb {
  flex: 1;
  min-width: 0;

  :deep(.el-breadcrumb__item) {
    display: inline-flex;
    align-items: center;
    gap: 4px;
  }

  :deep(.el-breadcrumb__inner) {
    color: $wms-text-secondary;
    font-weight: 400;
    font-size: $wms-font-size-small;

    &:hover {
      color: $wms-color-primary;
    }
  }

  :deep(.el-breadcrumb__item:last-child .el-breadcrumb__inner) {
    color: $wms-text-primary;
    font-weight: 500;
  }
}

// ── Right Section ─────────────────────────────────────────────────
.header-right {
  display: flex;
  align-items: center;
  gap: $wms-spacing-sm;
  flex-shrink: 0;
}

// Global Search
.global-search {
  .search-input {
    width: 200px;

    :deep(.el-input__wrapper) {
      background-color: $wms-bg-base;
      border-radius: 20px;
      box-shadow: none;
      padding: 0 12px;
      transition: all $wms-transition-hover;

      &:hover,
      &.is-focus {
        background-color: #fff;
        box-shadow: 0 0 0 1px $wms-color-primary inset;
      }
    }

    :deep(.el-input__inner) {
      font-size: $wms-font-size-small;

      &::placeholder {
        color: $wms-text-secondary;
      }
    }

    :deep(.el-input__prefix) {
      color: $wms-text-secondary;
    }
  }
}

// Notification Bell
.header-action {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: $wms-radius-md;
  cursor: pointer;
  color: $wms-text-regular;
  transition: all $wms-transition-hover;

  &:hover {
    background-color: $wms-color-primary-light-9;
    color: $wms-color-primary;
  }
}

// User Info
.user-info {
  width: auto;
  padding: 0 $wms-spacing-sm 0 $wms-spacing-xs;
  gap: $wms-spacing-xs;
  cursor: pointer;
  user-select: none;

  .user-avatar {
    background-color: $wms-color-primary;
    color: #ffffff;
    font-size: $wms-font-size-small;
    font-weight: 600;
  }

  .user-name {
    font-size: $wms-font-size-small;
    color: $wms-text-primary;
    max-width: 80px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .dropdown-arrow {
    color: $wms-text-secondary;
  }
}

.user-dropdown {
  :deep(.el-dropdown-menu__item) {
    display: flex;
    align-items: center;
    gap: $wms-spacing-xs;
    font-size: $wms-font-size-body;
  }
}
</style>

<!-- Notification Popover Styles (global because popper is appended to body) -->
<style lang="scss">
.notification-popover {
  padding: 0 !important;
  border-radius: $wms-radius-lg !important;
  box-shadow: $wms-shadow-md !important;

  .notification-panel {
    max-height: 400px;
    display: flex;
    flex-direction: column;
  }

  .notification-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: $wms-spacing-sm $wms-spacing-md;
    border-bottom: 1px solid $wms-border-base;

    .notification-title {
      font-size: $wms-font-size-body;
      font-weight: 600;
      color: $wms-text-primary;
    }
  }

  .notification-list {
    overflow-y: auto;
    max-height: 340px;
  }

  .notification-item {
    display: flex;
    gap: $wms-spacing-sm;
    padding: $wms-spacing-sm $wms-spacing-md;
    cursor: pointer;
    transition: background-color $wms-transition-hover;

    &:hover {
      background-color: $wms-color-primary-light-9;
    }

    + .notification-item {
      border-top: 1px solid $wms-border-base;
    }

    .notify-icon {
      flex-shrink: 0;
      width: 28px;
      height: 28px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      font-size: 14px;
      margin-top: 2px;
    }

    &.type-warning .notify-icon {
      color: $wms-color-warning;
      background-color: rgba(217, 119, 6, 0.1);
    }

    &.type-success .notify-icon {
      color: $wms-color-success;
      background-color: rgba(22, 163, 74, 0.1);
    }

    &.type-error .notify-icon {
      color: $wms-color-danger;
      background-color: rgba(220, 38, 38, 0.1);
    }

    &.type-info .notify-icon {
      color: $wms-color-info;
      background-color: rgba(14, 165, 233, 0.1);
    }

    .notify-content {
      flex: 1;
      min-width: 0;
    }

    .notify-title {
      font-size: $wms-font-size-body;
      font-weight: 500;
      color: $wms-text-primary;
      margin-bottom: 2px;
    }

    .notify-text {
      font-size: $wms-font-size-small;
      color: $wms-text-regular;
      line-height: 1.5;
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }

    .notify-time {
      font-size: 11px;
      color: $wms-text-secondary;
      margin-top: 4px;
    }
  }

  .notification-empty {
    padding: $wms-spacing-xl;
    text-align: center;
    color: $wms-text-secondary;
    font-size: $wms-font-size-small;
  }
}
</style>
