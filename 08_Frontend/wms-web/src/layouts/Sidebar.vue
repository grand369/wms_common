<template>
  <el-aside
    class="wms-sidebar"
    :width="isCollapsed ? SIDEBAR_COLLAPSED_WIDTH : SIDEBAR_WIDTH"
  >
    <!-- Logo Area -->
    <div class="sidebar-logo" :class="{ collapsed: isCollapsed }">
      <div class="logo-icon">
        <svg viewBox="0 0 32 32" width="28" height="28" fill="none">
          <rect width="32" height="32" rx="6" fill="#2563EB" />
          <path
            d="M8 10h6v12H8zM13 14h6v8h-6zM18 10h6v12h-6z"
            fill="#fff"
            opacity="0.9"
          />
        </svg>
      </div>
      <transition name="fade">
        <span v-if="!isCollapsed" class="logo-text">WMS 制造仓储</span>
      </transition>
    </div>

    <!-- Navigation Menu -->
    <el-scrollbar class="sidebar-menu-scroll">
      <el-menu
        :default-active="activeMenu"
        :collapse="isCollapsed"
        :collapse-transition="false"
        background-color="transparent"
        text-color="rgba(255,255,255,0.65)"
        active-text-color="#FFFFFF"
        router
        class="wms-menu"
      >
        <!-- Dashboard -->
        <el-menu-item index="/">
          <el-icon><Monitor /></el-icon>
          <template #title>首页概览</template>
        </el-menu-item>

        <!-- Module groups filtered by permissions -->
        <template v-for="group in filteredMenuGroups" :key="group.moduleKey">
          <el-sub-menu
            v-if="group.children.length > 0"
            :index="group.moduleKey"
            :popper-offset="12"
          >
            <template #title>
              <el-icon><component :is="group.icon" /></el-icon>
              <span>{{ group.label }}</span>
            </template>
            <el-menu-item
              v-for="item in group.children"
              :key="item.path"
              :index="item.path"
            >
              {{ item.label }}
            </el-menu-item>
          </el-sub-menu>
        </template>

        <!-- System -->
        <el-sub-menu index="system" :popper-offset="12">
          <template #title>
            <el-icon><Setting /></el-icon>
            <span>系统管理</span>
          </template>
          <el-menu-item index="/system/users">用户管理</el-menu-item>
          <el-menu-item index="/system/roles">角色管理</el-menu-item>
          <el-menu-item index="/system/permissions">权限管理</el-menu-item>
          <el-menu-item index="/system/organization">组织架构</el-menu-item>
          <el-menu-item index="/system/settings">系统设置</el-menu-item>
        </el-sub-menu>
      </el-menu>
    </el-scrollbar>
  </el-aside>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import { useAppStore } from '@/stores/app';
import { useAuthStore } from '@/stores/auth';
import {
  Monitor,
  OfficeBuilding,
  Box,
  DataBoard,
  Download,
  Upload,
  List,
  Switch,
  Finished,
  SetUp,
  Promotion,
  Ticket,
  Share,
  Operation,
  Bell,
  UserFilled,
  Setting,
  Notebook,
} from '@element-plus/icons-vue';
import type { Component } from 'vue';

// ── Menu Item ─────────────────────────────────────────────────────
interface MenuItem {
  label: string;
  path: string;
  permission?: string;
}

// ── Menu Group ────────────────────────────────────────────────────
interface MenuGroup {
  moduleKey: string;
  label: string;
  icon: Component;
  permission: string;
  children: MenuItem[];
}

// ── Layout Constants ────────────────────────────────────────────
const SIDEBAR_WIDTH = '220px';
const SIDEBAR_COLLAPSED_WIDTH = '64px';

