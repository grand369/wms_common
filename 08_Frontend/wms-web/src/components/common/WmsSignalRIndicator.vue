<template>
  <div class="wms-signalr-indicator">
    <el-tooltip
      :content="tooltipContent"
      placement="bottom"
      :disabled="!showTooltip"
    >
      <div
        class="indicator-dot"
        :class="`status-${connectionStatus}`"
        @mouseenter="showTooltip = true"
        @mouseleave="showTooltip = false"
      >
        <span class="dot" />
        <span v-if="showLabel" class="label">{{ statusLabel }}</span>
      </div>
    </el-tooltip>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'

/**
 * COMP-015 WmsSignalRIndicator - SignalR 连接状态指示器
 *
 * 显示 SignalR 连接状态
 * 支持 tooltip 显示连接详情
 *
 * @props
 * - status: 连接状态（可从外部传入）
 * - showLabel: 是否显示状态文本
 * - pollInterval: 轮询间隔（毫秒）
 *
 * @emits
 * - status-change: 状态变化
 */

// 连接状态类型
export type SignalRStatus = 'connected' | 'reconnecting' | 'disconnected'

// Props 定义
const props = withDefaults(defineProps<{
  status?: SignalRStatus
  showLabel?: boolean
  pollInterval?: number
}>(), {
  status: undefined,
  showLabel: false,
  pollInterval: 5000
})

// Emits 定义
const emit = defineEmits<{
  'status-change': [status: SignalRStatus]
}>()

// 内部状态
const connectionStatus = ref<SignalRStatus>('disconnected')
const showTooltip = ref(false)
// 连接详情
const connectionDetails = ref({
  url: '',
  reconnectAttempts: 0,
  lastConnected: ''
})

// 状态标签
const statusLabel = computed(() => {
  const labels: Record<SignalRStatus, string> = {
    connected: '已连接',
    reconnecting: '重连中',
    disconnected: '未连接'
  }
  return labels[connectionStatus.value]
})

// Tooltip 内容
const tooltipContent = computed(() => {
  let content = `状态: ${statusLabel.value}`
  if (connectionDetails.value.url) {
    content += `\n地址: ${connectionDetails.value.url}`
  }
  if (connectionDetails.value.reconnectAttempts > 0) {
    content += `\n重连次数: ${connectionDetails.value.reconnectAttempts}`
  }
  if (connectionDetails.value.lastConnected) {
    content += `\n上次连接: ${connectionDetails.value.lastConnected}`
  }
  return content
})

// 检查连接状态（实际项目中应使用 SignalR 服务）
async function checkConnection() {
  try {
    // TODO: 使用实际的 SignalR 服务
    // const status = await signalRService.getStatus()
    // connectionStatus.value = status

    // 模拟状态
    connectionStatus.value = 'connected'
  } catch (error) {
    connectionStatus.value = 'disconnected'
  }
  emit('status-change', connectionStatus.value)
}

// 定时器
let pollTimer: number | null = null

// 启动轮询
function startPolling() {
  if (props.pollInterval > 0) {
    pollTimer = window.setInterval(checkConnection, props.pollInterval)
  }
}

// 停止轮询
function stopPolling() {
  if (pollTimer) {
    clearInterval(pollTimer)
    pollTimer = null
  }
}

// 生命周期
onMounted(() => {
  checkConnection()
  startPolling()
})

onUnmounted(() => {
  stopPolling()
})

// 监听外部状态
import { watch } from 'vue'
watch(() => props.status, (newVal) => {
  if (newVal) {
    connectionStatus.value = newVal
  }
})
</script>

<style scoped lang="scss">
.wms-signalr-indicator {
  display: inline-flex;
  align-items: center;
}

.indicator-dot {
  display: flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: background 0.3s;

  &:hover {
    background: #F5F7FA;
  }
}

.dot {
  width: 10px;
  height: 10px;
  border-radius: 50%;
  display: inline-block;
  transition: background 0.3s;

  .status-connected & {
    background: #67C23A;
    box-shadow: 0 0 8px rgba(103, 194, 58, 0.4);
  }

  .status-reconnecting & {
    background: #E6A23C;
    animation: blink 1s infinite;

    @keyframes blink {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.3; }
    }
  }

  .status-disconnected & {
    background: #F56C6C;
  }
}

.label {
  font-size: 13px;
  color: #606266;
}
</style>
