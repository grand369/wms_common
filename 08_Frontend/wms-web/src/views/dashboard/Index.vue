<template>
  <div class="dashboard-index">
    <!-- Statistics Cards -->
    <div class="stats-row">
      <el-row :gutter="16">
        <el-col :xs="24" :sm="12" :lg="6" v-for="card in statCards" :key="card.key">
          <div class="stat-card" :class="card.colorClass">
            <div class="stat-card-left">
              <div class="stat-card-label">{{ card.label }}</div>
              <div class="stat-card-value">
                <template v-if="card.key === 'inventoryValue'">
                  ¥{{ formatNumberLarge(stats.inventoryValue) }}
                </template>
                <template v-else>
                  {{ formatNumberLarge(stats[card.key as keyof typeof stats] as number) }}
                </template>
              </div>
              <div class="stat-card-desc" v-if="card.desc">
                {{ card.desc }}
              </div>
            </div>
            <div class="stat-card-icon">
              <el-icon :size="28"><component :is="card.icon" /></el-icon>
            </div>
          </div>
        </el-col>
      </el-row>
    </div>

    <!-- Charts Row -->
    <el-row :gutter="16" class="charts-row">
      <el-col :xs="24" :lg="14">
        <div class="chart-card">
          <div class="chart-header">
            <h3>出入库趋势</h3>
            <el-radio-group v-model="trendDays" size="small">
              <el-radio-button :value="7">近7天</el-radio-button>
              <el-radio-button :value="30">近30天</el-radio-button>
            </el-radio-group>
          </div>
          <div class="chart-body">
            <VChart v-if="hasTrendData" :option="trendChartOption" autoresize style="height: 340px" />
            <el-empty v-else description="暂无趋势数据" />
          </div>
        </div>
      </el-col>
      <el-col :xs="24" :lg="10">
        <div class="chart-card">
          <div class="chart-header">
            <h3>库存分布</h3>
          </div>
          <div class="chart-body">
            <VChart v-if="hasDistributionData" :option="distributionChartOption" autoresize style="height: 340px" />
            <el-empty v-else description="暂无库存分布数据" />
          </div>
        </div>
      </el-col>
    </el-row>

    <!-- Bottom Row: Tasks & Alerts -->
    <el-row :gutter="16" class="bottom-row">
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header">
            <h3>任务执行率</h3>
          </div>
          <div class="task-rate-list" v-loading="loading">
            <div
              v-for="item in taskRates"
              :key="item.name"
              class="task-rate-item"
            >
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
            <el-empty v-if="!loading && taskRates.length === 0" description="暂无任务数据" />
          </div>
        </div>
      </el-col>
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header">
            <h3>预警信息</h3>
            <el-tag
              v-if="alerts.length > 0"
              :type="alerts.filter(a => a.level === 'danger').length > 0 ? 'danger' : 'warning'"
              size="small"
            >
              {{ alerts.length }} 条
            </el-tag>
          </div>
          <div class="alert-list" v-loading="loading">
            <div
              v-for="alert in alerts"
              :key="alert.id"
              class="alert-item"
            >
              <el-tag
                :type="alert.level === 'danger' ? 'danger' : 'warning'"
                size="small"
                effect="dark"
                class="alert-level"
              >
                {{ alert.level === 'danger' ? '严重' : '提示' }}
              </el-tag>
              <span class="alert-message" :title="alert.message">{{ alert.message }}</span>
              <span class="alert-time">{{ formatTime(alert.timestamp) }}</span>
            </div>
            <el-empty v-if="!loading && alerts.length === 0" description="暂无预警信息" />
          </div>
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import {
  Box,
  Download,
  Upload,
  List,
  TrendCharts,
} from '@element-plus/icons-vue';
import VChart from 'vue-echarts';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { LineChart, BarChart, PieChart } from 'echarts/charts';
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
} from 'echarts/components';
import {
  getDashboardStats,
  getInboundTrend,
  getOutboundTrend,
  getInventoryDistribution,
  getTaskExecutionRate,
  getDashboardAlerts,
  type DashboardStats,
  type InboundTrend,
  type OutboundTrend,
  type InventoryDistribution,
  type TaskExecutionRate,
  type DashboardAlert,
} from '@/api/dashboard';
import { useDashboardStore } from '@/stores/dashboard';

use([
  CanvasRenderer,
  LineChart,
  BarChart,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
]);

const dashboardStore = useDashboardStore();

// ── Stat Card Config ──────────────────────────────────────────
const statCards = [
  {
    key: 'inventoryValue',
    label: '库存总值',
    icon: Box,
    colorClass: 'card-primary',
    desc: '实时库存估值',
  },
  {
    key: 'todayInbound',
    label: '今日入库',
    icon: Download,
    colorClass: 'card-success',
    desc: '今日入库数量(件)',
  },
  {
    key: 'todayOutbound',
    label: '今日出库',
    icon: Upload,
    colorClass: 'card-warning',
    desc: '今日出库数量(件)',
  },
  {
    key: 'pendingTasks',
    label: '待处理任务',
    icon: List,
    colorClass: 'card-info',
    desc: '当前待完成任务',
  },
];

