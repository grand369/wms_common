<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="标题">
        <el-input v-model="filters.title" placeholder="请输入标题" clearable />
      </el-form-item>
      <el-form-item label="通知类型">
        <el-input v-model="filters.notificationType" placeholder="请输入通知类型" clearable />
      </el-form-item>
      <el-form-item label="已读状态">
        <el-select v-model="filters.isRead" placeholder="请选择状态" clearable>
          <el-option label="未读" :value="false" />
          <el-option label="已读" :value="true" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>通知列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleMarkAllRead">全部已读</el-button>
            <WmsExportButton :export-api="exportLogs" filename="通知日志清单.xlsx" />
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
        <el-table-column prop="title" label="标题" show-overflow-tooltip />
        <el-table-column prop="notificationType" label="通知类型" />
        <el-table-column prop="receiverName" label="接收人" />
        <el-table-column label="已读状态" align="center" width="90">
          <template #default="{ row }">
            <el-tag :type="(row as NotificationLogDto).isRead ? 'success' : 'info'" effect="light">
              {{ (row as NotificationLogDto).isRead ? '已读' : '未读' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="readTime" label="阅读时间" />
        <el-table-column prop="creationTime" label="创建时间" />
        <el-table-column label="操作" width="120" fixed="right">
          <template #default="{ row }">
            <el-button
              v-if="!(row as NotificationLogDto).isRead"
              link
              type="primary"
              :loading="readingId === (row as NotificationLogDto).id"
              @click="handleMarkRead(row as NotificationLogDto)"
            >
              标记已读
            </el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import { markAsRead, markAllAsRead } from '@/api/notification';
import type { NotificationLogDto } from '@/api/notification';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<NotificationLogDto>('/api/v1/notification/logs');

const readingId = ref('');

async function handleMarkRead(row: NotificationLogDto) {
  readingId.value = row.id;
  try {
    await markAsRead(row.id);
    ElMessage.success('已标记为已读');
    handleSearch();
  } catch {
    ElMessage.error('操作失败');
  } finally {
    readingId.value = '';
  }
}

async function handleMarkAllRead() {
  try {
    await markAllAsRead();
    ElMessage.success('全部标记已读');
    handleSearch();
  } catch {
    ElMessage.error('操作失败');
  }
}

async function exportLogs() {
  return { fileUrl: '/api/wms/notification/log/export', rowCount: total.value };
}

handleSearch();
</script>

<style scoped lang="scss">
.page-container { padding: 0; }
.card-header { display: flex; justify-content: space-between; align-items: center; }
.header-actions { display: flex; gap: 8px; }
</style>
