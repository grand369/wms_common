<template>
  <div class="dashboard-task">
    <!-- KPI Cards -->
    <el-row :gutter="16" class="stats-row">
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="任务执行率"
          :value="kpiStats.executionRate"
          unit="%"
          :icon="DataAnalysis"
          color="#2563EB"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="异常率"
          :value="data.abnormalRate"
          unit="%"
          :icon="WarningFilled"
          color="#DC2626"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="总任务数"
          :value="kpiStats.totalTasks"
          unit="个"
          :icon="List"
          color="#16A34A"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="已完成数"
          :value="kpiStats.completedTasks"
          unit="个"
          :icon="CircleCheckFilled"
          color="#0EA5E9"
        />
      </el-col>
    </el-row>

    <!-- Execution Rate List + Efficiency Heatmap -->
    <el-row :gutter="16">
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header"><h3>任务执行率</h3></div>
          <div class="task-rate-list" v-loading="loading">
            <div v-for="item in data.executionRate" :key="item.name" class="task-rate-item">
              <div class="task-rate-info">
                <span class="task-rate-name">{{ item.name }}</span>
                <span class="task-rate-count">{{ item.completed }}/{{ item.total }}</span>
              </div>
              <el-progress
                :percentage="item.rate"
                :stroke-width="10"
                :color="getProgressColor(item.rate)"
              />
            </div>
            <el-empty v-if="!loading && data.executionRate.length === 0" description="暂无任务数据" />
          </div>
        </div>
      </el-col>
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header"><h3>效率热力图</h3></div>
          <div class="chart-body">
            <VChart :option="efficiencyHeatmapOption" autoresize style="height: 320px" />
          </div>
        </div>
      </el-col>
    </el-row>

    <!-- Personnel Load -->
    <div class="chart-card">
      <div class="chart-header"><h3>人员负载</h3></div>
      <div class="chart-body">
        <VChart :option="personnelLoadChartOption" autoresize style="height: 320px" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import {
  DataAnalysis,
  WarningFilled,
  List,
  CircleCheckFilled,
} from '@element-plus/icons-vue';
import VChart from 'vue-echarts';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { BarChart } from 'echarts/charts';
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
} from 'echarts/components';
import WmsStatisticsCard from '@/components/common/WmsStatisticsCard.vue';
import {
  getTaskDashboard,
  type TaskDashboardData,
} from '@/api/dashboard';
import { useDashboardStore } from '@/stores/dashboard';

use([
  CanvasRenderer,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
]);

const dashboardStore = useDashboardStore();

// ── State ─────────────────────────────────────────────────────
const loading = ref(true);

const data = reactive<TaskDashboardData>({
  executionRate: [],
  efficiencyHeatmap: [],
  personnelLoad: [],
  abnormalRate: 0,
});

// ── KPI Computed ──────────────────────────────────────────────
const kpiStats = computed(() => {
  const rates = data.executionRate;
  const avgRate = rates.length > 0
    ? Math.round(rates.reduce((sum, r) => sum + r.rate, 0) / rates.length)
    : 0;
  const totalTasks = rates.reduce((sum, r) => sum + r.total, 0);
  const completedTasks = rates.reduce((sum, r) => sum + r.completed, 0);
  return { executionRate: avgRate, totalTasks, completedTasks };
});

// ── Helpers ───────────────────────────────────────────────────
function getProgressColor(rate: number): string {
  if (rate >= 90) return '#16A34A';
  if (rate >= 70) return '#2563EB';
  if (rate >= 50) return '#D97706';
  return '#DC2626';
}

