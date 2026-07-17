<template>
  <div class="page-container">
    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>库存调整</span>
          <el-button type="primary" @click="openCreateDialog">
            <el-icon><Plus /></el-icon> 新增调整
          </el-button>
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
        <el-table-column prop="materialCode" label="物料编码" />
        <el-table-column prop="adjustQty" label="调整数量" align="right" />
        <el-table-column prop="reason" label="调整原因" show-overflow-tooltip />
        <el-table-column prop="status" label="状态" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'warning' : 'success'">{{ row.status === 0 ? '待确认' : '已确认' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" :disabled="(row as InventoryAdjustmentDto).status !== 0" @click="handleConfirm(row as InventoryAdjustmentDto)">确认</el-button>
            <el-button link type="danger" :disabled="(row as InventoryAdjustmentDto).status !== 0" @click="handleDelete(row as InventoryAdjustmentDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      title="新增调整"
      :visible="createVisible"
      show-footer
      width="700px"
      :confirm-loading="createSubmitting"
      @close="createVisible = false"
      @cancel="createVisible = false"
      @confirm="handleCreateSubmit"
    >
      <el-form ref="createFormRef" :model="createForm" :rules="createRules" label-width="120px">
        <el-form-item label="选择库存余额" prop="balanceId">
          <el-select v-model="createForm.balanceId" placeholder="请选择库存余额" clearable style="width: 100%" @change="onBalanceChange">
            <el-option
              v-for="item in balanceList"
              :key="item.id"
              :label="`${item.materialCode} - ${item.warehouseName} - ${item.locationName || '无库位'} (可用: ${item.availableQty})`"
              :value="item.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="物料编码">
          <span>{{ selectedBalance?.materialCode || '-' }}</span>
        </el-form-item>
        <el-form-item label="调整数量" prop="adjustQty">
          <el-input-number v-model="createForm.adjustQty" :precision="2" />
        </el-form-item>
        <el-form-item label="调整原因" prop="reason">
          <el-input v-model="createForm.reason" type="textarea" :rows="3" placeholder="请输入调整原因" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { useTable } from '@/hooks/useTable';
import { createAdjustment, confirmAdjustment, getBalances } from '@/api/inventory';
import type { InventoryAdjustmentDto, InventoryBalanceDto } from '@/api/inventory';

const { loading, tableData, total, pagination, handlePageChange, handleSizeChange, handleSearch } =
  useTable<InventoryAdjustmentDto>('/api/v1/inventory/adjustments');

const balanceList = ref<InventoryBalanceDto[]>([]);
const selectedBalance = ref<InventoryBalanceDto | null>(null);
const createVisible = ref(false);
const createSubmitting = ref(false);
const createFormRef = ref<FormInstance>();
const createForm = ref({
  balanceId: '',
  materialId: '',
  materialCode: '',
  adjustQty: 0,
  reason: '',
});

const createRules: FormRules = {
  balanceId: [{ required: true, message: '请选择库存余额', trigger: 'change' }],
  adjustQty: [{ required: true, message: '请输入调整数量', trigger: 'blur' }],
  reason: [{ required: true, message: '请输入调整原因', trigger: 'blur' }],
};

async function loadBalances() {
  try {
    const res = await getBalances({ maxResultCount: 1000 });
    balanceList.value = res.items;
  } catch {
    ElMessage.error('加载库存余额失败');
  }
}

function openCreateDialog() {
  createForm.value = { balanceId: '', materialId: '', materialCode: '', adjustQty: 0, reason: '' };
  selectedBalance.value = null;
  createVisible.value = true;
}

function onBalanceChange(balanceId: string) {
  selectedBalance.value = balanceList.value.find((b) => b.id === balanceId) || null;
  if (selectedBalance.value) {
    createForm.value.materialId = selectedBalance.value.materialId;
    createForm.value.materialCode = selectedBalance.value.materialCode;
  }
}

async function handleCreateSubmit() {
  if (!createFormRef.value) return;
  try {
    await createFormRef.value.validate();
  } catch {
    return;
  }
  createSubmitting.value = true;
  try {
    await createAdjustment({
      balanceId: createForm.value.balanceId,
      materialId: createForm.value.materialId,
      materialCode: createForm.value.materialCode,
      adjustQty: createForm.value.adjustQty,
      reason: createForm.value.reason,
    } as InventoryAdjustmentDto);
    ElMessage.success('创建调整成功');
    createVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('创建调整失败');
  } finally {
    createSubmitting.value = false;
  }
}

async function handleConfirm(row: InventoryAdjustmentDto) {
  try {
    await confirmAdjustment(row.id);
    ElMessage.success('确认成功');
    handleSearch();
  } catch {
    ElMessage.error('确认失败');
  }
}

async function handleDelete(_row: InventoryAdjustmentDto) {
  try {
    await ElMessageBox.confirm('确认删除该调整单？', '提示', { type: 'warning' });
    // 目前 API 未提供删除接口，仅作提示
    ElMessage.info('删除功能待后端支持');
  } catch {
    // Cancel
  }
}

onMounted(() => {
  loadBalances();
});

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
</style>
