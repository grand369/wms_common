<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="冻结单号">
        <el-input v-model="filters.freezeOrderNo" placeholder="请输入冻结单号" clearable />
      </el-form-item>
      <el-form-item label="物料编码">
        <el-input v-model="filters.materialCode" placeholder="请输入物料编码" clearable />
      </el-form-item>
      <el-form-item label="冻结状态">
        <el-select v-model="filters.freezeStatusValue" placeholder="请选择冻结状态" clearable>
          <el-option :label="statusMap[0]" :value="0" />
          <el-option :label="statusMap[1]" :value="1" />
          <el-option :label="statusMap[2]" :value="2" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>冻结记录</span>
          <el-button type="primary" @click="openCreateDialog">
            <el-icon><Plus /></el-icon> 新增冻结
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
        <el-table-column prop="freezeOrderNo" label="冻结单号" />
        <el-table-column prop="materialCode" label="物料编码" />
        <el-table-column prop="freezeQuantity" label="冻结数量" align="right" />
        <el-table-column prop="freezeReason" label="冻结原因" show-overflow-tooltip />
        <el-table-column prop="freezeStatusName" label="状态" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="row.freezeStatusName" type="freeze" />
          </template>
        </el-table-column>
        <el-table-column prop="freezeStartTime" label="冻结时间" width="180" />
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button v-if="row.freezeStatusValue === 0" link type="success" @click="handleRelease(row as InventoryFreezeDto)">解冻</el-button>
            <el-button v-if="row.freezeStatusValue === 0" link type="warning" @click="handleCancel(row as InventoryFreezeDto)">取消</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      title="新增冻结"
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
              :label="`${item.materialCode} - ${item.materialName} - ${item.warehouseName} - ${item.locationName || '无库位'} (可用: ${item.availableQuantity})`"
              :value="item.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="物料编码">
          <span>{{ selectedBalance?.materialCode || '-' }}</span>
        </el-form-item>
        <el-form-item label="物料名称">
          <span>{{ selectedBalance?.materialName || '-' }}</span>
        </el-form-item>
        <el-form-item label="冻结数量" prop="qty">
          <el-input-number v-model="createForm.qty" :min="0.01" :max="selectedBalance?.availableQuantity || 0" :precision="2" />
        </el-form-item>
        <el-form-item label="冻结原因" prop="reason">
          <el-input v-model="createForm.reason" type="textarea" :rows="3" placeholder="请输入冻结原因" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import { useTable } from '@/hooks/useTable';
import { getBalances, freezeBalance, getFreezeOrders, releaseFreeze, cancelFreeze } from '@/api/inventory';
import type { InventoryFreezeDto, InventoryBalanceDto } from '@/api/inventory';

const statusMap: Record<number, string> = { 0: '冻结中', 1: '已解冻', 2: '已取消' };

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<InventoryFreezeDto>('/api/v1/inventory/freeze-orders');

const balanceList = ref<InventoryBalanceDto[]>([]);
const selectedBalance = ref<InventoryBalanceDto | null>(null);
const createVisible = ref(false);
const createSubmitting = ref(false);
const createFormRef = ref<FormInstance>();
const createForm = ref({
  balanceId: '',
  materialId: '',
  materialCode: '',
  qty: 0,
  reason: '',
});

const createRules: FormRules = {
  balanceId: [{ required: true, message: '请选择库存余额', trigger: 'change' }],
  qty: [{ required: true, message: '请输入冻结数量', trigger: 'blur' }],
  reason: [{ required: true, message: '请输入冻结原因', trigger: 'blur' }],
};

async function loadBalances() {
  try {
    const res = await getBalances({ maxResultCount: 1000 });
    balanceList.value = res.items.filter((i) => i.availableQuantity > 0);
  } catch {
    ElMessage.error('加载库存余额失败');
  }
}

function mapFreezeStatus(status: number) {
  const map: Record<number, string> = { 0: 'Active', 1: 'Released', 2: 'Cancelled' };
  return map[status] || 'Active';
}

function openCreateDialog() {
  loadBalances();
  createForm.value = { balanceId: '', materialId: '', materialCode: '', qty: 0, reason: '' };
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
  if (!createFormRef.value || !selectedBalance.value) return;
  try {
    await createFormRef.value.validate();
  } catch {
    return;
  }
  createSubmitting.value = true;
  try {
    await freezeBalance(selectedBalance.value.id, {
      qty: createForm.value.qty,
      reason: createForm.value.reason,
    });
    ElMessage.success('冻结成功');
    createVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('冻结失败');
  } finally {
    createSubmitting.value = false;
  }
}

async function handleRelease(row: InventoryFreezeDto) {
  try {
    await ElMessageBox.confirm('确认解冻该冻结记录？', '提示', { type: 'warning' });
    await releaseFreeze(row.id);
    ElMessage.success('解冻成功');
    handleSearch();
  } catch {
    // Cancel
  }
}

async function handleCancel(row: InventoryFreezeDto) {
  try {
    await ElMessageBox.confirm('确认取消该冻结记录？', '提示', { type: 'warning' });
    await cancelFreeze(row.id);
    ElMessage.success('取消成功');
    handleSearch();
  } catch {
    // Cancel
  }
}

loadBalances();
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
