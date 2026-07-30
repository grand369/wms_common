<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="仓库">
        <WmsWarehouseSelector v-model="filters.warehouseId" placeholder="请选择仓库" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="待确认" :value="0" />
          <el-option label="已完成" :value="1" />
        </el-select>
      </el-form-item>
      <el-form-item label="关键字">
        <el-input v-model="filters.keyword" placeholder="快照编号/仓库编码" clearable />
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>库存快照</span>
          <div class="header-actions">
            <WmsWarehouseSelector v-model="createWarehouseId" placeholder="选择仓库生成快照" />
            <el-button type="primary" @click="handleCreateSnapshot">
              <el-icon><Plus /></el-icon> 生成快照
            </el-button>
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
        <el-table-column prop="snapshotNo" label="快照编号" width="180" />
        <el-table-column prop="warehouseCode" label="仓库编码" width="120" />
        <el-table-column prop="snapshotTime" label="快照时间" width="180" />
        <el-table-column prop="totalQty" label="总数量" align="right" width="120">
          <template #default="{ row }">
            <span class="wms-number">{{ row.totalQty.toFixed(2) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="totalFrozenQty" label="冻结数量" align="right" width="120">
          <template #default="{ row }">
            <span class="wms-number status-frozen">{{ row.totalFrozenQty.toFixed(2) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="totalAvailableQty" label="可用数量" align="right" width="120">
          <template #default="{ row }">
            <span class="wms-number">{{ row.totalAvailableQty.toFixed(2) }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'warning' : 'success'">{{ row.status === 0 ? '待确认' : '已完成' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as InventorySnapshotDto)">查看</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      title="快照详情"
      :visible="detailVisible"
      show-footer
      width="500px"
      @close="detailVisible = false"
      @cancel="detailVisible = false"
      @confirm="detailVisible = false"
    >
      <el-descriptions :column="1" border v-loading="detailLoading">
        <el-descriptions-item label="快照编号">{{ currentSnapshot?.snapshotNo }}</el-descriptions-item>
        <el-descriptions-item label="仓库编码">{{ currentSnapshot?.warehouseCode }}</el-descriptions-item>
        <el-descriptions-item label="快照时间">{{ currentSnapshot?.snapshotTime }}</el-descriptions-item>
        <el-descriptions-item label="总数量">
          <span class="wms-number">{{ currentSnapshot?.totalQty?.toFixed(2) }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="冻结数量">
          <span class="wms-number status-frozen">{{ currentSnapshot?.totalFrozenQty?.toFixed(2) }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="可用数量">
          <span class="wms-number">{{ currentSnapshot?.totalAvailableQty?.toFixed(2) }}</span>
        </el-descriptions-item>
        <el-descriptions-item label="状态">{{ currentSnapshot?.status === 0 ? '待确认' : '已完成' }}</el-descriptions-item>
      </el-descriptions>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage } from 'element-plus';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsWarehouseSelector from '@/components/common/WmsWarehouseSelector.vue';
import { useTable } from '@/hooks/useTable';
import { getSnapshot, createSnapshot } from '@/api/inventory';
import type { InventorySnapshotDto } from '@/api/inventory';
import { getFriendlyErrorMessage, parseAxiosError } from '@/utils/errorHandler';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<InventorySnapshotDto>('/api/v1/inventory/snapshots');

const createWarehouseId = ref('');
const currentSnapshot = ref<InventorySnapshotDto | null>(null);
const detailVisible = ref(false);
const detailLoading = ref(false);

async function handleCreateSnapshot() {
  if (!createWarehouseId.value) {
    ElMessage.warning('请选择仓库');
    return;
  }
  try {
    await createSnapshot({ warehouseId: createWarehouseId.value });
    ElMessage.success('快照生成成功');
    createWarehouseId.value = '';
    handleSearch();
  } catch (err) {
    const friendlyMsg = getFriendlyErrorMessage(parseAxiosError(err))
    ElMessage.error(friendlyMsg);
  }
}

async function handleDetail(row: InventorySnapshotDto) {
  currentSnapshot.value = row;
  detailVisible.value = true;
  detailLoading.value = true;
  try {
    currentSnapshot.value = await getSnapshot(row.id);
  } catch (err) {
    const friendlyMsg = getFriendlyErrorMessage(parseAxiosError(err))
    ElMessage.error(friendlyMsg);
  } finally {
    detailLoading.value = false;
  }
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
  align-items: center;
}
</style>
