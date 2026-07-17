<template>
  <div class="wms-approval-flow">
    <div class="flow-container">
      <div
        v-for="(node, index) in nodes"
        :key="node.id"
        class="flow-node"
        :class="{
          'is-active': index === currentNodeIndex,
          'is-completed': index < currentNodeIndex,
          'is-rejected': node.status === 'Rejected'
        }"
      >
        <!-- 节点图标 -->
        <div class="node-icon">
          <el-icon v-if="node.status === 'Approved'" :size="20" color="#67C23A">
            <Check />
          </el-icon>
          <el-icon v-else-if="node.status === 'Rejected'" :size="20" color="#F56C6C">
            <Close />
          </el-icon>
          <el-icon v-else-if="index === currentNodeIndex" :size="20" color="#409EFF">
            <Loading />
          </el-icon>
          <span v-else class="node-index">{{ index + 1 }}</span>
        </div>

        <!-- 节点内容 -->
        <div class="node-content">
          <div class="node-title">{{ node.title }}</div>
          <div class="node-approver">
            审批人: {{ node.approver || '待分配' }}
          </div>

          <!-- 审批状态 -->
          <div class="node-status">
            <wms-status-tag
              :status="node.status"
              type="document"
              size="small"
            />
          </div>

          <!-- 审批时间 -->
          <div v-if="node.timestamp" class="node-time">
            {{ node.timestamp }}
          </div>

          <!-- 审批意见 -->
          <div v-if="node.comment" class="node-comment">
            <el-icon><ChatDotRound /></el-icon>
            {{ node.comment }}
          </div>
        </div>

        <!-- 连接线 -->
        <div v-if="index < nodes.length - 1" class="flow-connector">
          <el-icon><ArrowDown /></el-icon>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Check, Close, Loading, ArrowDown, ChatDotRound } from '@element-plus/icons-vue'
import WmsStatusTag from './WmsStatusTag.vue'

/**
 * COMP-016 WmsApprovalFlow - 审批流程可视化组件
 *
 * 展示审批流程的各个节点状态
 * 集成 WmsStatusTag 显示节点状态
 *
 * @props
 * - nodes: 审批节点数组
 * - currentNode: 当前节点索引或ID
 * - status: 整体状态
 */

// 审批节点接口
export interface WmsApprovalNode {
  id: string
  title: string
  approver?: string
  status: 'Pending' | 'InProgress' | 'Approved' | 'Rejected' | 'Skipped'
  timestamp?: string
  comment?: string
}

// Props 定义
const props = withDefaults(defineProps<{
  nodes: WmsApprovalNode[]
  currentNode?: string | number
  status?: string
}>(), {
  currentNode: 0,
  status: ''
})

// 当前节点索引
const currentNodeIndex = computed(() => {
  if (typeof props.currentNode === 'number') {
    return props.currentNode
  }
  return props.nodes.findIndex(node => node.id === props.currentNode)
})
</script>

<style scoped lang="scss">
.wms-approval-flow {
  padding: 24px;
}

.flow-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0;
}

.flow-node {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 240px;
  position: relative;

  &.is-active {
    .node-icon {
      border-color: #409EFF;
      background: #ECF5FF;
      animation: pulse 2s infinite;
    }
  }

  &.is-completed {
    .node-icon {
      border-color: #67C23A;
      background: #F0F9EB;
    }
  }

  &.is-rejected {
    .node-icon {
      border-color: #F56C6C;
      background: #FEF0F0;
    }
  }
}

.node-icon {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  border: 2px solid #DCDFE6;
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1;
  transition: all 0.3s;

  .node-index {
    font-weight: 600;
    color: #909399;
  }
}

.node-content {
  margin-top: 12px;
  text-align: center;
  padding: 12px;
  background: #FAFAFA;
  border-radius: 8px;
  width: 100%;
}

.node-title {
  font-weight: 600;
  font-size: 14px;
  color: #303133;
  margin-bottom: 8px;
}

.node-approver {
  font-size: 12px;
  color: #606266;
  margin-bottom: 8px;
}

.node-status {
  margin-bottom: 8px;
}

.node-time {
  font-size: 12px;
  color: #909399;
  margin-bottom: 4px;
}

.node-comment {
  font-size: 12px;
  color: #606266;
  background: white;
  padding: 8px;
  border-radius: 4px;
  margin-top: 8px;
  text-align: left;
  display: flex;
  align-items: flex-start;
  gap: 4px;

  .el-icon {
    margin-top: 2px;
    flex-shrink: 0;
  }
}

.flow-connector {
  height: 32px;
  display: flex;
  align-items: center;
  color: #DCDFE6;
  font-size: 20px;
}

@keyframes pulse {
  0%, 100% { box-shadow: 0 0 0 0 rgba(64, 158, 255, 0.4); }
  50% { box-shadow: 0 0 0 8px rgba(64, 158, 255, 0); }
}
</style>
