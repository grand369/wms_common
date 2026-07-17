<template>
  <div class="page-container">
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
        <el-table-column prop="snapshotNo" label="快照编号" />
        <el-table-column prop="warehouseId" label="仓库" />
        <el-table-column prop="snapshotTime" label="快照时间" />
        <el-table-column prop="totalQty" label="总数量" align="right" />
        <el-table-column prop="status" label="状态" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 0 ? 'warning' : 'success'">{{ row.status === 0 ? '待确认' : '已完成' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="120" fixed="right">
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
        <el-descriptions-item label="仓库">{{ currentSnapshot?.warehouseId }}</el-descriptions-item>
        <el-descriptions-item label="快照时间">{{ currentSnapshot?.snapshotTime }}</el-descriptions-item>
        <el-descriptions-item label="总数量">{{ currentSnapshot?.totalQty }}</el-descriptions-item>
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
import WmsWarehouseSelector from '@/components/common/WmsWarehouseSelector.vue';
import { useTable } from '@/hooks/useTable';
import { createSnapshot, getSnapshot } from '@/api/inventory';
import type { InventorySnapshotDto } from '@/api/inventory';

const { loading, tableData, total, pagination, handlePageChange, handleSizeChange, handleSearch } =
  useTable<InventorySnapshotDto>('/api/v1/inventory/snapshot');

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
  } catch {
    ElMessage.error('快照生成失败');
  }
}

async function handleDetail(row: InventorySnapshotDto) {
  currentSnapshot.value = row;
  detailVisible.value = true;
  detailLoading.value = true;
  try {
    currentSnapshot.value = await getSnapshot(row.id);
  } catch {
    ElMessage.error('加载快照详情失败');
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
