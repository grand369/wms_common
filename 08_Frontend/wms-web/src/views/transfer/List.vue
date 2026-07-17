<template>
  <div class="page-container">
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
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="草稿" :value="0" />
          <el-option label="待审批" :value="1" />
          <el-option label="审批通过" :value="2" />
          <el-option label="源仓出库" :value="3" />
          <el-option label="在途" :value="4" />
          <el-option label="目标仓入库" :value="5" />
          <el-option label="已完成" :value="6" />
          <el-option label="已取消" :value="7" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>调拨单列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建调拨单
            </el-button>
            <WmsExportButton :export-api="exportData" filename="调拨单清单.xlsx" />
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
        <el-table-column prop="transferNo" label="调拨单号" />
        <el-table-column prop="fromWarehouseName" label="源仓库" show-overflow-tooltip />
        <el-table-column prop="toWarehouseName" label="目标仓库" show-overflow-tooltip />
        <el-table-column prop="planDate" label="计划日期" width="120" />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapStatus(row.status)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as TransferDto)">详情</el-button>
            <el-button
              link
              type="primary"
              :disabled="(row as TransferDto).status !== 0"
              @click="handleEdit(row as TransferDto)"
            >
              编辑
            </el-button>
            <el-button
              link
              type="success"
              :disabled="(row as TransferDto).status !== 1"
              @click="handleApprove(row as TransferDto)"
            >
              审批
            </el-button>
            <el-button
              link
              type="danger"
              :disabled="(row as TransferDto).status >= 6"
              @click="handleCancel(row as TransferDto)"
            >
              取消
            </el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { Plus } from '@element-plus/icons-vue';
import { useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import { approveTransfer, cancelTransfer } from '@/api/transfer';
import type { TransferDto } from '@/api/transfer';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<TransferDto>('/api/v1/transfer/orders');

function mapStatus(status: number) {
  const map: Record<number, string> = {
    0: 'Draft',
    1: 'Confirmed',
    2: 'Approved',
    3: 'InProgress',
    4: 'InProgress',
    5: 'InProgress',
    6: 'Completed',
    7: 'Cancelled',
  };
  return map[status] || 'Draft';
}

function handleCreate() {
  router.push('/transfer/create');
}

function handleEdit(row: TransferDto) {
  router.push(`/transfer/create?id=${row.id}`);
}

function handleDetail(row: TransferDto) {
  router.push(`/transfer/detail/${row.id}`);
}

async function handleApprove(row: TransferDto) {
  try {
    await ElMessageBox.confirm(`确认审批调拨单 ${row.transferNo}？`, '审批', { type: 'warning' });
    await approveTransfer(row.id);
    ElMessage.success('审批成功');
    handleSearch();
  } catch (e: any) {
    if (e !== 'cancel') {
      ElMessage.error('审批失败');
    }
  }
}

async function handleCancel(row: TransferDto) {
  try {
    await ElMessageBox.confirm(`确认取消调拨单 ${row.transferNo}？`, '提示', { type: 'warning' });
    await cancelTransfer(row.id, { reason: '手动取消' });
    ElMessage.success('取消成功');
    handleSearch();
  } catch (e: any) {
    if (e !== 'cancel') {
      ElMessage.error('取消失败');
    }
  }
}

async function exportData() {
  return { fileUrl: '/api/wms/transfer/export', rowCount: total.value };
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
