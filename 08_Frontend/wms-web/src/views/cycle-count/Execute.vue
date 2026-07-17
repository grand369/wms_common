<template>
  <div class="page-container">
    <el-page-header title="返回计划" content="盘点执行" @back="goBack" />

    <el-card shadow="hover" class="detail-card">
      <template #header>
        <div class="card-header">
          <span>盘点计划：{{ planId }}</span>
          <div class="header-actions">
            <el-switch
              v-model="blindMode"
              active-text="盲盘模式"
              inactive-text="普通模式"
              inline-prompt
            />
          </div>
        </div>
      </template>

      <el-table
        v-loading="loading"
        :data="records"
        border
        stripe
        class="count-table"
        :row-class-name="getRowClassName"
      >
        <el-table-column type="index" label="序号" width="60" align="center" />
        <el-table-column prop="materialCode" label="物料编码" min-width="140" />
        <el-table-column prop="locationId" label="库位" min-width="120">
          <template #default="{ row }">
            {{ row.locationId || '-' }}
          </template>
        </el-table-column>
        <el-table-column
          v-if="!blindMode"
          prop="systemQty"
          label="系统数量"
          width="120"
          align="right"
        />
        <el-table-column label="盘点数量" min-width="160" align="center">
          <template #default="{ row, $index }">
            <el-input-number
              v-model="row._countQty"
              :min="0"
              :precision="0"
              :controls="true"
              size="small"
              style="width: 140px"
              placeholder="输入盘点数"
            />
          </template>
        </el-table-column>
        <el-table-column
          v-if="!blindMode"
          label="差异"
          width="100"
          align="right"
        >
          <template #default="{ row }">
            <span
              :class="{
                'diff-positive': getDiff(row) > 0,
                'diff-negative': getDiff(row) < 0,
              }"
            >
              {{ formatDiff(getDiff(row)) }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right" align="center">
          <template #default="{ row }">
            <el-button
              link
              type="success"
              :disabled="!isRowEdited(row) || row._confirmed"
              :loading="row._submitting"
              @click="handleConfirmOne(row)"
            >
              {{ row._confirmed ? '已确认' : '确认' }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <el-empty v-if="!loading && records.length === 0" description="暂无盘点记录" />

      <div class="count-actions" v-if="records.length > 0">
        <el-button type="primary" :loading="batchSubmitting" @click="handleBatchConfirm">
          批量确认
        </el-button>
        <el-button type="success" :disabled="!allConfirmed" @click="handleComplete">
          完成盘点
        </el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import {
  getCycleCountRecords,
  submitCount,
  completeCycleCount,
  type CycleCountRecordDto,
} from '@/api/cycleCount';

interface EditableRecord extends CycleCountRecordDto {
  _countQty: number;
  _confirmed: boolean;
  _submitting: boolean;
  _originalCount: number;
}

const route = useRoute();
const router = useRouter();
const planId = route.params.id as string;

const blindMode = ref(true);
const loading = ref(false);
const batchSubmitting = ref(false);
const records = ref<EditableRecord[]>([]);

const allConfirmed = computed(() => {
  if (records.value.length === 0) return false;
  return records.value.every((r) => r._confirmed);
});

function getDiff(row: EditableRecord): number {
  return (row._countQty ?? row.countQty ?? 0) - (row.systemQty ?? 0);
}

function formatDiff(value: number): string {
  if (value > 0) return `+${value}`;
  if (value === 0) return '0';
  return `${value}`;
}

function isRowEdited(row: EditableRecord): boolean {
  return (row._countQty ?? row.countQty) !== row._originalCount;
}

function getRowClassName({ row }: { row: EditableRecord }): string {
  if (row._confirmed) return 'row-confirmed';
  if (!isRowEdited(row)) return '';
  return 'row-edited';
}

async function loadRecords() {
  loading.value = true;
  try {
    const result = await getCycleCountRecords(planId);
    records.value = (result.items || []).map((item) => ({
      ...item,
      _countQty: item.countQty ?? 0,
      _confirmed: false,
      _submitting: false,
      _originalCount: item.countQty ?? 0,
    }));
  } catch {
    ElMessage.error('加载盘点记录失败');
  } finally {
    loading.value = false;
  }
}

async function handleConfirmOne(row: EditableRecord) {
  row._submitting = true;
  try {
    await submitCount(planId, {
      recordId: row.id,
      countQty: row._countQty,
    });
    row.countQty = row._countQty;
    row._confirmed = true;
    row._originalCount = row._countQty;
    ElMessage.success('确认成功');
  } catch {
    ElMessage.error('确认失败');
  } finally {
    row._submitting = false;
  }
}

async function handleBatchConfirm() {
  const edited = records.value.filter((r) => !r._confirmed && isRowEdited(r));
  if (edited.length === 0) {
    ElMessage.info('没有需要确认的记录');
    return;
  }

  try {
    await ElMessageBox.confirm(
      `确认提交 ${edited.length} 条盘点记录？`,
      '批量确认',
      { type: 'warning' }
    );
  } catch {
    return;
  }

  batchSubmitting.value = true;
  let successCount = 0;
  let failCount = 0;

  for (const row of edited) {
    row._submitting = true;
    try {
      await submitCount(planId, {
        recordId: row.id,
        countQty: row._countQty,
      });
      row.countQty = row._countQty;
      row._confirmed = true;
      row._originalCount = row._countQty;
      successCount++;
    } catch {
      failCount++;
    } finally {
      row._submitting = false;
    }
  }

  batchSubmitting.value = false;
  if (failCount === 0) {
    ElMessage.success(`批量确认成功，共 ${successCount} 条`);
  } else {
    ElMessage.warning(`确认完成：成功 ${successCount} 条，失败 ${failCount} 条`);
  }
}

async function handleComplete() {
  try {
    await ElMessageBox.confirm(
      '确认完成盘点计划？完成后将进入差异处理阶段。',
      '完成盘点',
      { type: 'warning' }
    );
    await completeCycleCount(planId);
    ElMessage.success('盘点已完成');
    router.push(`/cycle-count/difference/${planId}`);
  } catch {
    // user cancelled or error
  }
}

function goBack() {
  router.push('/cycle-count/plans');
}

onMounted(() => {
  loadRecords();
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
  gap: 12px;
  align-items: center;
}

.count-table {
  margin-top: 8px;

  :deep(.row-confirmed) {
    background-color: rgba(22, 163, 74, 0.05);
  }

  :deep(.row-edited) {
    background-color: rgba(217, 119, 6, 0.05);
  }
}

.count-actions {
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
