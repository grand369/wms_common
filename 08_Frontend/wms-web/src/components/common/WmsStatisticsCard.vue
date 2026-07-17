<template>
  <div class="wms-statistics-card" :class="{ 'is-loading': loading }">
    <!-- 图标 -->
    <div class="stat-icon" :style="{ color: color || '#409EFF' }">
      <el-icon :size="32">
        <component :is="icon" />
      </el-icon>
    </div>

    <!-- 内容 -->
    <div class="stat-content">
      <div class="stat-title">{{ title }}</div>
      <div class="stat-value" :style="{ color: color || '#303133' }">
        {{ displayValue }}
        <span v-if="unit" class="stat-unit">{{ unit }}</span>
      </div>

      <!-- 趋势 -->
      <div v-if="trend" class="stat-trend" :class="`trend-${trend}`">
        <el-icon v-if="trend === 'up'" :size="12"><ArrowUp /></el-icon>
        <el-icon v-else-if="trend === 'down'" :size="12"><ArrowDown /></el-icon>
        <el-icon v-else :size="12"><Minus /></el-icon>
        <span v-if="trendValue">{{ trendValue }}</span>
      </div>
    </div>

    <!-- 实时更新指示器 -->
    <div v-if="realtime" class="realtime-indicator">
      <span class="pulse-dot" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { ArrowUp, ArrowDown, Minus } from '@element-plus/icons-vue'

/**
 * COMP-014 WmsStatisticsCard - 统计卡片组件
 *
 * 用于仪表盘的统计卡片展示
 * 支持 SignalR 实时更新
 *
 * @props
 * - title: 卡片标题
 * - value: 数值
 * - unit: 单位
 * - icon: 图标组件
 * - trend: 趋势方向
 * - trendValue: 趋势值
 * - color: 主色调
 * - realtime: 是否启用实时更新
 * - signalRHub: SignalR Hub 名称
 * - signalRMethod: SignalR 方法名
 */

// Props 定义
const props = withDefaults(defineProps<{
  title: string
  value: number | string
  unit?: string
  icon?: any
  trend?: 'up' | 'down' | 'flat'
  trendValue?: string
  color?: string
  realtime?: boolean
  signalRHub?: string
  signalRMethod?: string
}>(), {
  unit: '',
  trend: undefined,
  trendValue: '',
  color: '',
  realtime: false,
  signalRHub: '',
  signalRMethod: ''
})

// 加载状态
const loading = ref(false)
// 显示值
const displayValue = computed(() => {
  if (typeof props.value === 'number') {
    return props.value.toLocaleString()
  }
  return props.value
})

// SignalR 连接（实际项目中应使用 SignalR 服务）
let signalRConnection: any = null

// 初始化 SignalR
async function initSignalR() {
  if (!props.realtime || !props.signalRHub) return

  try {
    // TODO: 使用实际的 SignalR 服务
    // signalRConnection = await signalRService.connect(props.signalRHub)
    // signalRConnection.on(props.signalRMethod, (newValue: any) => {
    //   // 更新值
    // })
    console.log('SignalR initialized for', props.signalRHub)
  } catch (error) {
    console.error('SignalR connection failed', error)
  }
}

// 销毁 SignalR 连接
function destroySignalR() {
  if (signalRConnection) {
    // signalRConnection.stop()
    signalRConnection = null
  }
}

// 生命周期
onMounted(() => {
  if (props.realtime) {
    initSignalR()
  }
})

onUnmounted(() => {
  destroySignalR()
})
</script>

<style scoped lang="scss">
.wms-statistics-card {
  position: relative;
  padding: 20px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.06);
  display: flex;
  align-items: flex-start;
  gap: 16px;
  transition: all 0.3s;
  overflow: hidden;

  &:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
    transform: translateY(-2px);
  }

  &.is-loading {
    opacity: 0.7;
  }
}

.stat-icon {
  width: 56px;
  height: 56px;
  border-radius: 8px;
  background: #F5F7FA;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.stat-content {
  flex: 1;
}

.stat-title {
  font-size: 14px;
  color: #909399;
  margin-bottom: 8px;
}

.stat-value {
  font-size: 28px;
  font-weight: 700;
  line-height: 1.2;
  font-family: 'DIN', 'Roboto', sans-serif;

  .stat-unit {
    font-size: 14px;
    font-weight: 400;
    margin-left: 4px;
    color: #909399;
  }
}

.stat-trend {
  display: flex;
  align-items: center;
  gap: 4px;
  margin-top: 8px;
  font-size: 13px;
  font-weight: 500;

  &.trend-up {
    color: #67C23A;
  }

  &.trend-down {
    color: #F56C6C;
  }

  &.trend-flat {
    color: #909399;
  }
}

.realtime-indicator {
  position: absolute;
  top: 12px;
  right: 12px;

  .pulse-dot {
    width: 8px;
    height: 8px;
    border-radius: 50%;
    background: #67C23A;
    display: inline-block;
    animation: pulse 2s infinite;

    @keyframes pulse {
      0%, 100% { opacity: 1; }
      50% { opacity: 0.4; }
    }
  }
}
</style>
