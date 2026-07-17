<template>
  <div class="dashboard-warehouse">
    <!-- KPI Cards -->
    <el-row :gutter="16" class="stats-row">
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="库位占用率"
          :value="data.occupancyRate"
          unit="%"
          :icon="Odometer"
          color="#2563EB"
          trend="up"
          trend-value="+3.2%"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="入库总数"
          :value="data.inboundCount"
          unit="件"
          :icon="Download"
          color="#16A34A"
          trend="up"
          trend-value="+12.1%"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="出库总数"
          :value="data.outboundCount"
          unit="件"
          :icon="Upload"
          color="#D97706"
          trend="up"
          trend-value="+8.5%"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="任务执行率"
          :value="data.taskRate"
          unit="%"
          :icon="List"
          color="#0EA5E9"
          trend="up"
          trend-value="+1.1%"
        />
      </el-col>
    </el-row>

    <!-- Inbound/Outbound Trend Chart -->
    <div class="chart-card">
      <div class="chart-header">
        <h3>入出库趋势</h3>
        <el-radio-group v-model="trendDays" size="small">
          <el-radio-button :value="7">近7天</el-radio-button>
          <el-radio-button :value="30">近30天</el-radio-button>
        </el-radio-group>
      </div>
      <div class="chart-body">
        <VChart :option="trendChartOption" autoresize style="height: 360px" />
      </div>
    </div>

    <!-- Location Heatmap Chart -->
    <div class="chart-card">
      <div class="chart-header">
        <h3>库位热力图</h3>
      </div>
      <div class="chart-body">
        <VChart :option="heatmapChartOption" autoresize style="height: 360px" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue';
import {
  Odometer,
  Download,
  Upload,
  List,
} from '@element-plus/icons-vue';
import VChart from 'vue-echarts';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { LineChart, BarChart } from 'echarts/charts';
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
  VisualMapComponent,
} from 'echarts/components';
import WmsStatisticsCard from '@/components/common/WmsStatisticsCard.vue';
import {
  getWarehouseDashboard,
  type WarehouseDashboardData,
} from '@/api/dashboard';
import { useDashboardStore } from '@/stores/dashboard';

use([
  CanvasRenderer,
  LineChart,
  BarChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
  VisualMapComponent,
]);

const dashboardStore = useDashboardStore();

// ── State ─────────────────────────────────────────────────────
const loading = ref(true);
const trendDays = ref(7);

const data = reactive<WarehouseDashboardData>({
  occupancyRate: 0,
  inboundCount: 0,
  outboundCount: 0,
  taskRate: 0,
  inboundTrend: [],
  outboundTrend: [],
  locationHeatmap: [],
});

// ── Formatting ────────────────────────────────────────────────
function formatPercent(n: number): string {
  if (n === undefined || n === null) return '--';
  return n.toFixed(1);
}

// ── Chart Options ─────────────────────────────────────────────
const trendChartOption = computed(() => ({
  tooltip: {
    trigger: 'axis',
    axisPointer: { type: 'cross' },
  },
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
    data: data.inboundTrend.map((d) => d.date.slice(5)),
    boundaryGap: false,
  },
  yAxis: { type: 'value', name: '数量' },
  series: [
    {
      name: '入库',
      type: 'line',
      data: data.inboundTrend.map((d) => d.quantity),
      smooth: true,
      lineStyle: { color: '#2563EB', width: 2 },
      itemStyle: { color: '#2563EB' },
      areaStyle: {
        color: {
          type: 'linear',
          x: 0, y: 0, x2: 0, y2: 1,
          colorStops: [
            { offset: 0, color: 'rgba(37,99,235,0.3)' },
            { offset: 1, color: 'rgba(37,99,235,0.02)' },
          ],
        },
      },
    },
    {
      name: '出库',
      type: 'line',
      data: data.outboundTrend.map((d) => d.quantity),
      smooth: true,
      lineStyle: { color: '#D97706', width: 2 },
      itemStyle: { color: '#D97706' },
      areaStyle: {
        color: {
          type: 'linear',
          x: 0, y: 0, x2: 0, y2: 1,
          colorStops: [
            { offset: 0, color: 'rgba(217,119,6,0.3)' },
            { offset: 1, color: 'rgba(217,119,6,0.02)' },
          ],
        },
      },
    },
  ],
}));

const heatmapChartOption = computed(() => ({
  tooltip: {
    trigger: 'axis',
    axisPointer: { type: 'shadow' },
    formatter: (params: any) => {
      const p = Array.isArray(params) ? params[0] : params;
      return `${p.name}<br/>占用率: ${p.value}%`;
    },
  },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '8%',
    top: '8%',
    containLabel: true,
  },
  xAxis: {
    type: 'category',
    data: data.locationHeatmap.map((d) => d.zone),
    axisLabel: { rotate: 30 },
  },
  yAxis: {
    type: 'value',
    name: '占用率(%)',
    max: 100,
  },
  series: [
    {
      type: 'bar',
      data: data.locationHeatmap.map((d) => ({
        value: d.rate,
        itemStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 0, y2: 1,
            colorStops: [
              {
                offset: 0,
                color: d.rate > 80 ? '#DC2626' : d.rate > 60 ? '#D97706' : '#16A34A',
              },
              {
                offset: 1,
                color: d.rate > 80 ? '#FCA5A5' : d.rate > 60 ? '#FDE68A' : '#86EFAC',
              },
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
    const cacheKey = `warehouse-${trendDays.value}`;
    let result = dashboardStore.getCached<WarehouseDashboardData>(cacheKey);
    if (!result) {
      result = await getWarehouseDashboard();
      dashboardStore.setCache(cacheKey, result);
    }
    Object.assign(data, result);
  } catch {
    // Silently handle errors
  } finally {
    loading.value = false;
  }
}

watch(trendDays, () => {
  loadData();
});

onMounted(() => {
  loadData();
});
</script>

<style lang="scss" scoped>
@use '@/styles/variables.scss' as *;

.dashboard-warehouse {
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

@media (max-width: 768px) {
  .dashboard-warehouse {
    padding: $wms-spacing-md;
  }
}
</style>
