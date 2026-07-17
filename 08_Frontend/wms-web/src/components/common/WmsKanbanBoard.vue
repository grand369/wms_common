<template>
  <div class="wms-kanban-board">
    <div class="kanban-grid">
      <div
      v-for="card in props.cards"
      :key="card.materialCode"
        class="kanban-card"
        :class="{ 'low-stock': isLowStock(card) }"
      >
        <!-- 物料信息 -->
        <div class="card-header">
          <div class="material-code">{{ card.materialCode }}</div>
          <div class="material-name">{{ card.materialName }}</div>
        </div>

        <!-- 库存进度 -->
        <div class="card-body">
          <div class="stock-info">
            <span>当前库存: <strong>{{ card.currentQty }}</strong> {{ card.unit }}</span>
          </div>

          <el-progress
            :percentage="getStockPercent(card)"
            :color="getProgressColor(card)"
            :stroke-width="12"
            :text-inside="true"
            :format="() => `${card.currentQty}/${card.maxQty}`"
          />

          <div class="stock-limits">
            <span class="min-marker">Min: {{ card.minQty }}</span>
            <span class="max-marker">Max: {{ card.maxQty }}</span>
          </div>
        </div>

        <!-- 操作按钮 -->
        <div class="card-footer">
          <el-button
            v-if="isLowStock(card)"
            type="danger"
            size="small"
            @click="onTriggerReplenishment(card)"
          >
            触发补货
          </el-button>
          <el-tag v-else :type="getStockTagType(card)" size="small">
            {{ getStockStatus(card) }}
          </el-tag>
        </div>
      </div>
    </div>

    <!-- 空状态 -->
    <el-empty
      v-if="!props.cards || props.cards.length === 0"
      description="暂无看板数据"
    />
  </div>
</template>

<script setup lang="ts">
import { ElMessage } from 'element-plus'

/**
 * COMP-013 WmsKanbanBoard - 线边仓看板组件
 */

// 看板卡片数据接口
export interface WmsKanbanCard {
  materialCode: string
  materialName: string
  currentQty: number
  minQty: number
  maxQty: number
  unit?: string
}

// Props 定义
const props = defineProps<{
  cards: WmsKanbanCard[]
}>()

// Emits 定义
const emit = defineEmits<{
  replenish: [card: WmsKanbanCard]
}>()

// 判断低库存
function isLowStock(card: WmsKanbanCard) {
  return card.currentQty <= card.minQty
}

// 获取库存百分比
function getStockPercent(card: WmsKanbanCard) {
  if (!card.maxQty) return 0
  return Math.min(Math.round((card.currentQty / card.maxQty) * 100), 100)
}

// 获取进度条颜色
function getProgressColor(card: WmsKanbanCard) {
  const percent = getStockPercent(card)
  if (percent <= 20) return '#F56C6C'
  if (percent <= 50) return '#E6A23C'
  return '#67C23A'
}

// 获取库存状态标签类型
function getStockTagType(card: WmsKanbanCard) {
  const percent = getStockPercent(card)
  if (percent <= 20) return 'danger'
  if (percent <= 50) return 'warning'
  return 'success'
}

// 获取库存状态文本
function getStockStatus(card: WmsKanbanCard) {
  const percent = getStockPercent(card)
  if (percent <= 20) return '库存不足'
  if (percent <= 50) return '库存偏低'
  if (percent <= 80) return '库存正常'
  return '库存充足'
}

// 触发补货
function onTriggerReplenishment(card: WmsKanbanCard) {
  emit('replenish', card)
  ElMessage.success(`已触发 ${card.materialName} 的补货申请`)
}
</script>

<style scoped lang="scss">
.wms-kanban-board {
  .kanban-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
    gap: 16px;
  }

  .kanban-card {
    padding: 16px;
    border: 1px solid #DCDFE6;
    border-radius: 8px;
    background: white;
    transition: all 0.3s;

    &:hover {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }

    &.low-stock {
      border-color: #F56C6C;
      animation: pulse 2s infinite;

      @keyframes pulse {
        0%, 100% { box-shadow: 0 0 0 0 rgba(245, 108, 108, 0.4); }
        50% { box-shadow: 0 0 0 8px rgba(245, 108, 108, 0); }
      }
    }
  }

  .card-header {
    margin-bottom: 12px;
    padding-bottom: 8px;
    border-bottom: 1px solid #EBEEF5;

    .material-code {
      font-weight: 600;
      font-size: 14px;
      color: #303133;
    }

    .material-name {
      font-size: 12px;
      color: #909399;
      margin-top: 4px;
    }
  }

  .card-body {
    margin-bottom: 12px;

    .stock-info {
      margin-bottom: 8px;
      font-size: 13px;
      color: #606266;
    }

    .stock-limits {
      display: flex;
      justify-content: space-between;
      margin-top: 8px;
      font-size: 12px;
      color: #909399;
    }
  }

  .card-footer {
    display: flex;
    justify-content: center;
  }
}
</style>
