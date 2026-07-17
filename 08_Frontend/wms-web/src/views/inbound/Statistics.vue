<template>
  <div class="page-container">
    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>入库统计</span>
          <div class="header-actions">
            <el-date-picker v-model="dateRange" type="daterange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" format="YYYY-MM-DD" value-format="YYYY-MM-DD" @change="loadStatistics" />
          </div>
        </div>
      </template>

      <div class="statistics-grid">
        <WmsStatisticsCard title="入库单总数" :value="stats.totalCount" :icon="Document" color="#409EFF" />
        <WmsStatisticsCard title="待处理" :value="stats.pendingCount" :icon="Clock" color="#E6A23C" />
        <WmsStatisticsCard title="已完成" :value="stats.completedCount" :icon="Check" color="#67C23A" />
        <WmsStatisticsCard title="今日入库" :value="stats.todayCount" :icon="TrendCharts" color="#909399" />
      </div>

      <el-divider content-position="left">趋势图</el-divider>
      <div ref="chartRef" class="chart-container" />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue';
import { Document, Clock, Check, TrendCharts } from '@element-plus/icons-vue';
import WmsStatisticsCard from '@/components/common/WmsStatisticsCard.vue';
import { getInboundStatistics } from '@/api/inbound';
import type { InboundStatisticsDto } from '@/api/inbound';

const stats = ref<InboundStatisticsDto>({
  totalCount: 0,
  pendingCount: 0,
  completedCount: 0,
  todayCount: 0,
});
const dateRange = ref<string[]>([]);
const chartRef = ref<HTMLDivElement>();

async function loadStatistics() {
  const params: { startDate?: string; endDate?: string } = {};
  if (dateRange.value && dateRange.value.length === 2) {
    params.startDate = dateRange.value[0];
    params.endDate = dateRange.value[1];
  }
  try {
    stats.value = await getInboundStatistics(params);
  } catch {
    // keep defaults
  }
  renderChart();
}

function renderChart() {
  nextTick(() => {
    if (!chartRef.value) return;
    // 简单使用 CSS 柱状图避免 echarts 依赖问题
    chartRef.value.innerHTML = `
      <div class="simple-chart">
        <div class="bar" style="height: ${Math.min(stats.value.totalCount * 2 + 20, 200)}px">
          <span class="bar-label">总数</span>
          <span class="bar-value">${stats.value.totalCount}</span>
        </div>
        <div class="bar" style="height: ${Math.min(stats.value.pendingCount * 4 + 20, 200)}px">
          <span class="bar-label">待处理</span>
          <span class="bar-value">${stats.value.pendingCount}</span>
        </div>
        <div class="bar" style="height: ${Math.min(stats.value.completedCount * 3 + 20, 200)}px">
          <span class="bar-label">已完成</span>
          <span class="bar-value">${stats.value.completedCount}</span>
        </div>
        <div class="bar" style="height: ${Math.min(stats.value.todayCount * 5 + 20, 200)}px">
          <span class="bar-label">今日</span>
          <span class="bar-value">${stats.value.todayCount}</span>
        </div>
      </div>
    `;
  });
}

onMounted(() => {
  loadStatistics();
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
.header-actions {
  display: flex;
  gap: 8px;
}
.statistics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}
.chart-container {
  min-height: 250px;
  :deep(.simple-chart) {
    display: flex;
    align-items: flex-end;
    justify-content: center;
    gap: 32px;
    height: 220px;
    padding: 16px;
    background: #f5f7fa;
    border-radius: 4px;
  }
  :deep(.bar) {
    width: 80px;
    background: #409eff;
    border-radius: 4px 4px 0 0;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
    align-items: center;
    color: white;
    padding-bottom: 8px;
    transition: all 0.3s;
  }
  :deep(.bar-label) {
    font-size: 12px;
    margin-bottom: 4px;
  }
  :deep(.bar-value) {
    font-weight: 700;
  }
}
</style>
