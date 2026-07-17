<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="任务号">
        <el-input v-model="filters.jobNo" placeholder="请输入任务号" clearable />
      </el-form-item>
      <el-form-item label="任务类型">
        <el-input v-model="filters.jobType" placeholder="请输入任务类型" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="等待中" :value="0" />
          <el-option label="打印中" :value="1" />
          <el-option label="已完成" :value="2" />
          <el-option label="失败" :value="3" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>打印任务列表</span>
          <div class="header-actions">
            <WmsExportButton :export-api="exportJobs" filename="打印任务清单.xlsx" />
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
        <el-table-column prop="jobNo" label="任务号" />
        <el-table-column prop="jobType" label="任务类型" />
        <el-table-column prop="printerName" label="打印机" />
        <el-table-column prop="printedTime" label="打印时间" />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapStatus(row.status)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button
              link
              type="warning"
              :disabled="(row as PrintJobDto).status !== 3"
              :loading="retryingId === (row as PrintJobDto).id"
              @click="handleRetry(row as PrintJobDto)"
            >
              重试
            </el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import { retryPrint } from '@/api/barcodeLabel';
import type { PrintJobDto } from '@/api/barcodeLabel';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<PrintJobDto>('/api/v1/barcode-label/print-jobs');

const retryingId = ref('');

function mapStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'InProgress', 2: 'Completed', 3: 'Cancelled' };
  return map[status] || 'Draft';
}

async function handleRetry(row: PrintJobDto) {
  retryingId.value = row.id;
  try {
    await retryPrint(row.id);
    ElMessage.success('重试成功');
    handleSearch();
  } catch {
    ElMessage.error('重试失败');
  } finally {
    retryingId.value = '';
  }
}

async function exportJobs() {
  return { fileUrl: '/api/v1/barcode-label/print-jobs/export', rowCount: total.value };
}

handleSearch();
</script>

<style scoped lang="scss">
.page-container { padding: 0; }
.card-header { display: flex; justify-content: space-between; align-items: center; }
.header-actions { display: flex; gap: 8px; }
</style>
