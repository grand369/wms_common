<template>
  <el-tag
    :type="tagType"
    :effect="tagEffect"
    :size="size"
    class="wms-status-tag"
  >
    {{ statusLabel }}
  </el-tag>
</template>

<script setup lang="ts">
import { computed } from 'vue'

/**
 * COMP-005 WmsStatusTag - 业务状态标签组件
 *
 * 自动映射业务状态到 Element Plus tag 类型和颜色
 * 支持库存状态和单据状态两种类型
 *
 * @props
 * - status: 状态枚举值
 * - type: 状态类型，'inventory' | 'document'
 * - size: 标签大小，默认 'default'
 */

// 库存状态枚举
export type InventoryStatus = 'Available' | 'Frozen' | 'PendingInspection' | 'Quarantined' | 'InTransit' | 'Outsourced'

// 单据状态枚举
export type DocumentStatus = 'Draft' | 'Confirmed' | 'InProgress' | 'Completed' | 'Cancelled'

// Props 定义
const props = withDefaults(defineProps<{
  status: string
  type: 'inventory' | 'document'
  size?: 'large' | 'default' | 'small'
}>(), {
  size: 'default'
})

// 库存状态配置映射
const inventoryStatusMap: Record<InventoryStatus, { label: string; type: 'success' | 'warning' | 'info' | 'danger' }> = {
  Available: { label: '可用', type: 'success' },
  Frozen: { label: '冻结', type: 'danger' },
  PendingInspection: { label: '待检', type: 'warning' },
  Quarantined: { label: '隔离', type: 'danger' },
  InTransit: { label: '在途', type: 'info' },
  Outsourced: { label: '外协', type: 'info' }
}

// 单据状态配置映射
const documentStatusMap: Record<DocumentStatus, { label: string; type: 'success' | 'warning' | 'info' | 'danger' }> = {
  Draft: { label: '草稿', type: 'info' },
  Confirmed: { label: '已确认', type: 'info' },
  InProgress: { label: '进行中', type: 'warning' },
  Completed: { label: '已完成', type: 'success' },
  Cancelled: { label: '已取消', type: 'info' }
}

// 标签类型计算
const tagType = computed(() => {
  if (props.type === 'inventory') {
    return inventoryStatusMap[props.status as InventoryStatus]?.type || 'info'
  } else {
    return documentStatusMap[props.status as DocumentStatus]?.type || 'info'
  }
})

// 标签效果
const tagEffect = computed(() => {
  return tagType.value === 'info' ? 'dark' : 'light'
})

// 状态标签文本
const statusLabel = computed(() => {
  if (props.type === 'inventory') {
    return inventoryStatusMap[props.status as InventoryStatus]?.label || props.status
  } else {
    return documentStatusMap[props.status as DocumentStatus]?.label || props.status
  }
})
</script>

<style scoped lang="scss">
.wms-status-tag {
  font-weight: 500;
}
</style>
