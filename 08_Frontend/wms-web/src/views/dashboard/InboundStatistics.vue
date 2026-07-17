<template>
  <div class="dashboard-inbound-stats">
    <!-- KPI Cards -->
    <el-row :gutter="16" class="stats-row">
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="入库总量"
          :value="data.inboundCount"
          unit="件"
          :icon="Box"
          color="#2563EB"
          trend="up"
          trend-value="+15.2%"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="质检合格率"
          :value="data.qualityRate"
          unit="%"
          :icon="CircleCheckFilled"
          color="#16A34A"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="供应商数"
          :value="data.supplierDistribution.length"
          unit="个"
          :icon="OfficeBuilding"
          color="#D97706"
        />
      </el-col>
      <el-col :xs="24" :sm="12" :lg="6">
        <WmsStatisticsCard
          title="入库类型"
          :value="data.typeDistribution.length"
          unit="种"
          :icon="Grid"
          color="#0EA5E9"
        />
      </el-col>
    </el-row>

    <!-- Row 1: Inbound Trend + Supplier Distribution -->
    <el-row :gutter="16">
      <el-col :xs="24" :lg="14">
        <div class="chart-card">
          <div class="chart-header"><h3>入库趋势</h3></div>
          <div class="chart-body">
            <VChart :option="trendChartOption" autoresize style="height: 320px" />
          </div>
        </div>
      </el-col>
      <el-col :xs="24" :lg="10">
        <div class="chart-card">
          <div class="chart-header"><h3>供应商分布</h3></div>
          <div class="chart-body">
            <VChart :option="supplierChartOption" autoresize style="height: 320px" />
          </div>
        </div>
      </el-col>
    </el-row>

    <!-- Row 2: Type Distribution + Quality Rate -->
    <el-row :gutter="16">
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header"><h3>类型占比</h3></div>
          <div class="chart-body">
            <VChart :option="typeDistributionChartOption" autoresize style="height: 320px" />
          </div>
        </div>
      </el-col>
      <el-col :xs="24" :lg="12">
        <div class="chart-card">
          <div class="chart-header"><h3>质检合格率</h3></div>
          <div class="quality-rate-body">
            <div class="quality-rate-circle">
              <el-progress
                type="dashboard"
                :percentage="data.qualityRate"
                :color="getQualityColor(data.qualityRate)"
                :stroke-width="14"
                :width="200"
              >
                <template #default>
                  <span class="quality-rate-value">{{ data.qualityRate }}%</span>
                  <span class="quality-rate-label">合格率</span>
                </template>
              </el-progress>
            </div>
            <div class="quality-rate-stats">
              <div class="quality-stat">
                <span class="quality-stat-label">入库总数</span>
                <span class="quality-stat-value">{{ data.inboundCount.toLocaleString() }}</span>
              </div>
              <div class="quality-stat">
                <span class="quality-stat-label">合格数</span>
                <span class="quality-stat-value">{{ Math.round(data.inboundCount * data.qualityRate / 100).toLocaleString() }}</span>
              </div>
            </div>
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
  CircleCheckFilled,
  OfficeBuilding,
  Grid,
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
  getInboundStatsDashboard,
  type InboundStatsDashboardData,
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

const data = reactive<InboundStatsDashboardData>({
  inboundCount: 0,
  supplierDistribution: [],
  qualityRate: 0,
  typeDistribution: [],
  inboundTrend: [],
});

// ── Helpers ───────────────────────────────────────────────────
function getQualityColor(rate: number): string {
  if (rate >= 95) return '#16A34A';
  if (rate >= 80) return '#2563EB';
  if (rate >= 60) return '#D97706';
  return '#DC2626';
}

// ── Chart Options ─────────────────────────────────────────────
const trendChartOption = computed(() => ({
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
    data: data.inboundTrend.map((d) => d.date.slice(5)),
    boundaryGap: false,
  },
  yAxis: { type: 'value', name: '数量' },
  series: [
    {
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
  ],
}));

const supplierChartOption = computed(() => ({
  tooltip: { trigger: 'axis', axisPointer: { type: 'shadow' } },
  grid: {
    left: '3%',
    right: '8%',
    bottom: '8%',
    top: '8%',
    containLabel: true,
  },
  xAxis: { type: 'value' },
  yAxis: {
    type: 'category',
    data: data.supplierDistribution.map((d) => d.supplier),
    axisLabel: { width: 100, overflow: 'truncate' },
  },
  series: [
    {
      type: 'bar',
      data: data.supplierDistribution.map((d) => ({
        value: d.count,
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
      barWidth: '60%',
    },
  ],
}));

const typeDistributionChartOption = computed(() => ({
  tooltip: { trigger: 'item', formatter: '{b}: {c} ({d}%)' },
  legend: {
    orient: 'vertical',
    left: 'left',
    top: 'center',
  },
  series: [
    {
      type: 'pie',
      radius: ['40%', '70%'],
      center: ['55%', '50%'],
      avoidLabelOverlap: false,
      label: { show: false },
      emphasis: {
        label: { show: true, fontSize: 14, fontWeight: 'bold' },
      },
      data: data.typeDistribution.map((d) => ({
        name: d.type,
        value: d.count,
      })),
    },
  ],
  color: ['#2563EB', '#D97706', '#16A34A', '#0EA5E9', '#DC2626', '#475569'],
}));

// ── Data Loading ──────────────────────────────────────────────
async function loadData() {
  loading.value = true;
  try {
    const cacheKey = 'inbound-stats-dashboard';
    let result = dashboardStore.getCached<InboundStatsDashboardData>(cacheKey);
    if (!result) {
      result = await getInboundStatsDashboard();
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

.dashboard-inbound-stats {
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

// ── Quality Rate Display ──────────────────────────────────────
.quality-rate-body {
  padding: $wms-spacing-lg;
  min-height: 320px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: $wms-spacing-lg;
}

.quality-rate-circle {
  display: flex;
  align-items: center;
  justify-content: center;
}

.quality-rate-value {
  font-size: 28px;
  font-weight: 700;
  color: $wms-text-primary;
}

.quality-rate-label {
  display: block;
  font-size: $wms-font-size-small;
  color: $wms-text-secondary;
  margin-top: $wms-spacing-xs;
}

.quality-rate-stats {
  display: flex;
  gap: $wms-spacing-xl;
}

.quality-stat {
  text-align: center;

  .quality-stat-label {
    display: block;
    font-size: $wms-font-size-small;
    color: $wms-text-secondary;
    margin-bottom: $wms-spacing-xs;
  }

  .quality-stat-value {
    font-size: $wms-font-size-number-lg;
    font-weight: 600;
    color: $wms-text-primary;
  }
}

@media (max-width: 768px) {
  .dashboard-inbound-stats {
    padding: $wms-spacing-md;
  }
}
</style>
