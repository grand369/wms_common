<template>
  <el-container class="wms-layout">
    <!-- Sidebar -->
    <Sidebar />

    <!-- Right Section: Header + Main -->
    <el-container class="wms-layout-right">
      <!-- Header -->
      <Header />

      <!-- Main Content -->
      <el-main class="wms-main">
        <router-view v-slot="{ Component, route: currentRoute }">
          <transition name="page-fade" mode="out-in">
            <component :is="Component" :key="currentRoute.fullPath" />
          </transition>
        </router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import { HomeFilled } from '@element-plus/icons-vue';
import Sidebar from './Sidebar.vue';
import Header from './Header.vue';

const route = useRoute();

// ── Breadcrumb from route metadata ────────────────────────────────
interface BreadcrumbMeta {
  label: string;
  path?: string;
}

const breadcrumbItems = computed<BreadcrumbMeta[]>(() => {
  const meta = route.meta;
  const items: BreadcrumbMeta[] = [];

  // Use breadcrumb array from meta if provided
  if (Array.isArray(meta.breadcrumb)) {
    return meta.breadcrumb as BreadcrumbMeta[];
  }

  // Otherwise use title from meta
  const title = meta.title as string;
  if (title && route.path !== '/') {
    items.push({ label: title });
  }

  return items;
});

const hasBreadcrumb = computed(() => breadcrumbItems.value.length > 0);
</script>

<style scoped lang="scss">
@use "@/styles/mixins.scss" as *;

.wms-layout {
  height: 100vh;
  overflow: hidden;
}

.wms-layout-right {
  flex-direction: column;
  overflow: hidden;
}

// ── Breadcrumb Bar ────────────────────────────────────────────────
.wms-breadcrumb-bar {
  height: $wms-breadcrumb-height;
  display: flex;
  align-items: center;
  flex-shrink: 0;
  background-color: $wms-bg-content;
  border-bottom: 1px solid $wms-border-base;
  padding: 0 $wms-spacing-lg;

  .breadcrumb-content {
    :deep(.el-breadcrumb__item) {
      display: inline-flex;
      align-items: center;
      gap: 4px;
    }

    :deep(.el-breadcrumb__inner) {
      color: $wms-text-secondary;
      font-size: $wms-font-size-small;
      font-weight: 400;

      &:hover {
        color: $wms-color-primary;
      }
    }

    :deep(.el-breadcrumb__item:last-child .el-breadcrumb__inner) {
      color: $wms-text-primary;
      font-weight: 500;
    }
  }
}

// ── Main Content ──────────────────────────────────────────────────
.wms-main {
  flex: 1;
  background-color: $wms-bg-base;
  padding: $wms-spacing-lg;
  overflow-y: auto;

  @include custom-scrollbar;
}

// ── Page Transition ───────────────────────────────────────────────
.page-fade-enter-active,
.page-fade-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}

.page-fade-enter-from {
  opacity: 0;
  transform: translateY(4px);
}

.page-fade-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}
</style>
