<template>
  <div class="page-container">
    <el-card shadow="hover" class="filter-card">
      <template #header>
        <div class="card-header">
          <span>
            <el-icon><Location /></el-icon>
            在途调拨实时跟踪
          </span>
          <div class="header-actions">
            <WmsSignalRIndicator show-label />
            <el-button :icon="Refresh" @click="handleRefresh">刷新</el-button>
          </div>
        </div>
      </template>

      <WmsSearch @search="handleSearch" @reset="resetFilters">
        <el-form-item label="调拨单号">
          <el-input v-model="filters.transferNo" placeholder="请输入调拨单号" clearable />
        </el-form-item>
        <el-form-item label="源仓库">
          <el-input v-model="filters.fromWarehouseName" placeholder="请输入源仓库" clearable />
        </el-form-item>
        <el-form-item label="目标仓库">
          <el-input v-model="filters.toWarehouseName" placeholder="请输入目标仓库" clearable />
        </el-form-item>
      </WmsSearch>
    </el-card>

    <el-card shadow="hover" class="table-card">
      <template #header>
        <div class="card-header">
          <span>在途列表</span>
          <span class="subtitle">仅显示状态为「在途」的调拨单</span>
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
        <el-table-column prop="transferNo" label="调拨单号" width="180" />
        <el-table-column prop="fromWarehouseName" label="源仓库" show-overflow-tooltip />
        <el-table-column prop="toWarehouseName" label="目标仓库" show-overflow-tooltip />
        <el-table-column prop="planDate" label="计划日期" width="120" />
        <el-table-column prop="status" label="状态" align="center" width="120">
          <template #default>
            <el-tag type="warning" effect="dark">在途</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as TransferDto)">详情</el-button>
            <el-button link type="primary" @click="handleTrack(row as TransferDto)">跟踪</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import { Location, Refresh } from '@element-plus/icons-vue';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsSignalRIndicator from '@/components/common/WmsSignalRIndicator.vue';
import { useTable } from '@/hooks/useTable';
import { useTransferStore } from '@/stores/transfer';
import type { TransferDto } from '@/api/transfer';

const router = useRouter();
const transferStore = useTransferStore();

// Force filter to status=4 (InTransit) regardless of user input
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters, fetchData } =
  useTable<TransferDto>('/api/v1/transfer/orders');

filters.status = 4;

function handleDetail(row: TransferDto) {
  router.push(`/transfer/detail/${row.id}`);
}

function handleTrack(row: TransferDto) {
  router.push(`/transfer/detail/${row.id}`);
}

function handleRefresh() {
  fetchData();
}

function applyStoreUpdate() {
  // 监听 store 变化自动更新列表
  tableData.value = tableData.value.map((row) => {
    const stored = transferStore.getTransfer(row.id);
    return stored ? { ...row, status: stored.status } : row;
  });
}

let pollTimer: number | null = null;
function startPolling() {
  pollTimer = window.setInterval(() => {
    fetchData();
  }, 30000);
}
function stopPolling() {
  if (pollTimer) {
    clearInterval(pollTimer);
    pollTimer = null;
  }
}

watch(tableData, applyStoreUpdate, { deep: true });

onMounted(() => {
  handleSearch();
  startPolling();
});
onUnmounted(() => {
  stopPolling();
});
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 16px;
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
.subtitle {
  font-size: 12px;
  color: #909399;
}
.filter-card,
.table-card {
  margin: 0;
}
</style>
