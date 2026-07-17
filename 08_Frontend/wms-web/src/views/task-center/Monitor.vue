<template>
  <div class="page-container">
    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>任务监控</span>
          <WmsSignalRIndicator :status="connected ? 'connected' : 'disconnected'" show-label />
        </div>
      </template>

      <div class="statistics-grid">
        <WmsStatisticsCard title="待处理" :value="monitor.pendingCount" :icon="Clock" color="#E6A23C" realtime />
        <WmsStatisticsCard title="进行中" :value="monitor.inProgressCount" :icon="Loading" color="#409EFF" realtime />
        <WmsStatisticsCard title="已完成" :value="monitor.completedCount" :icon="Check" color="#67C23A" realtime />
        <WmsStatisticsCard title="异常" :value="monitor.exceptionCount" :icon="Warning" color="#F56C6C" realtime />
      </div>

      <el-divider content-position="left">实时任务状态</el-divider>
      <div class="monitor-legend">
        <span class="legend-item"><span class="dot pending" />待处理</span>
        <span class="legend-item"><span class="dot in-progress" />进行中</span>
        <span class="legend-item"><span class="dot completed" />已完成</span>
        <span class="legend-item"><span class="dot exception" />异常</span>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Clock, Loading, Check, Warning } from '@element-plus/icons-vue';
import WmsStatisticsCard from '@/components/common/WmsStatisticsCard.vue';
import WmsSignalRIndicator from '@/components/common/WmsSignalRIndicator.vue';
import { useSignalR } from '@/utils/signalr';
import { getTaskMonitor } from '@/api/taskCenter';
import type { TaskMonitorDto } from '@/api/taskCenter';

const { connected, on } = useSignalR('/signalr/task');
const monitor = ref<TaskMonitorDto>({
  pendingCount: 0,
  inProgressCount: 0,
  completedCount: 0,
  exceptionCount: 0,
});

async function loadMonitor() {
  try {
    monitor.value = await getTaskMonitor();
  } catch {
    // keep defaults
  }
}

onMounted(() => {
  loadMonitor();
  on('TaskMonitorUpdate', (data: TaskMonitorDto) => {
    monitor.value = data;
  });
});
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.statistics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}
.monitor-legend {
  display: flex;
  gap: 24px;
  padding: 16px;
  background: #f5f7fa;
  border-radius: 4px;
}
.legend-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
}
.dot {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  &.pending { background: #E6A23C; }
  &.in-progress { background: #409EFF; }
  &.completed { background: #67C23A; }
  &.exception { background: #F56C6C; }
}
</style>
