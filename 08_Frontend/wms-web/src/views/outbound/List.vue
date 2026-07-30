<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="出库单号">
        <el-input v-model="filters.outboundOrderNo" placeholder="请输入出库单号" clearable />
      </el-form-item>
      <el-form-item label="仓库">
        <el-input v-model="filters.warehouseCode" placeholder="请输入仓库编码" clearable />
      </el-form-item>
      <el-form-item label="类型">
        <el-select v-model="filters.outboundTypeValue" placeholder="请选择类型" clearable>
          <el-option label="生产出库" :value="1" />
          <el-option label="销售出库" :value="2" />
          <el-option label="退货出库" :value="3" />
          <el-option label="调拨出库" :value="4" />
        </el-select>
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.outboundStatusValue" placeholder="请选择状态" clearable>
          <el-option label="草稿" :value="0" />
          <el-option label="已分配" :value="1" />
          <el-option label="拣货中" :value="2" />
          <el-option label="发货中" :value="3" />
          <el-option label="已完成" :value="4" />
          <el-option label="已取消" :value="5" />
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
        <el-table-column prop="outboundOrderNo" label="出库单号" />
        <el-table-column prop="outboundTypeName" label="出库类型" />
        <el-table-column prop="warehouseCode" label="仓库编码" />
        <el-table-column prop="totalRequiredQuantity" label="需求数量" align="right" />
        <el-table-column prop="totalAllocatedQuantity" label="已分配" align="right" />
        <el-table-column prop="totalPickedQuantity" label="已拣货" align="right" />
        <el-table-column prop="totalShippedQuantity" label="已发货" align="right" />
        <el-table-column prop="outboundStatusName" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="row.outboundStatusName" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as OutboundOrderDto)">详情</el-button>
            <el-button link type="primary" :disabled="(row as OutboundOrderDto).outboundStatusValue !== 0" @click="handleEdit(row as OutboundOrderDto)">编辑</el-button>
            <el-button link type="success" :disabled="(row as OutboundOrderDto).outboundStatusValue !== 0" @click="handleAllocate(row as OutboundOrderDto)">分配</el-button>
            <el-button link type="danger" :disabled="(row as OutboundOrderDto).outboundStatusValue > 2" @click="handleCancel(row as OutboundOrderDto)">取消</el-button>
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
import { allocateOutbound, cancelOutbound, getOutboundOrder } from '@/api/outbound';
import type { OutboundOrderDto, OutboundAllocateCommandDto } from '@/api/outbound';
import { getFriendlyErrorMessage, parseAxiosError } from '@/utils/errorHandler';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<OutboundOrderDto>('/api/v1/outbound/orders');

function handleCreate() {
  router.push('/outbound/create');
}

function handleEdit(row: OutboundOrderDto) {
  router.push(`/outbound/create?id=${row.id}`);
}

function handleDetail(row: OutboundOrderDto) {
  router.push(`/outbound/detail/${row.id}`);
}

async function handleAllocate(row: OutboundOrderDto) {
  try {
    const orderDetail = await getOutboundOrder(row.id);
    if (!orderDetail.lines || orderDetail.lines.length === 0) {
      ElMessage.warning('出库单明细为空，无法分配');
      return;
    }
    const command: OutboundAllocateCommandDto = {
      idempotencyId: `alloc_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
      lines: orderDetail.lines.map((l) => ({
        lineId: l.id,
        allocatedQuantity: l.requiredQuantity,
      })),
    };
    await allocateOutbound(row.id, command);
    ElMessage.success('分配成功');
    handleSearch();
  } catch (err) {
    const friendlyMsg = getFriendlyErrorMessage(parseAxiosError(err))
    ElMessage.error(friendlyMsg);
  }
}

async function handleCancel(row: OutboundOrderDto) {
  try {
    await cancelOutbound(row.id);
    ElMessage.success('取消成功');
    handleSearch();
  } catch (err) {
    const friendlyMsg = getFriendlyErrorMessage(parseAxiosError(err))
    ElMessage.error(friendlyMsg);
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
