<template>
  <div class="page-container">
    <el-page-header title="返回执行" content="差异处理" @back="goBack" />

    <el-card shadow="hover" class="detail-card">
      <template #header>
        <div class="card-header">
          <span>差异分析：{{ planId }}</span>
          <div class="header-actions">
            <el-tag v-if="threshold > 0" type="warning" size="small">
              差异阈值: {{ threshold }}
            </el-tag>
          </div>
        </div>
      </template>

      <el-table
        v-loading="loading"
        :data="differences"
        border
        stripe
        class="diff-table"
        :row-class-name="getRowClassName"
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="50" />
        <el-table-column type="index" label="序号" width="60" align="center" />
        <el-table-column prop="materialCode" label="物料编码" min-width="140" />
        <el-table-column prop="locationId" label="库位" min-width="120">
          <template #default="{ row }">
            {{ row.locationId || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="systemQty" label="系统数量" width="120" align="right" />
        <el-table-column prop="countQty" label="实盘数量" width="120" align="right" />
        <el-table-column label="差异数量" width="120" align="right">
          <template #default="{ row }">
            <span
              :class="{
                'diff-positive': row.differenceQty > 0,
                'diff-negative': row.differenceQty < 0,
              }"
            >
              {{ formatDiff(row.differenceQty) }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="差异状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag
              :type="isExceedThreshold(row) ? 'danger' : 'success'"
              size="small"
              effect="dark"
            >
              {{ isExceedThreshold(row) ? '异常' : '正常' }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>

      <el-empty v-if="!loading && differences.length === 0" description="暂无差异记录" />

      <div class="diff-actions" v-if="differences.length > 0">
        <el-button
          type="warning"
          :disabled="selectedRows.length === 0"
          @click="handleConfirmDiff"
        >
          确认差异 ({{ selectedRows.length }})
        </el-button>
        <el-button type="primary" :loading="adjusting" @click="handleGenerateAdjustment">
          生成调整单
        </el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import {
  getCycleCountDifferences,
  confirmDifference,
  generateAdjustment,
  type CycleCountRecordDto,
} from '@/api/cycleCount';

const route = useRoute();
const router = useRouter();
const planId = route.params.id as string;

const loading = ref(false);
const adjusting = ref(false);
const differences = ref<CycleCountRecordDto[]>([]);
const selectedRows = ref<CycleCountRecordDto[]>([]);
const threshold = ref(0);

function formatDiff(value: number): string {
  if (value > 0) return `+${value}`;
  if (value === 0) return '0';
  return `${value}`;
}

function isExceedThreshold(row: CycleCountRecordDto): boolean {
  return Math.abs(row.differenceQty) > threshold.value;
}

function getRowClassName({ row }: { row: CycleCountRecordDto }): string {
  if (isExceedThreshold(row)) return 'row-exceed';
  return '';
}

function handleSelectionChange(rows: CycleCountRecordDto[]) {
  selectedRows.value = rows;
}

async function loadDifferences() {
  loading.value = true;
  try {
    const result = await getCycleCountDifferences(planId);
    differences.value = result.items || [];

    // Calculate threshold: use max risk tolerance or default > 0
    if (differences.value.length > 0) {
      threshold.value = 0;
    }
  } catch {
    ElMessage.error('加载差异数据失败');
  } finally {
    loading.value = false;
  }
}

async function handleConfirmDiff() {
  if (selectedRows.value.length === 0) {
    ElMessage.warning('请先选择要确认的差异记录');
    return;
  }

  try {
    const { value: reason } = await ElMessageBox.prompt(
      '请输入差异原因（选填）',
      '确认差异',
      {
        confirmButtonText: '确认',
        cancelButtonText: '取消',
        inputPlaceholder: '差异原因...',
      }
    ).catch(() => ({ value: undefined }));

    if (reason === undefined) return;

    await confirmDifference(planId, { reason });
    ElMessage.success('差异确认成功');
    loadDifferences();
  } catch {
    ElMessage.error('差异确认失败');
  }
}

async function handleGenerateAdjustment() {
  try {
    await ElMessageBox.confirm(
      '确认根据当前差异数据生成调整单？',
      '生成调整单',
      { type: 'warning' }
    );
  } catch {
    return;
  }

  adjusting.value = true;
  try {
    await generateAdjustment(planId);
    ElMessage.success('调整单已生成');
  } catch {
    ElMessage.error('生成调整单失败');
  } finally {
    adjusting.value = false;
  }
}

function goBack() {
  router.push(`/cycle-count/execute/${planId}`);
}

onMounted(() => {
  loadDifferences();
});
</script>

<style scoped lang="scss">
@use '@/styles/variables.scss' as *;

.page-container {
  padding: 0;
}

.detail-card {
  margin-top: 16px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.diff-table {
  margin-top: 8px;

  :deep(.row-exceed) {
    background-color: rgba(220, 38, 38, 0.08) !important;
  }
}

.diff-actions {
  margin-top: 16px;
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

.diff-positive {
  color: $wms-color-success;
  font-weight: 600;
}

.diff-negative {
  color: #DC2626;
  font-weight: 600;
}
</style>
