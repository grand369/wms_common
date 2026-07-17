<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="领料单号">
        <el-input v-model="filters.requisitionNo" placeholder="请输入领料单号" clearable />
      </el-form-item>
      <el-form-item label="工单号">
        <el-input v-model="filters.workOrderNo" placeholder="请输入工单号" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="草稿" :value="0" />
          <el-option label="已下发" :value="1" />
          <el-option label="已完成" :value="2" />
          <el-option label="已取消" :value="3" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>领料单列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建领料单
            </el-button>
            <WmsExportButton :export-api="exportRequisitions" filename="领料单清单.xlsx" />
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
        <el-table-column prop="requisitionNo" label="领料单号" />
        <el-table-column prop="workOrderNo" label="工单号" />
        <el-table-column prop="productionLineName" label="产线" />
        <el-table-column prop="planDate" label="计划日期" />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapStatus(row.status)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as ProductionRequisitionDto)">详情</el-button>
            <el-button link type="success" :disabled="(row as ProductionRequisitionDto).status !== 0" @click="handleIssue(row as ProductionRequisitionDto)">下发</el-button>
            <el-button link type="danger" :disabled="(row as ProductionRequisitionDto).status !== 0" @click="handleCancel(row as ProductionRequisitionDto)">取消</el-button>
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
import { issueRequisition } from '@/api/production';
import type { ProductionRequisitionDto } from '@/api/production';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<ProductionRequisitionDto>('/api/v1/production/requisitions');

function mapStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'Issued', 2: 'Completed', 3: 'Cancelled' };
  return map[status] || 'Draft';
}

function handleCreate() {
  router.push('/production/requisition/create');
}

function handleDetail(row: ProductionRequisitionDto) {
  router.push(`/production/requisition/detail/${row.id}`);
}

async function handleIssue(row: ProductionRequisitionDto) {
  try {
    await issueRequisition(row.id);
    ElMessage.success('下发成功');
    handleSearch();
  } catch {
    ElMessage.error('下发失败');
  }
}

async function handleCancel(row: ProductionRequisitionDto) {
  ElMessage.warning('取消功能待实现');
}

async function exportRequisitions() {
  return { fileUrl: '/api/wms/production/requisition/export', rowCount: total.value };
}

handleSearch();
</script>

<style scoped lang="scss">
.page-container { padding: 0; }
.card-header { display: flex; justify-content: space-between; align-items: center; }
.header-actions { display: flex; gap: 8px; }
</style>
