<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="物料">
        <el-input v-model="filters.materialCode" placeholder="请输入物料编码" clearable />
      </el-form-item>
      <el-form-item label="仓库">
        <el-input v-model="filters.warehouseId" placeholder="请输入仓库" clearable />
      </el-form-item>
      <el-form-item label="来源单据">
        <el-input v-model="filters.sourceDocType" placeholder="请输入来源单据类型" clearable />
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>库存台账</span>
          <WmsExportButton :export-api="exportLedger" filename="库存台账.xlsx" />
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
        <el-table-column prop="transactionTime" label="时间" min-width="160" />
        <el-table-column prop="materialCode" label="物料编码" />
        <el-table-column prop="sourceDocType" label="来源单据" />
        <el-table-column prop="inQty" label="入库数量" align="right">
          <template #default="{ row }">
            {{ row.inQty || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="outQty" label="出库数量" align="right">
          <template #default="{ row }">
            {{ row.outQty || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="balanceQty" label="结余数量" align="right" />
      </WmsTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import type { InventoryLedgerDto } from '@/api/inventory';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<InventoryLedgerDto>('/api/v1/inventory/ledger-entries');

async function exportLedger() {
  return { fileUrl: '/api/v1/inventory/ledger-entries/export', rowCount: total.value };
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
