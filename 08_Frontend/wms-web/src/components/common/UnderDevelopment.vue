<template>
  <div class="under-dev">
    <div class="under-dev-icon">
      <el-icon :size="64"><Tools /></el-icon>
    </div>
    <h2 class="under-dev-title">{{ title }}</h2>
    <p class="under-dev-desc">{{ description }}</p>
    <div v-if="features.length > 0" class="under-dev-features">
      <div v-for="feature in features" :key="feature" class="feature-tag">
        <el-icon :size="14"><Check /></el-icon>
        <span>{{ feature }}</span>
      </div>
    </div>
    <el-divider />
    <p class="under-dev-hint">该模块正在紧张开发中，预计近期上线，敬请期待。</p>
  </div>
</template>

<script setup lang="ts">
import { Check, Tools } from '@element-plus/icons-vue';

withDefaults(
  defineProps<{
    title: string;
    description?: string;
    features?: string[];
  }>(),
  {
    description: '该功能模块正在开发中',
    features: () => [],
  },
);
</script>

<style lang="scss" scoped>
@use '@/styles/variables.scss' as *;

.under-dev {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 40px;
  text-align: center;
  min-height: 400px;
}

.under-dev-icon {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  background: $wms-color-primary-light-9;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: $wms-spacing-lg;

  .el-icon {
    color: $wms-color-primary;
  }
}

.under-dev-title {
  font-size: $wms-font-size-h1;
  font-weight: 600;
  color: $wms-text-primary;
  margin-bottom: $wms-spacing-xs;
}

.under-dev-desc {
  font-size: $wms-font-size-body;
  color: $wms-text-secondary;
  margin-bottom: $wms-spacing-lg;
  max-width: 480px;
}

.under-dev-features {
  display: flex;
  flex-wrap: wrap;
  gap: $wms-spacing-sm;
  justify-content: center;
  max-width: 560px;
  margin-bottom: $wms-spacing-md;
}

.feature-tag {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 4px 12px;
  background: $wms-bg-base;
  border: 1px solid $wms-border-base;
  border-radius: $wms-radius-round;
  font-size: $wms-font-size-small;
  color: $wms-text-regular;

  .el-icon {
    color: $wms-color-success;
  }
}

.under-dev-hint {
  font-size: $wms-font-size-small;
  color: $wms-text-secondary;
}

:deep(.el-divider) {
  width: 200px;
  margin: $wms-spacing-md auto;
}
</style>
