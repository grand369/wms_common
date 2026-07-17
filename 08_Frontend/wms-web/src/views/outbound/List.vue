<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="出库单号">
        <el-input v-model="filters.orderNo" placeholder="请输入出库单号" clearable />
      </el-form-item>
      <el-form-item label="仓库">
        <el-input v-model="filters.warehouseName" placeholder="请输入仓库" clearable />
      </el-form-item>
      <el-form-item label="客户">
        <el-input v-model="filters.customerName" placeholder="请输入客户" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="草稿" :value="0" />
          <el-option label="已确认" :value="1" />
          <el-option label="进行中" :value="2" />
          <el-option label="已完成" :value="3" />
          <el-option label="已取消" :value="4" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>出库单列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建出库单
            </el-button>
            <WmsExportButton :export-api="exportOrders" filename="出库单清单.xlsx" />
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
        <el-table-column prop="orderNo" label="出库单号" />
        <el-table-column prop="orderType" label="出库类型" />
        <el-table-column prop="customerName" label="客户" show-overflow-tooltip />
        <el-table-column prop="warehouseName" label="仓库" />
        <el-table-column prop="planDate" label="计划日期" />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapDocumentStatus(row.status)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as OutboundOrderDto)">详情</el-button>
            <el-button link type="primary" :disabled="(row as OutboundOrderDto).status !== 0" @click="handleEdit(row as OutboundOrderDto)">编辑</el-button>
            <el-button link type="success" :disabled="(row as OutboundOrderDto).status !== 0" @click="handleConfirm(row as OutboundOrderDto)">确认</el-button>
            <el-button link type="danger" :disabled="(row as OutboundOrderDto).status > 2" @click="handleCancel(row as OutboundOrderDto)">取消</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { Plus } from '@element-plus/icons-vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import { allocateOutbound, cancelOutbound } from '@/api/outbound';
import type { OutboundOrderDto } from '@/api/outbound';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<OutboundOrderDto>('/api/v1/outbound/orders');

function mapDocumentStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'Confirmed', 2: 'InProgress', 3: 'Completed', 4: 'Cancelled' };
  return map[status] || 'Draft';
}

function handleCreate() {
  router.push('/outbound/create');
}

function handleEdit(row: OutboundOrderDto) {
  router.push(`/outbound/create?id=${row.id}`);
}

function handleDetail(row: OutboundOrderDto) {
  router.push(`/outbound/detail/${row.id}`);
}

async function handleConfirm(row: OutboundOrderDto) {
  try {
    await allocateOutbound(row.id);
    ElMessage.success('确认并分配成功');
    handleSearch();
  } catch {
    ElMessage.error('确认失败');
  }
}

async function handleCancel(row: OutboundOrderDto) {
  try {
    await cancelOutbound(row.id);
    ElMessage.success('取消成功');
    handleSearch();
  } catch {
    ElMessage.error('取消失败');
  }
}

async function exportOrders() {
  return { fileUrl: '/api/wms/outbound/export', rowCount: total.value };
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
