<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="入库单号">
        <el-input v-model="filters.orderNo" placeholder="请输入入库单号" clearable />
      </el-form-item>
      <el-form-item label="仓库">
        <el-input v-model="filters.warehouseName" placeholder="请输入仓库" clearable />
      </el-form-item>
      <el-form-item label="供应商">
        <el-input v-model="filters.supplierName" placeholder="请输入供应商" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="草稿" :value="0" />
          <el-option label="已确认" :value="1" />
          <el-option label="质检中" :value="2" />
          <el-option label="隔离" :value="3" />
          <el-option label="上架中" :value="4" />
          <el-option label="已完成" :value="5" />
          <el-option label="已关闭" :value="6" />
          <el-option label="已取消" :value="7" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>入库单列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建入库单
            </el-button>
            <WmsExportButton :export-api="exportOrders" filename="入库单清单.xlsx" />
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
        <el-table-column prop="inboundOrderNo" label="入库单号" />
        <el-table-column prop="inboundTypeName" label="入库类型" />
        <el-table-column prop="supplierName" label="供应商" show-overflow-tooltip />
        <el-table-column prop="warehouseCode" label="仓库" />
        <el-table-column prop="creationTime" label="创建时间" />
        <el-table-column prop="inboundStatusValue" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapDocumentStatus(row.inboundStatusValue)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as InboundOrderDto)">详情</el-button>
            <el-button link type="primary" :disabled="(row as InboundOrderDto).inboundStatusValue !== 0" @click="handleEdit(row as InboundOrderDto)">编辑</el-button>
            <el-button link type="success" :disabled="(row as InboundOrderDto).inboundStatusValue !== 0" @click="handleConfirm(row as InboundOrderDto)">确认</el-button>
            <el-button link type="danger" :disabled="(row as InboundOrderDto).inboundStatusValue > 1" @click="handleCancel(row as InboundOrderDto)">取消</el-button>
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
import { getInboundOrder, confirmInbound, cancelInbound } from '@/api/inbound';
import type { InboundOrderDto } from '@/api/inbound';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<InboundOrderDto>('/api/v1/inbound/orders');




function mapDocumentStatus(status: number) {
  const map: Record<number, string> = { 
    0: 'Draft', 
    1: 'Confirmed', 
    2: 'Inspecting', 
    3: 'Isolated', 
    4: 'Putaway', 
    5: 'Completed', 
    6: 'Closed', 
    7: 'Cancelled' 
  };
  return map[status] || 'Draft';
}

function handleCreate() {
  router.push('/inbound/create');
}

function handleEdit(row: InboundOrderDto) {
  router.push(`/inbound/create?id=${row.id}`);
}

function handleDetail(row: InboundOrderDto) {
  router.push(`/inbound/detail/${row.id}`);
}

async function handleConfirm(row: InboundOrderDto) {
  try {
    // 需要先获取订单详情来获取lines
    const order = await getInboundOrder(row.id);
    const confirmData = {
      idempotencyId: Date.now().toString(),
      lines: (order.lines || []).map(l => ({
        lineId: l.id || '',
        receivedQuantity: l.planQuantity,
      })),
    };
    await confirmInbound(row.id, confirmData);
    ElMessage.success('确认成功');
    handleSearch();
  } catch {
    ElMessage.error('确认失败');
  }
}

async function handleCancel(row: InboundOrderDto) {
  try {
    await cancelInbound(row.id);
    ElMessage.success('取消成功');
    handleSearch();
  } catch {
    ElMessage.error('取消失败');
  }
}

async function exportOrders() {
  return { fileUrl: '/api/wms/inbound/export', rowCount: total.value };
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