// ── Chart Options ─────────────────────────────────────────────
const efficiencyHeatmapOption = computed(() => ({
  tooltip: { trigger: 'axis', axisPointer: { type: 'shadow' } },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '8%',
    top: '8%',
    containLabel: true,
  },
  xAxis: {
    type: 'category',
    data: data.efficiencyHeatmap.map((d) => d.period),
  },
  yAxis: { type: 'value', name: '效率值' },
  series: [
    {
      type: 'bar',
      data: data.efficiencyHeatmap.map((d) => ({
        value: d.value,
        itemStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 0, y2: 1,
            colorStops: [
              {
                offset: 0,
                color: d.value > 90 ? '#16A34A' : d.value > 70 ? '#2563EB' : d.value > 50 ? '#D97706' : '#DC2626',
              },
              {
                offset: 1,
                color: d.value > 90 ? '#86EFAC' : d.value > 70 ? '#93BBFD' : d.value > 50 ? '#FDE68A' : '#FCA5A5',
              },
            ],
          },
        },
      })),
      barWidth: '50%',
    },
  ],
}));

const personnelLoadChartOption = computed(() => ({
  tooltip: { trigger: 'axis', axisPointer: { type: 'shadow' } },
  legend: {
    data: ['任务数', '完成率'],
    bottom: 0,
  },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '12%',
    top: '8%',
    containLabel: true,
  },
  xAxis: { type: 'value' },
  yAxis: {
    type: 'category',
    data: data.personnelLoad.map((d) => d.name),
  },
  series: [
    {
      name: '任务数',
      type: 'bar',
      data: data.personnelLoad.map((d) => ({
        value: d.taskCount,
        itemStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 1, y2: 0,
            colorStops: [
              { offset: 0, color: '#2563EB' },
              { offset: 1, color: '#93BBFD' },
            ],
          },
        },
      })),
      barWidth: '50%',
    },
    {
      name: '完成率',
      type: 'bar',
      data: data.personnelLoad.map((d) => ({
        value: d.rate,
        itemStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 1, y2: 0,
            colorStops: [
              { offset: 0, color: '#16A34A' },
              { offset: 1, color: '#86EFAC' },
            ],
          },
        },
      })),
      barWidth: '50%',
    },
  ],
}));

// ── Data Loading ──────────────────────────────────────────────
async function loadData() {
  loading.value = true;
  try {
    const cacheKey = 'task-dashboard';
    let result = dashboardStore.getCached<TaskDashboardData>(cacheKey);
    if (!result) {
      result = await getTaskDashboard();
      dashboardStore.setCache(cacheKey, result);
    }
    Object.assign(data, result);
  } catch {
    // Silently handle errors
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  loadData();
});
</script>

<style lang="scss" scoped>
@use '@/styles/variables.scss' as *;

.dashboard-task {
  padding: $wms-spacing-lg;
  max-width: 1400px;
  margin: 0 auto;
}

.stats-row {
  margin-bottom: $wms-spacing-md;
}

.chart-card {
  background: $wms-bg-content;
  border-radius: $wms-radius-lg;
  box-shadow: $wms-shadow-sm;
  margin-bottom: $wms-spacing-md;
  overflow: hidden;
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: $wms-spacing-md $wms-spacing-lg;
  border-bottom: 1px solid $wms-border-base;

  h3 {
    font-size: $wms-font-size-h2;
    font-weight: 600;
    color: $wms-text-primary;
  }
}

.chart-body {
  padding: $wms-spacing-md;
  min-height: 320px;
}

// ── Task Rate List ────────────────────────────────────────────
.task-rate-list {
  padding: $wms-spacing-md $wms-spacing-lg;
  min-height: 280px;
}

.task-rate-item {
  margin-bottom: $wms-spacing-md;

  &:last-child {
    margin-bottom: 0;
  }
}

.task-rate-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: $wms-spacing-xs;

  .task-rate-name {
    font-size: $wms-font-size-body;
    color: $wms-text-regular;
  }

  .task-rate-count {
    font-size: $wms-font-size-small;
    color: $wms-text-secondary;
  }
}

@media (max-width: 768px) {
  .dashboard-task {
    padding: $wms-spacing-md;
  }
}
</style>
