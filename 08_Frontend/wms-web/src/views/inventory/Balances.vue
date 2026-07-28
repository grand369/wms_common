<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="仓库">
        <el-input v-model="filters.warehouseName" placeholder="请输入仓库名称" clearable />
      </el-form-item>
      <el-form-item label="物料">
        <el-input v-model="filters.materialName" placeholder="请输入物料名称" clearable />
      </el-form-item>
      <el-form-item label="库位">
        <el-input v-model="filters.locationName" placeholder="请输入库位" clearable />
      </el-form-item>
      <el-form-item label="批次">
        <el-input v-model="filters.batchNo" placeholder="请输入批次号" clearable />
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>库存余额</span>
          <WmsSignalRIndicator :status="connected ? 'connected' : 'disconnected'" show-label />
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
        <el-table-column prop="materialName" label="物料名称" show-overflow-tooltip />
        <el-table-column prop="warehouseName" label="仓库" />
        <el-table-column prop="locationName" label="库位" />
        <el-table-column prop="batchNumber" label="批次" />
        <el-table-column prop="quantity" label="总数量" align="right" />
        <el-table-column prop="availableQuantity" label="可用数量" align="right" />
        <el-table-column prop="frozenQuantity" label="冻结数量" align="right" />
        <el-table-column prop="reservedQuantity" label="预留数量" align="right" />
        <el-table-column prop="inventoryStatusName" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapInventoryStatus(row.inventoryStatusValue)" type="inventory" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as InventoryBalanceDto)">详情</el-button>
            <el-button link type="warning" @click="handleFreeze(row as InventoryBalanceDto)">冻结</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      title="冻结库存"
      :visible="freezeVisible"
      show-footer
      width="500px"
      :confirm-loading="freezeSubmitting"
      @close="freezeVisible = false"
      @cancel="freezeVisible = false"
      @confirm="handleFreezeSubmit"
    >
      <el-form ref="freezeFormRef" :model="freezeForm" :rules="freezeRules" label-width="100px">
        <el-form-item label="物料">
          <span>{{ currentRow?.materialCode }} - {{ currentRow?.materialName }}</span>
        </el-form-item>
        <el-form-item label="库位">
          <span>{{ currentRow?.locationName || '-' }}</span>
        </el-form-item>
        <el-form-item label="可冻结数量">
          <span>{{ currentRow?.availableQuantity }}</span>
        </el-form-item>
        <el-form-item label="冻结数量" prop="qty">
          <el-input-number v-model="freezeForm.qty" :min="0.01" :max="currentRow?.availableQuantity || 0" :precision="2" />
        </el-form-item>
        <el-form-item label="冻结原因" prop="reason">
          <el-input v-model="freezeForm.reason" type="textarea" :rows="3" placeholder="请输入冻结原因" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsSignalRIndicator from '@/components/common/WmsSignalRIndicator.vue';
import { useTable } from '@/hooks/useTable';
import { useSignalR } from '@/utils/signalr';
import { freezeBalance } from '@/api/inventory';
import type { InventoryBalanceDto } from '@/api/inventory';

const router = useRouter();
const { connected } = useSignalR('/signalr/inventory');

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<InventoryBalanceDto>('/api/v1/inventory/balances');

const currentRow = ref<InventoryBalanceDto | null>(null);
const freezeVisible = ref(false);
const freezeSubmitting = ref(false);
const freezeFormRef = ref<FormInstance>();
const freezeForm = ref({ qty: 0, reason: '' });
const freezeRules: FormRules = {
  qty: [{ required: true, message: '请输入冻结数量', trigger: 'blur' }],
  reason: [{ required: true, message: '请输入冻结原因', trigger: 'blur' }],
};

function mapInventoryStatus(status: number) {
  const map: Record<number, string> = { 0: 'Available', 1: 'Frozen', 2: 'PendingInspection', 3: 'Quarantined', 4: 'InTransit' };
  return map[status] || 'Available';
}

function handleDetail(row: InventoryBalanceDto) {
  router.push(`/inventory/balance-detail/${row.id}`);
}

function handleFreeze(row: InventoryBalanceDto) {
  currentRow.value = row;
  freezeForm.value = { qty: 0, reason: '' };
  freezeVisible.value = true;
}

async function handleFreezeSubmit() {
  if (!freezeFormRef.value || !currentRow.value) return;
  try {
    await freezeFormRef.value.validate();
  } catch {
    return;
  }
  freezeSubmitting.value = true;
  try {
    await freezeBalance(currentRow.value.id, { qty: freezeForm.value.qty, reason: freezeForm.value.reason });
    ElMessage.success('冻结成功');
    freezeVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('冻结失败');
  } finally {
    freezeSubmitting.value = false;
  }
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
</style>
