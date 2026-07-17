<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="委外单号">
        <el-input v-model="filters.orderNo" placeholder="请输入委外单号" clearable />
      </el-form-item>
      <el-form-item label="供应商">
        <el-input v-model="filters.supplierName" placeholder="请输入供应商" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="草稿" :value="0" />
          <el-option label="已发送" :value="1" />
          <el-option label="部分回收" :value="2" />
          <el-option label="已完成" :value="3" />
          <el-option label="超期" :value="4" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>委外订单列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建委外单
            </el-button>
            <WmsExportButton :export-api="exportOrders" filename="委外订单清单.xlsx" />
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
        <el-table-column prop="orderNo" label="委外单号" />
        <el-table-column prop="supplierName" label="供应商" show-overflow-tooltip />
        <el-table-column prop="planDate" label="计划日期" />
        <el-table-column label="状态" align="center" width="120">
          <template #default="{ row }">
            <el-tag v-if="(row as SubcontractOrderDto).status === 4" type="danger" effect="light">超期</el-tag>
            <WmsStatusTag v-else :status="mapStatus((row as SubcontractOrderDto).status)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as SubcontractOrderDto)">详情</el-button>
            <el-button link type="success" :disabled="(row as SubcontractOrderDto).status !== 0" @click="handleEdit(row as SubcontractOrderDto)">编辑</el-button>
            <el-button link type="danger" :disabled="(row as SubcontractOrderDto).status >= 3" @click="handleCancel(row as SubcontractOrderDto)">取消</el-button>
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
import type { SubcontractOrderDto } from '@/api/production';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<SubcontractOrderDto>('/api/v1/production/subcontract');

function mapStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'Sent', 2: 'PartialReturn', 3: 'Completed', 4: 'Overdue' };
  return map[status] || 'Draft';
}

function handleCreate() {
  router.push('/production/subcontract/create');
}

function handleDetail(row: SubcontractOrderDto) {
  router.push(`/production/subcontract/detail/${row.id}`);
}

function handleEdit(row: SubcontractOrderDto) {
  router.push(`/production/subcontract/create?id=${row.id}`);
}

async function handleCancel(row: SubcontractOrderDto) {
  ElMessage.warning('取消功能待实现');
}

async function exportOrders() {
  return { fileUrl: '/api/wms/production/subcontract/export', rowCount: total.value };
}

handleSearch();
</script>

<style scoped lang="scss">
.page-container { padding: 0; }
.card-header { display: flex; justify-content: space-between; align-items: center; }
.header-actions { display: flex; gap: 8px; }
</style>