// ── Complete Menu Data ────────────────────────────────────────────
const menuGroups: MenuGroup[] = [
  {
    moduleKey: 'warehouse',
    label: '仓库管理',
    icon: OfficeBuilding,
    permission: 'Wms.Warehouse',
    children: [
      { label: '仓库列表', path: '/warehouse/list' },
      { label: '库区管理', path: '/warehouse/areas' },
      { label: '库位管理', path: '/warehouse/locations' },
      { label: '库位地图', path: '/warehouse/location-map' },
    ],
  },
  {
    moduleKey: 'material',
    label: '物料管理',
    icon: Box,
    permission: 'Wms.Material',
    children: [
      { label: '物料列表', path: '/material/list' },
      { label: '分类管理', path: '/material/classifications' },
      { label: '发料策略', path: '/material/issue-strategies' },
    ],
  },
  {
    moduleKey: 'inventory',
    label: '库存管理',
    icon: DataBoard,
    permission: 'Wms.Inventory',
    children: [
      { label: '库存余额', path: '/inventory/balances' },
      { label: '库存台账', path: '/inventory/ledger' },
      { label: '库存预警', path: '/inventory/alerts' },
      { label: '冻结/解冻', path: '/inventory/freeze' },
      { label: '库存调整', path: '/inventory/adjustments' },
      { label: '库存快照', path: '/inventory/snapshots' },
    ],
  },
  {
    moduleKey: 'inbound',
    label: '入库管理',
    icon: Download,
    permission: 'Wms.Inbound',
    children: [
      { label: '入库单列表', path: '/inbound/list' },
      { label: '创建入库单', path: '/inbound/create' },
      { label: '入库统计', path: '/inbound/statistics' },
    ],
  },
  {
    moduleKey: 'outbound',
    label: '出库管理',
    icon: Upload,
    permission: 'Wms.Outbound',
    children: [
      { label: '出库单列表', path: '/outbound/list' },
      { label: '创建出库单', path: '/outbound/create' },
      { label: '出库统计', path: '/outbound/statistics' },
    ],
  },
  {
    moduleKey: 'task-center',
    label: '任务中心',
    icon: List,
    permission: 'Wms.TaskCenter',
    children: [
      { label: '任务列表', path: '/task-center/list' },
      { label: '任务监控', path: '/task-center/monitor' },
    ],
  },
  {
    moduleKey: 'transfer',
    label: '调拨管理',
    icon: Switch,
    permission: 'Wms.Transfer',
    children: [
      { label: '调拨单列表', path: '/transfer/list' },
      { label: '创建调拨单', path: '/transfer/create' },
      { label: '在途跟踪', path: '/transfer/tracking' },
    ],
  },
  {
    moduleKey: 'cycle-count',
    label: '盘点管理',
    icon: Finished,
    permission: 'Wms.CycleCount',
    children: [
      { label: '盘点计划', path: '/cycle-count/plans' },
      { label: '盘点执行', path: '/cycle-count/execute' },
      { label: '差异处理', path: '/cycle-count/difference' },
    ],
  },
  {
    moduleKey: 'line-side',
    label: '线边仓',
    icon: SetUp,
    permission: 'Wms.LineSide',
    children: [
      { label: '线边仓概览', path: '/line-side/overview' },
      { label: '看板页', path: '/line-side/kanban' },
      { label: '补料任务', path: '/line-side/replenishment' },
    ],
  },
  {
    moduleKey: 'production',
    label: '生产协同',
    icon: Promotion,
    permission: 'Wms.Production',
    children: [
      { label: '领料单', path: '/production/requisitions' },
      { label: '成品入库', path: '/production/finished-goods' },
      { label: '委外追踪', path: '/production/subcontract' },
    ],
  },
  {
    moduleKey: 'barcode-label',
    label: '条码标签',
    icon: Ticket,
    permission: 'Wms.BarcodeLabel',
    children: [
      { label: '条码规则', path: '/barcode-label/rules' },
      { label: '标签模板', path: '/barcode-label/templates' },
      { label: '打印任务', path: '/barcode-label/print-jobs' },
    ],
  },
  {
    moduleKey: 'workflow',
    label: '工作流',
    icon: Share,
    permission: 'Wms.Workflow',
    children: [
      { label: '审批流配置', path: '/workflow/definitions' },
      { label: '审批页面', path: '/workflow/approval' },
    ],
  },
  {
    moduleKey: 'rule-engine',
    label: '规则引擎',
    icon: Operation,
    permission: 'Wms.RuleEngine',
    children: [
      { label: '规则配置', path: '/rule-engine/rules' },
      { label: '规则测试', path: '/rule-engine/test' },
    ],
  },
  {
    moduleKey: 'notification',
    label: '通知管理',
    icon: Bell,
    permission: 'Wms.Notification',
    children: [
      { label: '通知列表', path: '/notification/logs' },
      { label: '通知配置', path: '/notification/config' },
    ],
  },
  {
    moduleKey: 'supplier',
    label: '供应商管理',
    icon: UserFilled,
    permission: 'Wms.Supplier',
    children: [
      { label: '供应商列表', path: '/supplier/list' },
    ],
  },
  {
    moduleKey: 'data-dictionary',
    label: '数据字典',
    icon: Notebook,
    permission: 'Wms.DataDictionary',
    children: [
      { label: '数据字典', path: '/data-dictionary/list' },
    ],
  },
];

