<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="工位编码">
        <el-input v-model="filters.code" placeholder="请输入工位编码" clearable />
      </el-form-item>
      <el-form-item label="工位名称">
        <el-input v-model="filters.name" placeholder="请输入工位名称" clearable />
      </el-form-item>
      <el-form-item label="产线">
        <el-input v-model="filters.productionLineName" placeholder="请输入产线" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="启用" :value="1" />
          <el-option label="停用" :value="0" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>线边仓工位列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建工位
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
        <el-table-column prop="code" label="工位编码" width="160" />
        <el-table-column prop="name" label="工位名称" show-overflow-tooltip />
        <el-table-column prop="productionLineName" label="所属产线" show-overflow-tooltip />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="{ row }">
            <el-tag :type="(row as LineSideStationDto).status === 1 ? 'success' : 'info'">
              {{ (row as LineSideStationDto).status === 1 ? '启用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as LineSideStationDto)">详情</el-button>
            <el-button link type="primary" @click="handleKanban(row as LineSideStationDto)">看板</el-button>
            <el-button link type="warning" @click="handleReplenish(row as LineSideStationDto)">补料</el-button>
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
import { useTable } from '@/hooks/useTable';
import type { LineSideStationDto } from '@/api/lineSide';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<LineSideStationDto>('/api/v1/line-side/warehouses'); 

function handleCreate() {
  ElMessage.info('请通过系统管理-工位定义创建');
}

function handleDetail(row: LineSideStationDto) {
  ElMessage.info(`工位详情 ${row.code}`);
}

function handleKanban(row: LineSideStationDto) {
  router.push(`/line-side/kanban/${row.id}`);
}

function handleReplenish(row: LineSideStationDto) {
  router.push(`/line-side/replenishment?stationId=${row.id}`);
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
