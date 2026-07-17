<template>
  <div class="page-container">
    <el-tabs v-model="activeTab" type="border-card">
      <el-tab-pane label="通知规则" name="rules">
        <WmsSearch @search="ruleTable.handleSearch" @reset="ruleTable.resetFilters">
          <el-form-item label="规则编码">
            <el-input v-model="ruleTable.filters.code" placeholder="请输入规则编码" clearable />
          </el-form-item>
          <el-form-item label="规则名称">
            <el-input v-model="ruleTable.filters.name" placeholder="请输入规则名称" clearable />
          </el-form-item>
          <el-form-item label="事件类型">
            <el-input v-model="ruleTable.filters.eventType" placeholder="请输入事件类型" clearable />
          </el-form-item>
        </WmsSearch>

        <div class="tab-header">
          <div />
          <div class="header-actions">
            <el-button type="primary" @click="handleCreateRule">
              <el-icon><Plus /></el-icon> 新建规则
            </el-button>
            <WmsExportButton :export-api="exportRules" filename="通知规则清单.xlsx" />
          </div>
        </div>

        <WmsTable
          :data="ruleTable.tableData"
          :loading="ruleTable.loading"
          :total="ruleTable.total"
          v-model:current-page="ruleTable.pagination.currentPage"
          v-model:page-size="ruleTable.pagination.pageSize"
          :page-sizes="ruleTable.pagination.pageSizes"
          @page-change="ruleTable.handlePageChange"
          @size-change="ruleTable.handleSizeChange"
        >
          <el-table-column prop="code" label="规则编码" />
          <el-table-column prop="name" label="规则名称" />
          <el-table-column prop="eventType" label="事件类型" />
          <el-table-column prop="channelType" label="发送渠道" />
          <el-table-column label="启用状态" align="center" width="80">
            <template #default="{ row }">
              <el-switch
                :model-value="(row as NotificationRuleDto).isEnabled"
                @change="(val: boolean) => handleToggleRule(row as NotificationRuleDto, val)"
              />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="180" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleEditRule(row as NotificationRuleDto)">编辑</el-button>
              <el-button link type="danger" @click="handleDeleteRule(row as NotificationRuleDto)">删除</el-button>
            </template>
          </el-table-column>
        </WmsTable>
      </el-tab-pane>

      <el-tab-pane label="模板管理" name="templates">
        <WmsSearch @search="templateTable.handleSearch" @reset="templateTable.resetFilters">
          <el-form-item label="模板编码">
            <el-input v-model="templateTable.filters.code" placeholder="请输入模板编码" clearable />
          </el-form-item>
          <el-form-item label="模板名称">
            <el-input v-model="templateTable.filters.name" placeholder="请输入模板名称" clearable />
          </el-form-item>
          <el-form-item label="发送渠道">
            <el-input v-model="templateTable.filters.channelType" placeholder="请输入发送渠道" clearable />
          </el-form-item>
        </WmsSearch>

        <div class="tab-header">
          <div />
          <div class="header-actions">
            <el-button type="primary" @click="handleCreateTemplate">
              <el-icon><Plus /></el-icon> 新建模板
            </el-button>
            <WmsExportButton :export-api="exportTemplates" filename="通知模板清单.xlsx" />
          </div>
        </div>

        <WmsTable
          :data="templateTable.tableData"
          :loading="templateTable.loading"
          :total="templateTable.total"
          v-model:current-page="templateTable.pagination.currentPage"
          v-model:page-size="templateTable.pagination.pageSize"
          :page-sizes="templateTable.pagination.pageSizes"
          @page-change="templateTable.handlePageChange"
          @size-change="templateTable.handleSizeChange"
        >
          <el-table-column prop="code" label="模板编码" />
          <el-table-column prop="name" label="模板名称" />
          <el-table-column prop="channelType" label="发送渠道" />
          <el-table-column prop="subject" label="主题" show-overflow-tooltip />
          <el-table-column label="操作" width="180" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleEditTemplate(row as NotificationTemplateDto)">编辑</el-button>
              <el-button link type="danger" @click="handleDeleteTemplate(row as NotificationTemplateDto)">删除</el-button>
            </template>
          </el-table-column>
        </WmsTable>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Plus } from '@element-plus/icons-vue';
import { useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import {
  updateNotificationRule,
  deleteNotificationRule,
  updateNotificationTemplate,
  deleteNotificationTemplate,
} from '@/api/notification';
import type { NotificationRuleDto, NotificationTemplateDto } from '@/api/notification';

const router = useRouter();
const activeTab = ref('rules');

const ruleTable = useTable<NotificationRuleDto>('/api/wms/notification/rule');
const templateTable = useTable<NotificationTemplateDto>('/api/wms/notification/template');

async function handleToggleRule(row: NotificationRuleDto, val: boolean) {
  try {
    await updateNotificationRule(row.id, { ...row, isEnabled: val });
    ElMessage.success(val ? '已启用' : '已禁用');
    ruleTable.handleSearch();
  } catch {
    ElMessage.error('操作失败');
  }
}

function handleCreateRule() {
  router.push('/notification/rule/create');
}

function handleEditRule(row: NotificationRuleDto) {
  router.push(`/notification/rule/create?id=${row.id}`);
}

async function handleDeleteRule(row: NotificationRuleDto) {
  try {
    await ElMessageBox.confirm('确定要删除该规则吗？', '提示', { type: 'warning' });
    await deleteNotificationRule(row.id);
    ElMessage.success('删除成功');
    ruleTable.handleSearch();
  } catch {
    // cancelled
  }
}

function handleCreateTemplate() {
  router.push('/notification/template/create');
}

function handleEditTemplate(row: NotificationTemplateDto) {
  router.push(`/notification/template/create?id=${row.id}`);
}

async function handleDeleteTemplate(row: NotificationTemplateDto) {
  try {
    await ElMessageBox.confirm('确定要删除该模板吗？', '提示', { type: 'warning' });
    await deleteNotificationTemplate(row.id);
    ElMessage.success('删除成功');
    templateTable.handleSearch();
  } catch {
    // cancelled
  }
}

async function exportRules() {
  return { fileUrl: '/api/wms/notification/rule/export', rowCount: ruleTable.total.value };
}

async function exportTemplates() {
  return { fileUrl: '/api/wms/notification/template/export', rowCount: templateTable.total.value };
}

onMounted(() => {
  ruleTable.handleSearch();
});
</script>

<style scoped lang="scss">
.page-container { padding: 0; }
.tab-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.header-actions { display: flex; gap: 8px; }
</style>
