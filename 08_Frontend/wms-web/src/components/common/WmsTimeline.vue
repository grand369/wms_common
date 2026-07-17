<template>
  <div class="wms-timeline">
    <el-timeline>
      <el-timeline-item
        v-for="(item, index) in props.items"
        :key="index"
        :timestamp="item.time"
        :type="getStatusType(item.status)"
        :color="getStatusColor(item.status)"
        placement="top"
      >
        <div class="timeline-content">
          <div class="timeline-header">
            <wms-status-tag
              :status="item.status"
              :type="props.statusType"
              size="small"
            />
            <span v-if="item.operator" class="timeline-operator">
              操作人: {{ item.operator }}
            </span>
          </div>
          <div class="timeline-description">
            {{ item.description }}
          </div>
        </div>
      </el-timeline-item>
    </el-timeline>

    <!-- 空状态 -->
    <el-empty v-if="!props.items || props.items.length === 0" description="暂无状态记录" />
  </div>
</template>

<script setup lang="ts">
/**
 * COMP-006 WmsTimeline - 状态时间线组件
 *
 * 用于详情页的状态历史展示
 * 基于 Element Plus el-timeline 封装
 * 集成 WmsStatusTag 显示状态标签
 *
 * @props
 * - items: 时间线数据数组
 * - statusType: 状态类型，传递给 WmsStatusTag
 */

import WmsStatusTag from './WmsStatusTag.vue'

// 时间线数据项接口
export interface WmsTimelineItem {
  time: string
  status: string
  description: string
  operator?: string
}

// Props 定义
const props = withDefaults(defineProps<{
  items: WmsTimelineItem[]
  statusType?: 'inventory' | 'document'
}>(), {
  statusType: 'document'
})

// 获取状态对应的时间线类型
function getStatusType(status: string) {
  const typeMap: Record<string, 'primary' | 'success' | 'warning' | 'danger' | 'info'> = {
    'Completed': 'success',
    'Confirmed': 'primary',
    'InProgress': 'warning',
    'Cancelled': 'info',
    'Draft': 'info',
    'Available': 'success',
    'Frozen': 'danger',
    'PendingInspection': 'warning'
  }
  return typeMap[status] || 'primary'
}

// 获取状态对应的颜色
function getStatusColor(status: string) {
  const colorMap: Record<string, string> = {
    'Completed': '#67C23A',
    'Confirmed': '#409EFF',
    'InProgress': '#E6A23C',
    'Cancelled': '#909399',
    'Draft': '#909399',
    'Available': '#67C23A',
    'Frozen': '#F56C6C',
    'PendingInspection': '#E6A23C'
  }
  return colorMap[status] || '#409EFF'
}
</script>

<style scoped lang="scss">
.wms-timeline {
  padding: 16px;

  .timeline-content {
    padding: 8px 0;
  }

  .timeline-header {
    display: flex;
    align-items: center;
    gap: 12px;
    margin-bottom: 8px;
  }

  .timeline-operator {
    font-size: 13px;
    color: #909399;
  }

  .timeline-description {
    font-size: 14px;
    color: #606266;
    line-height: 1.6;
  }
}
</style>
