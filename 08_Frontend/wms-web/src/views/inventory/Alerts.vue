<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="物料">
        <el-input v-model="filters.materialCode" placeholder="请输入物料编码" clearable />
      </el-form-item>
      <el-form-item label="仓库">
        <el-input v-model="filters.warehouseId" placeholder="请输入仓库" clearable />
      </el-form-item>
      <el-form-item label="预警类型">
        <el-select v-model="filters.alertType" placeholder="请选择预警类型" clearable>
          <el-option label="库存不足" value="LowStock" />
          <el-option label="库存过高" value="OverStock" />
          <el-option label="呆滞库存" value="SlowMoving" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>库存预警</span>
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
        <el-table-column prop="alertType" label="预警类型" />
        <el-table-column prop="warehouseId" label="仓库" />
        <el-table-column prop="threshold" label="阈值" align="right" />
        <el-table-column prop="currentQty" label="当前数量" align="right" />
        <el-table-column prop="status" label="状态" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'danger' : 'info'">{{ row.status === 0 ? '未处理' : '已处理' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as InventoryAlertDto)">查看</el-button>
            <el-button link type="success" :disabled="(row as InventoryAlertDto).status !== 0" @click="handleResolve(row as InventoryAlertDto)">处理</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      title="预警详情"
      :visible="detailVisible"
      show-footer
      width="500px"
      @close="detailVisible = false"
      @cancel="detailVisible = false"
      @confirm="detailVisible = false"
    >
      <el-descriptions :column="1" border>
        <el-descriptions-item label="物料编码">{{ currentAlert?.materialCode }}</el-descriptions-item>
        <el-descriptions-item label="预警类型">{{ currentAlert?.alertType }}</el-descriptions-item>
        <el-descriptions-item label="仓库">{{ currentAlert?.warehouseId }}</el-descriptions-item>
        <el-descriptions-item label="阈值">{{ currentAlert?.threshold }}</el-descriptions-item>
        <el-descriptions-item label="当前数量">{{ currentAlert?.currentQty }}</el-descriptions-item>
        <el-descriptions-item label="状态">{{ currentAlert?.status === 0 ? '未处理' : '已处理' }}</el-descriptions-item>
      </el-descriptions>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsSignalRIndicator from '@/components/common/WmsSignalRIndicator.vue';
import { useTable } from '@/hooks/useTable';
import { useSignalR } from '@/utils/signalr';
import type { InventoryAlertDto } from '@/api/inventory';

const { connected } = useSignalR('/signalr/inventory');
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<InventoryAlertDto>('/api/v1/inventory/alerts');

const currentAlert = ref<InventoryAlertDto | null>(null);
const detailVisible = ref(false);

function handleDetail(row: InventoryAlertDto) {
  currentAlert.value = row;
  detailVisible.value = true;
}

async function handleResolve(_row: InventoryAlertDto) {
  ElMessage.info('处理流程已触发');
  // TODO: 调用处理 API
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
