<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="模板编码">
        <el-input v-model="filters.code" placeholder="请输入模板编码" clearable />
      </el-form-item>
      <el-form-item label="模板名称">
        <el-input v-model="filters.name" placeholder="请输入模板名称" clearable />
      </el-form-item>
      <el-form-item label="模板类型">
        <el-input v-model="filters.templateType" placeholder="请输入模板类型" clearable />
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>标签模板列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建模板
            </el-button>
            <WmsExportButton :export-api="exportTemplates" filename="标签模板清单.xlsx" />
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
        <el-table-column prop="code" label="模板编码" />
        <el-table-column prop="name" label="模板名称" />
        <el-table-column prop="templateType" label="模板类型" />
        <el-table-column prop="width" label="宽度(mm)" />
        <el-table-column prop="height" label="高度(mm)" />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapStatus(row.status)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row as LabelTemplateDto)">编辑内容</el-button>
            <el-button link type="primary" @click="handleDetail(row as LabelTemplateDto)">详情</el-button>
            <el-button link type="danger" @click="handleDelete(row as LabelTemplateDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog :visible="dialogVisible" title="编辑模板内容" width="800px" @close="dialogVisible = false" @confirm="handleSaveContent">
      <el-input
        v-model="editingContent"
        type="textarea"
        :rows="15"
        placeholder="请输入JSON模板内容"
      />
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { Plus } from '@element-plus/icons-vue';
import { useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { useTable } from '@/hooks/useTable';
import { updateTemplate, deleteTemplate } from '@/api/barcodeLabel';
import type { LabelTemplateDto } from '@/api/barcodeLabel';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<LabelTemplateDto>('/api/v1/barcode-label/templates');

const dialogVisible = ref(false);
const editingId = ref('');
const editingContent = ref('');
let editingRow: LabelTemplateDto | null = null;

function mapStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'Active', 2: 'Completed' };
  return map[status] || 'Draft';
}

function handleCreate() {
  router.push('/barcode-label/template/create');
}

function handleDetail(row: LabelTemplateDto) {
  router.push(`/barcode-label/template/detail/${row.id}`);
}

function handleEdit(row: LabelTemplateDto) {
  editingRow = row;
  editingId.value = row.id;
  editingContent.value = row.content;
  dialogVisible.value = true;
}

async function handleSaveContent() {
  if (!editingRow) return;
  try {
    await updateTemplate(editingId.value, { ...editingRow, content: editingContent.value });
    ElMessage.success('保存成功');
    dialogVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('保存失败');
  }
}

async function handleDelete(row: LabelTemplateDto) {
  try {
    await ElMessageBox.confirm('确定要删除该模板吗？', '提示', { type: 'warning' });
    await deleteTemplate(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // cancelled
  }
}

async function exportTemplates() {
  return { fileUrl: '/api/v1/barcode-label/templates/export', rowCount: total.value };
}

handleSearch();
</script>

<style scoped lang="scss">
.page-container { padding: 0; }
.card-header { display: flex; justify-content: space-between; align-items: center; }
.header-actions { display: flex; gap: 8px; }
</style>
