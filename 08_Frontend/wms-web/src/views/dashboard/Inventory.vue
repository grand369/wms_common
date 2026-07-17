<template>
  <div class="dashboard-inventory">
    <!-- KPI Cards -->
    <el-row :gutter="16" class="stats-row">
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="库存总值"
          :value="kpiCards.totalValue"
          unit="件"
          :icon="Coin"
          color="#2563EB"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="预警总数"
          :value="kpiCards.alertTotal"
          unit="条"
          :icon="WarningFilled"
          color="#DC2626"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="冻结总数"
          :value="kpiCards.frozenTotal"
          unit="件"
          :icon="Lock"
          color="#D97706"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="调整次数"
          :value="kpiCards.adjustmentTotal"
          unit="次"
          :icon="Setting"
          color="#0EA5E9"
        />
      </el-col>
    </el-row>

    <!-- 2x2 Charts Grid -->
    <el-row :gutter="16">
      <!-- Row 1: Distribution Pie + Alert Trend -->
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header"><h3>库存分布</h3></div>
          <div class="chart-body">
            <VChart :option="distributionChartOption" autoresize style="height: 320px" />
          </div>
        </div>
      </el-col>
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header"><h3>预警趋势</h3></div>
          <div class="chart-body">
            <VChart :option="alertTrendChartOption" autoresize style="height: 320px" />
          </div>
        </div>
      </el-col>
    </el-row>

    <el-row :gutter="16">
      <!-- Row 2: Frozen Stats + Adjustment Trend -->
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header"><h3>冻结统计</h3></div>
          <div class="chart-body">
            <VChart :option="frozenStatsChartOption" autoresize style="height: 320px" />
          </div>
        </div>
      </el-col>
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header"><h3>调整趋势</h3></div>
          <div class="chart-body">
            <VChart :option="adjustmentTrendChartOption" autoresize style="height: 320px" />
          </div>
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import {
  Coin,
  WarningFilled,
  Lock,
  Setting,
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
import WmsStatisticsCard from '@/components/common/WmsStatisticsCard.vue';
import {
  getInventoryDashboard,
  type InventoryDashboardData,
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

// ── State ─────────────────────────────────────────────────────
const loading = ref(true);

const data = reactive<InventoryDashboardData>({
  distribution: [],
  alertTrend: [],
  frozenStats: [],
  adjustmentTrend: [],
});

// ── KPI Computed ──────────────────────────────────────────────
const kpiCards = computed(() => ({
  totalValue: data.distribution.reduce((sum, d) => sum + d.value, 0),
  alertTotal: data.alertTrend.reduce((sum, d) => sum + d.count, 0),
  frozenTotal: data.frozenStats.reduce((sum, d) => sum + d.count, 0),
  adjustmentTotal: data.adjustmentTrend.reduce((sum, d) => sum + d.count, 0),
}));

// ── Chart Options ─────────────────────────────────────────────
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
      radius: ['45%', '72%'],
      center: ['55%', '50%'],
      avoidLabelOverlap: false,
      label: { show: false },
      emphasis: {
        label: { show: true, fontSize: 14, fontWeight: 'bold' },
      },
      data: data.distribution.map((d) => ({
        name: d.category,
        value: d.value,
      })),
    },
  ],
  color: ['#2563EB', '#D97706', '#16A34A', '#0EA5E9', '#DC2626', '#475569'],
}));

const alertTrendChartOption = computed(() => ({
  tooltip: { trigger: 'axis' },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '8%',
    top: '8%',
    containLabel: true,
  },
  xAxis: {
    type: 'category',
    data: data.alertTrend.map((d) => d.date.slice(5)),
    boundaryGap: false,
  },
  yAxis: { type: 'value', name: '数量' },
  series: [
    {
      type: 'line',
      data: data.alertTrend.map((d) => d.count),
      smooth: true,
      lineStyle: { color: '#DC2626', width: 2 },
      itemStyle: { color: '#DC2626' },
      areaStyle: {
        color: {
          type: 'linear',
          x: 0, y: 0, x2: 0, y2: 1,
          colorStops: [
            { offset: 0, color: 'rgba(220,38,38,0.25)' },
            { offset: 1, color: 'rgba(220,38,38,0.02)' },
          ],
        },
      },
    },
  ],
}));

const frozenStatsChartOption = computed(() => ({
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
    data: data.frozenStats.map((d) => d.status),
  },
  yAxis: { type: 'value', name: '数量' },
  series: [
    {
      type: 'bar',
      data: data.frozenStats.map((d) => ({
        value: d.count,
        itemStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 0, y2: 1,
            colorStops: [
              { offset: 0, color: '#D97706' },
              { offset: 1, color: '#FDE68A' },
            ],
          },
        },
      })),
      barWidth: '50%',
    },
  ],
}));

const adjustmentTrendChartOption = computed(() => ({
  tooltip: { trigger: 'axis' },
  grid: {
    left: '3%',
    right: '4%',
    bottom: '8%',
    top: '8%',
    containLabel: true,
  },
  xAxis: {
    type: 'category',
    data: data.adjustmentTrend.map((d) => d.date.slice(5)),
    boundaryGap: false,
  },
  yAxis: { type: 'value', name: '次数' },
  series: [
    {
      type: 'line',
      data: data.adjustmentTrend.map((d) => d.count),
      smooth: true,
      lineStyle: { color: '#0EA5E9', width: 2 },
      itemStyle: { color: '#0EA5E9' },
      areaStyle: {
        color: {
          type: 'linear',
          x: 0, y: 0, x2: 0, y2: 1,
          colorStops: [
            { offset: 0, color: 'rgba(14,165,233,0.25)' },
            { offset: 1, color: 'rgba(14,165,233,0.02)' },
          ],
        },
      },
    },
  ],
}));

// ── Data Loading ──────────────────────────────────────────────
async function loadData() {
  loading.value = true;
  try {
    const cacheKey = 'inventory-dashboard';
    let result = dashboardStore.getCached<InventoryDashboardData>(cacheKey);
    if (!result) {
      result = await getInventoryDashboard();
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

.dashboard-inventory {
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
  .dashboard-inventory {
    padding: $wms-spacing-md;
  }
}
</style>