// ── Stores ────────────────────────────────────────────────────────
const appStore = useAppStore();
const authStore = useAuthStore();
const route = useRoute();

const isCollapsed = computed(() => appStore.isCollapsed);

// ── Active menu computed ──────────────────────────────────────────
const activeMenu = computed(() => {
  const { path } = route;
  // Match deepest route first
  if (path === '/') return '/';

  // Find matching menu item
  for (const group of menuGroups) {
    for (const item of group.children) {
      if (path.startsWith(item.path)) return item.path;
    }
  }

  // Check system routes
  const systemPrefixes = [
    '/system/users',
    '/system/roles',
    '/system/permissions',
    '/system/organization',
    '/system/settings',
  ];
  for (const prefix of systemPrefixes) {
    if (path.startsWith(prefix)) return prefix;
  }

  return path;
});

// ── Permission-filtered menu groups ───────────────────────────────
const filteredMenuGroups = computed(() => {
  var perm = menuGroups
    .filter((group) => {
      return authStore.hasPermission(group.permission);
    })
    .map((group) => ({
      ...group,
      children: group.children.filter(
        (item) => !item.permission || authStore.hasPermission(item.permission)
      ),
    }))
    .filter((group) => group.children.length > 0);
    return perm;
});
</script>

<style scoped lang="scss">
.wms-sidebar {
  background-color: $wms-bg-sidebar;
  transition: width $wms-transition-expand;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  z-index: $wms-z-sidebar;
}

// ── Logo ──────────────────────────────────────────────────────────
.sidebar-logo {
  height: $wms-header-height;
  display: flex;
  align-items: center;
  padding: 0 $wms-spacing-md;
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
  gap: $wms-spacing-sm;
  overflow: hidden;
  flex-shrink: 0;

  .logo-icon {
    flex-shrink: 0;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  .logo-text {
    font-size: $wms-font-size-h2;
    font-weight: 700;
    color: #ffffff;
    white-space: nowrap;
    overflow: hidden;
    letter-spacing: 0.5px;
  }

  &.collapsed {
    justify-content: center;
    padding: 0;
  }
}

// ── Scroll container ──────────────────────────────────────────────
.sidebar-menu-scroll {
  flex: 1;
  overflow: hidden;
  :deep(.el-scrollbar__wrap) {
    overflow-x: hidden;
  }
}

// ── Menu ──────────────────────────────────────────────────────────
.wms-menu {
  border-right: none;
  padding-top: $wms-spacing-xs;

  // Override Element Plus menu item styles
  :deep(.el-menu-item),
  :deep(.el-sub-menu__title) {
    height: 46px;
    line-height: 46px;
    margin: 2px $wms-spacing-xs;
    border-radius: $wms-radius-md;
    padding-left: 20px !important;

    .el-icon {
      font-size: 18px;
      margin-right: 8px;
      width: 18px;
      text-align: center;
    }

    &:hover {
      background-color: rgba(255, 255, 255, 0.08) !important;
      color: #ffffff !important;
    }
  }

  :deep(.el-menu-item.is-active) {
    background-color: $wms-bg-sidebar-active !important;
    color: #ffffff !important;
    font-weight: 500;

    &::before {
      content: '';
      position: absolute;
      left: 0;
      top: 50%;
      transform: translateY(-50%);
      width: 3px;
      height: 20px;
      background-color: #ffffff;
      border-radius: 0 2px 2px 0;
    }
  }

  // Submenu children indentation
  :deep(.el-menu .el-menu-item) {
    padding-left: 54px !important;
  }

  // Collapsed mode adjustments
  &.el-menu--collapse {
    :deep(.el-menu-item),
    :deep(.el-sub-menu__title) {
      padding-left: 0 !important;
      justify-content: center;
      margin: 2px $wms-spacing-xs;

      .el-icon {
        margin-right: 0;
      }
    }
  }

  // Sub-menu popup
  :deep(.el-menu--popup) {
    background-color: $wms-bg-sidebar;
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-radius: $wms-radius-md;
    padding: 4px 0;

    .el-menu-item {
      height: 40px;
      line-height: 40px;
      color: rgba(255, 255, 255, 0.65);

      &:hover {
        background-color: rgba(255, 255, 255, 0.08) !important;
        color: #ffffff !important;
      }

      &.is-active {
        background-color: $wms-bg-sidebar-active !important;
        color: #ffffff !important;
      }
    }
  }
}

// ── Fade transition for logo text ─────────────────────────────────
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
