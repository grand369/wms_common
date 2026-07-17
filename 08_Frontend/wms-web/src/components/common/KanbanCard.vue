<template>
  <div class="kanban-card" :class="{ 'is-low': isLow, 'is-critical': isCritical }">
    <div class="card-header">
      <span class="material-code">{{ material.materialCode }}</span>
      <el-tag :type="statusType" size="small" effect="dark">
        {{ statusText }}
      </el-tag>
    </div>
    <div class="material-name" :title="material.materialName">{{ material.materialName }}</div>
    <div class="progress-section">
      <div class="progress-labels">
        <span class="progress-label">库存</span>
        <span class="progress-label-right">
          <span :class="{ 'low-text': isLow }">{{ material.currentQty }}</span>
          <span class="separator">/</span>
          <span>{{ material.requiredQty }}</span>
        </span>
      </div>
      <el-progress
        :percentage="percentage"
        :stroke-width="10"
        :color="progressColor"
      />
    </div>
    <div class="card-footer">
      <el-button
        v-if="isLow"
        type="danger"
        size="small"
        :plain="!isCritical"
        @click="handleReplenish"
      >
        触发补料
      </el-button>
      <span v-else class="status-ok">库存充足</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import type { KanbanMaterialDto } from '@/api/lineSide';

const props = defineProps<{
  material: KanbanMaterialDto;
}>();

const emit = defineEmits<{
  (e: 'replenish', material: KanbanMaterialDto): void;
}>();

const percentage = computed(() => {
  if (props.material.requiredQty <= 0) return 100;
  const pct = (props.material.currentQty / props.material.requiredQty) * 100;
  return Math.min(Math.round(pct), 100);
});

const isLow = computed(() => percentage.value < 100);
const isCritical = computed(() => percentage.value < 50);

const statusText = computed(() => {
  if (isCritical.value) return '紧急';
  if (isLow.value) return '不足';
  return '正常';
});

const statusType = computed(() => {
  if (isCritical.value) return 'danger';
  if (isLow.value) return 'warning';
  return 'success';
});

const progressColor = computed(() => {
  if (isCritical.value) return '#DC2626';
  if (percentage.value < 80) return '#D97706';
  if (percentage.value < 100) return '#2563EB';
  return '#16A34A';
});

function handleReplenish() {
  emit('replenish', props.material);
}
</script>

<style scoped lang="scss">
@use '@/styles/variables.scss' as *;

.kanban-card {
  background: $wms-bg-content;
  border-radius: $wms-radius-lg;
  box-shadow: $wms-shadow-sm;
  padding: $wms-spacing-md;
  transition: all 0.3s ease;
  border-left: 4px solid $wms-color-success;

  &.is-low {
    border-left-color: $wms-color-warning;
  }

  &.is-critical {
    border-left-color: #DC2626;
    animation: pulse-border 1.5s ease-in-out infinite;
  }
}

@keyframes pulse-border {
  0%, 100% { box-shadow: $wms-shadow-sm; }
  50% { box-shadow: 0 0 12px rgba(220, 38, 38, 0.35); }
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: $wms-spacing-xs;
}

.material-code {
  font-weight: 600;
  font-size: $wms-font-size-body;
  color: $wms-text-primary;
  font-family: var(--wms-font-family-number);
}

.material-name {
  font-size: $wms-font-size-small;
  color: $wms-text-secondary;
  margin-bottom: $wms-spacing-sm;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.progress-section {
  margin-bottom: $wms-spacing-sm;
}

.progress-labels {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 4px;
  font-size: $wms-font-size-small;
  color: $wms-text-secondary;
}

.progress-label-right {
  font-family: var(--wms-font-family-number);
}

.low-text {
  color: #DC2626;
  font-weight: 600;
}

.separator {
  margin: 0 2px;
}

.card-footer {
  text-align: right;
  min-height: 32px;
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.status-ok {
  font-size: $wms-font-size-small;
  color: $wms-color-success;
}
</style>
