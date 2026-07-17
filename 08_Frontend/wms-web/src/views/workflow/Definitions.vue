<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="编码">
        <el-input v-model="filters.code" placeholder="请输入编码" clearable />
      </el-form-item>
      <el-form-item label="名称">
        <el-input v-model="filters.name" placeholder="请输入名称" clearable />
      </el-form-item>
      <el-form-item label="业务类型">
        <el-input v-model="filters.entityType" placeholder="请输入业务类型" clearable />
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>审批流定义</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建定义
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
        <el-table-column prop="code" label="编码" min-width="140" />
        <el-table-column prop="name" label="名称" min-width="160" show-overflow-tooltip />
        <el-table-column prop="entityType" label="业务类型" width="120" />
        <el-table-column prop="version" label="版本" width="80" align="center">
          <template #default="{ row }">
            <el-tag size="small">v{{ row.version }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="isPublished" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag
              :type="row.isPublished ? 'success' : 'info'"
              size="small"
              effect="dark"
            >
              {{ row.isPublished ? '已发布' : '草稿' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="280" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row as WorkflowDefinitionDto)">
              编辑
            </el-button>
            <el-button link type="success" :disabled="row.isPublished" @click="handlePublish(row as WorkflowDefinitionDto)">
              发布
            </el-button>
            <el-button link type="danger" @click="handleDelete(row as WorkflowDefinitionDto)">
              删除
            </el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <!-- Edit Dialog -->
    <WmsDialog
      :visible="dialogVisible"
      :title="isEdit ? '编辑审批流' : '新建审批流'"
      width="700px"
      :show-footer="false"
      @close="handleDialogClose"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-form-item label="编码" prop="code">
          <el-input v-model="formData.code" placeholder="请输入编码" />
        </el-form-item>
        <el-form-item label="名称" prop="name">
          <el-input v-model="formData.name" placeholder="请输入名称" />
        </el-form-item>
        <el-form-item label="业务类型" prop="entityType">
          <el-input v-model="formData.entityType" placeholder="如: Inbound, Outbound" />
        </el-form-item>
      </el-form>

      <el-divider content-position="left">审批节点配置</el-divider>
      <NodeTableEditor v-model="editingNodes" />

      <div class="dialog-footer">
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" :loading="saveLoading" @click="handleSave">
          保存
        </el-button>
      </div>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus';
import { Plus } from '@element-plus/icons-vue';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import NodeTableEditor from '@/components/common/NodeTableEditor.vue';
import { useTable } from '@/hooks/useTable';
import {
  getApprovalFlowDefinitions,
  getApprovalFlowDefinition,
  createDefinition,
  updateDefinition,
  deleteDefinition,
  publishDefinition,
  type WorkflowDefinitionDto,
  type WorkflowNodeDto,
} from '@/api/workflow';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<WorkflowDefinitionDto>('/api/v1/workflow/definitions');

// ── Dialog State ─────────────────────────────────────────────
const dialogVisible = ref(false);
const isEdit = ref(false);
const saveLoading = ref(false);
const editingId = ref('');
const formRef = ref<FormInstance>();
const editingNodes = ref<WorkflowNodeDto[]>([]);

const formData = ref({
  code: '',
  name: '',
  entityType: '',
});

const formRules: FormRules = {
  code: [{ required: true, message: '请输入编码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入名称', trigger: 'blur' }],
  entityType: [{ required: true, message: '请输入业务类型', trigger: 'blur' }],
};

// ── CRUD Operations ──────────────────────────────────────────
function handleCreate() {
  isEdit.value = false;
  editingId.value = '';
  formData.value = { code: '', name: '', entityType: '' };
  editingNodes.value = [];
  dialogVisible.value = true;
}

async function handleEdit(row: WorkflowDefinitionDto) {
  isEdit.value = true;
  editingId.value = row.id;
  dialogVisible.value = true;
  try {
    const detail = await getApprovalFlowDefinition(row.id);
    formData.value = {
      code: detail.code,
      name: detail.name,
      entityType: detail.entityType,
    };
    editingNodes.value = detail.nodes ? [...detail.nodes] : [];
  } catch {
    ElMessage.error('加载定义详情失败');
  }
}

function handleDialogClose() {
  dialogVisible.value = false;
  editingNodes.value = [];
}

async function handleSave() {
  if (!formRef.value) return;
  const valid = await formRef.value.validate().catch(() => false);
  if (!valid) return;

  saveLoading.value = true;
  try {
    const data: WorkflowDefinitionDto = {
      id: editingId.value || '',
      code: formData.value.code,
      name: formData.value.name,
      entityType: formData.value.entityType,
      version: 1,
      isPublished: false,
      nodes: editingNodes.value,
    };

    if (isEdit.value) {
      await updateDefinition(editingId.value, data);
      ElMessage.success('更新成功');
    } else {
      await createDefinition(data);
      ElMessage.success('创建成功');
    }
    dialogVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('保存失败');
  } finally {
    saveLoading.value = false;
  }
}

async function handlePublish(row: WorkflowDefinitionDto) {
  try {
    await ElMessageBox.confirm(
      `确认发布审批流「${row.name}」？发布后不可再修改节点配置。`,
      '发布确认',
      { type: 'warning' }
    );
    await publishDefinition(row.id);
    ElMessage.success('发布成功');
    handleSearch();
  } catch {
    // cancelled or error
  }
}

async function handleDelete(row: WorkflowDefinitionDto) {
  try {
    await ElMessageBox.confirm(
      `确认删除审批流「${row.name}」？`,
      '删除确认',
      { type: 'warning' }
    );
    await deleteDefinition(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // cancelled
  }
}

handleSearch();
</script>

<style scoped lang="scss">
@use '@/styles/variables.scss' as *;

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

.dialog-footer {
  margin-top: 16px;
  text-align: right;
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
</style>