// ── State ─────────────────────────────────────────────────────
const loading = ref(true);
const trendDays = ref(7);

const stats = reactive<DashboardStats>({
  inventoryValue: 0,
  todayInbound: 0,
  todayOutbound: 0,
  pendingTasks: 0,
  alertCount: 0,
});

const inboundTrend = ref<InboundTrend[]>([]);
const outboundTrend = ref<OutboundTrend[]>([]);
const distribution = ref<InventoryDistribution[]>([]);
const taskRates = ref<TaskExecutionRate[]>([]);
const alerts = ref<DashboardAlert[]>([]);

// ── Computed ──────────────────────────────────────────────────
const hasTrendData = computed(() => inboundTrend.value.length > 0 || outboundTrend.value.length > 0);

const hasDistributionData = computed(() => distribution.value.length > 0);

// ── Formatting Helpers ────────────────────────────────────────
function formatNumberLarge(n: number): string {
  if (n === undefined || n === null) return '--';
  if (n >= 10000) return (n / 10000).toFixed(1) + '万';
  return n.toLocaleString();
}

function formatTime(timestamp: string): string {
  if (!timestamp) return '';
  const d = new Date(timestamp);
  return d.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' });
}

function getProgressColor(rate: number): string {
  if (rate >= 90) return '#16A34A';
  if (rate >= 70) return '#2563EB';
  if (rate >= 50) return '#D97706';
  return '#DC2626';
}

// ── Chart Options ─────────────────────────────────────────────
const trendChartOption = computed(() => {
  const inData = inboundTrend.value.slice(0, trendDays.value);
  const outData = outboundTrend.value.slice(0, trendDays.value);
  return {
    tooltip: { trigger: 'axis' },
    legend: {
      data: ['入库', '出库'],
      bottom: 0,
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '12%',
      top: '8%',
      containLabel: true,
    },
    xAxis: {
      type: 'category',
      data: inData.map((d) => d.date.slice(5)),
      boundaryGap: false,
    },
    yAxis: { type: 'value' },
    series: [
      {
        name: '入库',
        type: 'line',
        data: inData.map((d) => d.quantity),
        smooth: true,
        lineStyle: { color: '#2563EB', width: 2 },
        itemStyle: { color: '#2563EB' },
        areaStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 0, y2: 1,
            colorStops: [
              { offset: 0, color: 'rgba(37,99,235,0.25)' },
              { offset: 1, color: 'rgba(37,99,235,0.02)' },
            ],
          },
        },
      },
      {
        name: '出库',
        type: 'line',
        data: outData.map((d) => d.quantity),
        smooth: true,
        lineStyle: { color: '#D97706', width: 2 },
        itemStyle: { color: '#D97706' },
        areaStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 0, y2: 1,
            colorStops: [
              { offset: 0, color: 'rgba(217,119,6,0.25)' },
              { offset: 1, color: 'rgba(217,119,6,0.02)' },
            ],
          },
        },
      },
    ],
  };
});

const distributionChartOption = computed(() => ({
  tooltip: { trigger: 'item', formatter: '{b}: {c} ({d}%)' },
  legend: {
    orient: 'vertical',
    left: 'left',
    top: 'center',
  },
  series: [
    {
      type: 'pie',
      radius: ['45%', '75%'],
      center: ['55%', '50%'],
      avoidLabelOverlap: false,
      label: { show: false },
      emphasis: {
        label: { show: true, fontSize: 14, fontWeight: 'bold' },
      },
      data: distribution.value.map((d) => ({
        name: d.category,
        value: d.value,
      })),
    },
  ],
  color: ['#2563EB', '#D97706', '#16A34A', '#0EA5E9', '#DC2626', '#475569'],
}));

