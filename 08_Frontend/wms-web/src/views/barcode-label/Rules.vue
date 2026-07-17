<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="规则编码">
        <el-input v-model="filters.code" placeholder="请输入规则编码" clearable />
      </el-form-item>
      <el-form-item label="规则名称">
        <el-input v-model="filters.name" placeholder="请输入规则名称" clearable />
      </el-form-item>
      <el-form-item label="规则类型">
        <el-input v-model="filters.ruleType" placeholder="请输入规则类型" clearable />
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>条码规则列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建规则
            </el-button>
            <WmsExportButton :export-api="exportRules" filename="条码规则清单.xlsx" />
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
        <el-table-column prop="code" label="规则编码" />
        <el-table-column prop="name" label="规则名称" />
        <el-table-column prop="ruleType" label="规则类型" />
        <el-table-column prop="pattern" label="规则样式" show-overflow-tooltip />
        <el-table-column prop="prefix" label="前缀" />
        <el-table-column label="启停" align="center" width="80">
          <template #default="{ row }">
            <el-switch
              :model-value="(row as BarcodeRuleDto).status === 1"
              @change="(val: boolean) => handleToggle(row as BarcodeRuleDto, val)"
            />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row as BarcodeRuleDto)">编辑</el-button>
            <el-button link type="danger" @click="handleDelete(row as BarcodeRuleDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { Plus } from '@element-plus/icons-vue';
import { useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import { updateRule, deleteRule } from '@/api/barcodeLabel';
import type { BarcodeRuleDto } from '@/api/barcodeLabel';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<BarcodeRuleDto>('/api/v1/barcode-label/rules');

async function handleToggle(row: BarcodeRuleDto, val: boolean) {
  try {
    await updateRule(row.id, { ...row, status: val ? 1 : 0 });
    ElMessage.success(val ? '已启用' : '已禁用');
    handleSearch();
  } catch {
    ElMessage.error('操作失败');
  }
}

function handleCreate() {
  router.push('/barcode-label/rule/create');
}

function handleEdit(row: BarcodeRuleDto) {
  router.push(`/barcode-label/rule/create?id=${row.id}`);
}

async function handleDelete(row: BarcodeRuleDto) {
  try {
    await ElMessageBox.confirm('确定要删除该规则吗？', '提示', { type: 'warning' });
    await deleteRule(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // cancelled
  }
}

async function exportRules() {
  return { fileUrl: '/api/wms/barcode-label/rule/export', rowCount: total.value };
}

handleSearch();
</script>

<style scoped lang="scss">
.page-container { padding: 0; }
.card-header { display: flex; justify-content: space-between; align-items: center; }
.header-actions { display: flex; gap: 8px; }
</style>
