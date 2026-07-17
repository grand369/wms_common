<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="任务单号">
        <el-input v-model="filters.taskNo" placeholder="请输入任务单号" clearable />
      </el-form-item>
      <el-form-item label="工位">
        <el-input v-model="filters.stationName" placeholder="请输入工位名称" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="待处理" :value="0" />
          <el-option label="配送中" :value="1" />
          <el-option label="已完成" :value="2" />
          <el-option label="已取消" :value="3" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>补料任务列表</span>
          <div class="header-actions">
            <WmsExportButton :export-api="exportData" filename="补料任务清单.xlsx" />
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
        <el-table-column prop="taskNo" label="任务单号" width="180" />
        <el-table-column prop="stationName" label="工位" show-overflow-tooltip />
        <el-table-column prop="materialCode" label="物料编码" width="160" />
        <el-table-column prop="qty" label="数量" align="right" width="100" />
        <el-table-column prop="status" label="状态" align="center" width="120">
          <template #default="{ row }">
            <el-tag :type="mapStatusType((row as ReplenishmentTaskDto).status)">
              {{ mapStatusText((row as ReplenishmentTaskDto).status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="160" fixed="right">
          <template #default="{ row }">
            <el-button
              link
              type="success"
              :disabled="(row as ReplenishmentTaskDto).status >= 2"
              :loading="completingId === (row as ReplenishmentTaskDto).id"
              @click="handleComplete(row as ReplenishmentTaskDto)"
            >
              完成
            </el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import { completeReplenishment } from '@/api/lineSide';
import type { ReplenishmentTaskDto } from '@/api/lineSide';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<ReplenishmentTaskDto>('/api/wms/line-side/replenishment');

const completingId = ref<string | null>(null);

function mapStatusType(status: number) {
  const map: Record<number, 'info' | 'warning' | 'success' | 'danger'> = {
    0: 'warning',
    1: 'info',
    2: 'success',
    3: 'danger',
  };
  return map[status] || 'info';
}

function mapStatusText(status: number) {
  const map: Record<number, string> = {
    0: '待处理',
    1: '配送中',
    2: '已完成',
    3: '已取消',
  };
  return map[status] || '未知';
}

async function handleComplete(row: ReplenishmentTaskDto) {
  try {
    await ElMessageBox.confirm(`确认完成补料任务 ${row.taskNo}？`, '提示', { type: 'warning' });
  } catch {
    return;
  }
  completingId.value = row.id;
  try {
    await completeReplenishment(row.id);
    ElMessage.success('已完成');
    handleSearch();
  } catch {
    ElMessage.error('操作失败');
  } finally {
    completingId.value = null;
  }
}

async function exportData() {
  return { fileUrl: '/api/wms/line-side/replenishment/export', rowCount: total.value };
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