// ── Data Loading ──────────────────────────────────────────────
async function loadDashboard() {
  // Try cache first
  const cachedStats = dashboardStore.getCached<DashboardStats>('dashboard-stats');
  const cachedInbound = dashboardStore.getCached<InboundTrend[]>('dashboard-inbound');
  const cachedOutbound = dashboardStore.getCached<OutboundTrend[]>('dashboard-outbound');
  const cachedDist = dashboardStore.getCached<InventoryDistribution[]>('dashboard-dist');
  const cachedTasks = dashboardStore.getCached<TaskExecutionRate[]>('dashboard-tasks');
  const cachedAlerts = dashboardStore.getCached<DashboardAlert[]>('dashboard-alerts');

  if (cachedStats && cachedInbound && cachedOutbound && cachedDist && cachedTasks && cachedAlerts) {
    Object.assign(stats, cachedStats);
    inboundTrend.value = cachedInbound;
    outboundTrend.value = cachedOutbound;
    distribution.value = cachedDist;
    taskRates.value = cachedTasks;
    alerts.value = cachedAlerts;
    loading.value = false;
    return;
  }

  loading.value = true;
  try {
    const [statsResult, inbound, outbound, dist, tasks, alertData] = await Promise.allSettled([
      getDashboardStats(),
      getInboundTrend(),
      getOutboundTrend(),
      getInventoryDistribution(),
      getTaskExecutionRate(),
      getDashboardAlerts(),
    ]);

    if (statsResult.status === 'fulfilled') {
      Object.assign(stats, statsResult.value);
      dashboardStore.setCache('dashboard-stats', statsResult.value);
    }
    if (inbound.status === 'fulfilled') {
      inboundTrend.value = inbound.value;
      dashboardStore.setCache('dashboard-inbound', inbound.value);
    }
    if (outbound.status === 'fulfilled') {
      outboundTrend.value = outbound.value;
      dashboardStore.setCache('dashboard-outbound', outbound.value);
    }
    if (dist.status === 'fulfilled') {
      distribution.value = dist.value;
      dashboardStore.setCache('dashboard-dist', dist.value);
    }
    if (tasks.status === 'fulfilled') {
      taskRates.value = tasks.value;
      dashboardStore.setCache('dashboard-tasks', tasks.value);
    }
    if (alertData.status === 'fulfilled') {
      alerts.value = alertData.value;
      dashboardStore.setCache('dashboard-alerts', alertData.value);
    }
  } catch {
    // Silently handle errors - show empty states
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  loadDashboard();
});
</script>

<style scoped lang="scss">
@use '@/styles/variables.scss' as *;

.dashboard-index {
  padding: $wms-spacing-lg;
  max-width: 1400px;
  margin: 0 auto;
}

// ── Stats Cards ───────────────────────────────────────────────
.stats-row {
  margin-bottom: $wms-spacing-md;
}

.stat-card {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: $wms-spacing-lg;
  border-radius: $wms-radius-lg;
  background: $wms-bg-content;
  box-shadow: $wms-shadow-sm;
  margin-bottom: $wms-spacing-md;
  position: relative;
  overflow: hidden;
  min-height: 120px;

  &::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 4px;
    height: 100%;
    border-radius: 4px 0 0 4px;
  }

  &.card-primary::before { background: $wms-color-primary; }
  &.card-success::before { background: $wms-color-success; }
  &.card-warning::before { background: $wms-color-warning; }
  &.card-info::before { background: $wms-color-info; }
}

.stat-card-left {
  flex: 1;
}

.stat-card-label {
  font-size: $wms-font-size-small;
  color: $wms-text-secondary;
  margin-bottom: $wms-spacing-xs;
}

.stat-card-value {
  font-size: 28px;
  font-weight: 700;
  color: $wms-text-primary;
  margin-bottom: $wms-spacing-xs;
  line-height: 1.2;
}

.stat-card-desc {
  font-size: $wms-font-size-small;
  color: $wms-text-secondary;
}

.stat-card-icon {
  width: 56px;
  height: 56px;
  border-radius: $wms-radius-md;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.card-primary .stat-card-icon {
  background: $wms-color-primary-light-9;
  .el-icon { color: $wms-color-primary; }
}
.card-success .stat-card-icon {
  background: rgba(22, 163, 74, 0.1);
  .el-icon { color: $wms-color-success; }
}
.card-warning .stat-card-icon {
  background: rgba(217, 119, 6, 0.1);
  .el-icon { color: $wms-color-warning; }
}
.card-info .stat-card-icon {
  background: rgba(14, 165, 233, 0.1);
  .el-icon { color: $wms-color-info; }
}

// ── Chart Cards ───────────────────────────────────────────────
.charts-row {
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
  min-height: 340px;
}

// ── Bottom Row ────────────────────────────────────────────────
.bottom-row {
  margin-bottom: $wms-spacing-md;
}

// ── Task Rate List ────────────────────────────────────────────
.task-rate-list {
  padding: $wms-spacing-md $wms-spacing-lg;
  min-height: 280px;
}

.task-rate-item {
  margin-bottom: $wms-spacing-md;

  &:last-child { margin-bottom: 0; }
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
    font-family: var(--wms-font-family-number);
  }
}

// ── Alert List ────────────────────────────────────────────────
.alert-list {
  padding: $wms-spacing-sm $wms-spacing-lg;
  min-height: 280px;
}

.alert-item {
  display: flex;
  align-items: center;
  gap: $wms-spacing-sm;
  padding: $wms-spacing-sm 0;
  border-bottom: 1px solid $wms-border-base;

  &:last-child { border-bottom: none; }
}

.alert-level {
  flex-shrink: 0;
}

.alert-message {
  flex: 1;
  font-size: $wms-font-size-body;
  color: $wms-text-regular;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.alert-time {
  font-size: $wms-font-size-small;
  color: $wms-text-secondary;
  flex-shrink: 0;
}

// ── Responsive ────────────────────────────────────────────────
@media (max-width: 768px) {
  .dashboard-index {
    padding: $wms-spacing-md;
  }

  .stat-card-value {
    font-size: 24px;
  }
}
</style>
