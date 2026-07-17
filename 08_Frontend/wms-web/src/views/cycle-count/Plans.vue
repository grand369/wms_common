<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="计划单号">
        <el-input v-model="filters.planNo" placeholder="请输入计划单号" clearable />
      </el-form-item>
      <el-form-item label="仓库">
        <el-input v-model="filters.warehouseName" placeholder="请输入仓库" clearable />
      </el-form-item>
      <el-form-item label="盘点类型">
        <el-select v-model="filters.countType" placeholder="请选择盘点类型" clearable>
          <el-option label="全盘" value="Full" />
          <el-option label="抽盘" value="Spot" />
          <el-option label="循环盘点" value="Cycle" />
          <el-option label="动盘" value="Dynamic" />
        </el-select>
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="草稿" :value="0" />
          <el-option label="进行中" :value="1" />
          <el-option label="已完成" :value="2" />
          <el-option label="已取消" :value="3" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>盘点计划列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="openCreateDialog">
              <el-icon><Plus /></el-icon> 新建盘点计划
            </el-button>
          </div>
        </div>
      </template>

      <WmsTable
        :data="tableData"
        :loading="loading"
        :total="total"
        v-model:current-page="pagination.currentPage"
        v-model:page-size="pagination.pageSize"
        :page-sizes="pagination.pageSizes"
        @page-change="handlePageChange"
        @size-change="handleSizeChange"
      >
        <el-table-column prop="planNo" label="计划单号" width="180" />
        <el-table-column prop="warehouseName" label="仓库" show-overflow-tooltip />
        <el-table-column prop="countType" label="盘点类型" width="120">
          <template #default="{ row }">
            {{ mapCountType((row as CycleCountPlanDto).countType) }}
          </template>
        </el-table-column>
        <el-table-column prop="planDate" label="计划日期" width="120" />
        <el-table-column prop="status" label="状态" align="center" width="120">
          <template #default="{ row }">
            <WmsStatusTag :status="mapStatus((row as CycleCountPlanDto).status)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="320" fixed="right">
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              :disabled="(row as CycleCountPlanDto).status === 0"
              @click="handleStart(row as CycleCountPlanDto)"
            >
              开始
            </el-button>
            <el-button
              link
              type="success"
              :disabled="(row as CycleCountPlanDto).status !== 1"
              @click="handleComplete(row as CycleCountPlanDto)"
            >
              完成
            </el-button>
            <el-button
              link
              type="warning"
              :disabled="(row as CycleCountPlanDto).status === 0"
              @click="handleDifference(row as CycleCountPlanDto)"
            >
              差异
            </el-button>
            <el-button link type="info" @click="handleExecute(row as CycleCountPlanDto)">执行</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <!-- 新建盘点计划对话框 -->
    <WmsDialog
      :visible="createDialogVisible"
      title="新建盘点计划"
      width="640px"
      @close="createDialogVisible = false"
      @cancel="createDialogVisible = false"
      @confirm="handleCreateConfirm"
    >
      <WmsForm
        ref="formRef"
        :form-items="formItems"
        :form-data="formData"
        :form-rules="formRules"
        label-width="120px"
        @change="onFieldChange"
      />
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Plus } from '@element-plus/icons-vue';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsForm from '@/components/common/WmsForm.vue';
import { useTable } from '@/hooks/useTable';
import { createCycleCountPlan, startCounting, completeCycleCount } from '@/api/cycleCount';
import type { CycleCountPlanDto, CreateOrUpdateCycleCountPlanDto } from '@/api/cycleCount';
import type { WmsFormItem } from '@/components/common/WmsForm.vue';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<CycleCountPlanDto>('/api/v1/cycle-count/plans');

const createDialogVisible = ref(false);
const submitting = ref(false);
const formRef = ref<any>(null);

const formData = reactive<CreateOrUpdateCycleCountPlanDto>({
  warehouseId: '',
  countType: 'Full',
  planDate: '',
  locationIds: [],
  materialIds: [],
});

const formRules = {
  warehouseId: [{ required: true, message: '请输入仓库ID', trigger: 'change' }],
  countType: [{ required: true, message: '请选择盘点类型', trigger: 'change' }],
  planDate: [{ required: true, message: '请选择计划日期', trigger: 'change' }],
};

const formItems: WmsFormItem[] = [
  { prop: 'warehouseId', label: '仓库ID', type: 'input', span: 12, rules: formRules.warehouseId, placeholder: '请输入仓库ID' },
  {
    prop: 'countType',
    label: '盘点类型',
    type: 'select',
    span: 12,
    rules: formRules.countType,
    options: [
      { label: '全盘', value: 'Full' },
      { label: '抽盘', value: 'Spot' },
      { label: '循环盘点', value: 'Cycle' },
      { label: '动盘', value: 'Dynamic' },
    ],
  },
  { prop: 'planDate', label: '计划日期', type: 'date', span: 12, rules: formRules.planDate, dateType: 'date' },
];

function onFieldChange(_prop: string, _value: any) {
  /* no-op */
}

function mapCountType(type: string) {
  const map: Record<string, string> = {
    Full: '全盘',
    Spot: '抽盘',
    Cycle: '循环盘点',
    Dynamic: '动盘',
  };
  return map[type] || type;
}

function mapStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'InProgress', 2: 'Completed', 3: 'Cancelled' };
  return map[status] || 'Draft';
}

function openCreateDialog() {
  formData.warehouseId = '';
  formData.countType = 'Full';
  formData.planDate = '';
  formData.locationIds = [];
  formData.materialIds = [];
  createDialogVisible.value = true;
}

async function handleCreateConfirm() {
  try {
    if (formRef.value && formRef.value.validate) {
      const valid = await formRef.value.validate();
      if (!valid) return;
    }
  } catch {
    return;
  }
  if (!formData.warehouseId || !formData.countType || !formData.planDate) {
    ElMessage.warning('请填写完整的盘点计划信息');
    return;
  }
  submitting.value = true;
  try {
    await createCycleCountPlan({ ...formData });
    ElMessage.success('创建成功');
    createDialogVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('创建失败');
  } finally {
    submitting.value = false;
  }
}

async function handleStart(row: CycleCountPlanDto) {
  try {
    await ElMessageBox.confirm(`确认开始执行盘点计划 ${row.planNo}？`, '提示', { type: 'warning' });
  } catch {
    return;
  }
  try {
    await startCounting(row.id);
    ElMessage.success('已开始盘点');
    handleSearch();
  } catch {
    ElMessage.error('开始失败');
  }
}

async function handleComplete(row: CycleCountPlanDto) {
  try {
    await ElMessageBox.confirm(`确认完成盘点计划 ${row.planNo}？`, '提示', { type: 'warning' });
  } catch {
    return;
  }
  try {
    await completeCycleCount(row.id);
    ElMessage.success('已完成');
    handleSearch();
  } catch {
    ElMessage.error('完成失败');
  }
}

function handleExecute(row: CycleCountPlanDto) {
  router.push(`/cycle-count/execute/${row.id}`);
}

function handleDifference(row: CycleCountPlanDto) {
  router.push(`/cycle-count/difference/${row.id}`);
}

handleSearch();
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
</style>
