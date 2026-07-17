<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="编码">
        <el-input v-model="filters.code" placeholder="请输入规则编码" clearable />
      </el-form-item>
      <el-form-item label="名称">
        <el-input v-model="filters.name" placeholder="请输入规则名称" clearable />
      </el-form-item>
      <el-form-item label="规则类型">
        <el-input v-model="filters.ruleType" placeholder="请输入规则类型" clearable />
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>业务规则列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建规则
            </el-button>
            <el-button @click="handleImport">
              <el-icon><Download /></el-icon> 导入行业包
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
        <el-table-column prop="ruleType" label="规则类型" width="120" />
        <el-table-column prop="priority" label="优先级" width="80" align="center" sortable />
        <el-table-column prop="isEnabled" label="启用" width="80" align="center">
          <template #default="{ row }">
            <el-switch
              :model-value="(row as BusinessRuleDto).isEnabled"
              size="small"
              @change="(val: boolean) => handleToggle(row as BusinessRuleDto, val)"
            />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right" align="center">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row as BusinessRuleDto)">
              编辑
            </el-button>
            <el-button link type="danger" @click="handleDelete(row as BusinessRuleDto)">
              删除
            </el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <!-- Edit Dialog -->
    <WmsDialog
      :visible="dialogVisible"
      :title="isEdit ? '编辑规则' : '新建规则'"
      width="600px"
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
          <el-input v-model="formData.code" placeholder="请输入规则编码" />
        </el-form-item>
        <el-form-item label="名称" prop="name">
          <el-input v-model="formData.name" placeholder="请输入规则名称" />
        </el-form-item>
        <el-form-item label="规则类型" prop="ruleType">
          <el-input v-model="formData.ruleType" placeholder="如: PutawayStrategy" />
        </el-form-item>
        <el-form-item label="优先级" prop="priority">
          <el-input-number v-model="formData.priority" :min="0" :max="999" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input
            v-model="formData.description"
            type="textarea"
            :rows="2"
            placeholder="规则描述（选填）"
          />
        </el-form-item>
        <el-form-item label="表达式">
          <el-input
            v-model="formData.expression"
            type="textarea"
            :rows="3"
            placeholder="规则表达式（选填）"
          />
        </el-form-item>
      </el-form>

      <div class="dialog-footer">
        <el-button @click="handleDialogClose">取消</el-button>
        <el-button type="primary" :loading="saveLoading" @click="handleSave">
          保存
        </el-button>
      </div>
    </WmsDialog>

    <!-- Import Industry Package Dialog -->
    <WmsDialog
      :visible="importVisible"
      title="导入行业包"
      width="600px"
      :show-footer="false"
      @close="importVisible = false"
    >
      <el-table
        v-loading="importLoading"
        :data="industryPackages"
        border
        stripe
        max-height="400"
      >
        <el-table-column prop="code" label="编码" min-width="120" />
        <el-table-column prop="name" label="名称" min-width="160" show-overflow-tooltip />
        <el-table-column prop="industry" label="行业" width="100" />
        <el-table-column prop="version" label="版本" width="80" />
        <el-table-column label="操作" width="100" align="center">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleImportPackage(row as IndustryPackageDto)">
              导入
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-empty v-if="!importLoading && industryPackages.length === 0" description="暂无行业包" />
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus';
import { Plus, Download } from '@element-plus/icons-vue';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { useTable } from '@/hooks/useTable';
import {
  getBusinessRules,
  getBusinessRule,
  createRule,
  updateRule,
  deleteRule,
  getIndustryPackages,
  importPackage,
  type BusinessRuleDto,
  type IndustryPackageDto,
} from '@/api/ruleEngine';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<BusinessRuleDto>('/api/v1/rule-engine/rules');

// ── Edit Dialog ──────────────────────────────────────────────
const dialogVisible = ref(false);
const isEdit = ref(false);
const saveLoading = ref(false);
const editingId = ref('');
const formRef = ref<FormInstance>();

const formData = ref({
  code: '',
  name: '',
  ruleType: '',
  priority: 0,
  description: '',
  expression: '',
});

const formRules: FormRules = {
  code: [{ required: true, message: '请输入规则编码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入规则名称', trigger: 'blur' }],
  ruleType: [{ required: true, message: '请输入规则类型', trigger: 'blur' }],
  priority: [{ required: true, message: '请输入优先级', trigger: 'blur' }],
};

function handleCreate() {
  isEdit.value = false;
  editingId.value = '';
  formData.value = { code: '', name: '', ruleType: '', priority: 0, description: '', expression: '' };
  dialogVisible.value = true;
}

async function handleEdit(row: BusinessRuleDto) {
  isEdit.value = true;
  editingId.value = row.id;
  dialogVisible.value = true;
  try {
    const detail = await getBusinessRule(row.id);
    formData.value = {
      code: detail.code,
      name: detail.name,
      ruleType: detail.ruleType,
      priority: detail.priority,
      description: detail.description || '',
      expression: detail.expression || '',
    };
  } catch {
    ElMessage.error('加载规则详情失败');
  }
}

function handleDialogClose() {
  dialogVisible.value = false;
}

async function handleSave() {
  if (!formRef.value) return;
  const valid = await formRef.value.validate().catch(() => false);
  if (!valid) return;

  saveLoading.value = true;
  try {
    const data: BusinessRuleDto = {
      id: editingId.value || '',
      code: formData.value.code,
      name: formData.value.name,
      ruleType: formData.value.ruleType,
      priority: formData.value.priority,
      isEnabled: true,
      description: formData.value.description || undefined,
      expression: formData.value.expression || undefined,
    };

    if (isEdit.value) {
      await updateRule(editingId.value, data);
      ElMessage.success('更新成功');
    } else {
      await createRule(data);
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

// ── Toggle ───────────────────────────────────────────────────
async function handleToggle(row: BusinessRuleDto, value: boolean) {
  try {
    const data = { ...row, isEnabled: value };
    await updateRule(row.id, data);
    ElMessage.success(value ? '已启用' : '已禁用');
    handleSearch();
  } catch {
    ElMessage.error('操作失败');
  }
}

// ── Delete ───────────────────────────────────────────────────
async function handleDelete(row: BusinessRuleDto) {
  try {
    await ElMessageBox.confirm(
      `确认删除规则「${row.name}」？`,
      '删除确认',
      { type: 'warning' }
    );
    await deleteRule(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // cancelled
  }
}

// ── Import ───────────────────────────────────────────────────
const importVisible = ref(false);
const importLoading = ref(false);
const industryPackages = ref<IndustryPackageDto[]>([]);

async function handleImport() {
  importVisible.value = true;
  importLoading.value = true;
  try {
    const result = await getIndustryPackages({ maxResultCount: 50 });
    industryPackages.value = result.items || [];
  } catch {
    ElMessage.error('加载行业包失败');
  } finally {
    importLoading.value = false;
  }
}

async function handleImportPackage(row: IndustryPackageDto) {
  try {
    await ElMessageBox.confirm(
      `确认导入行业包「${row.name}」？已存在的规则可能会被覆盖。`,
      '导入确认',
      { type: 'warning' }
    );
    await importPackage(row.id);
    ElMessage.success('导入成功');
    importVisible.value = false;
    handleSearch();
  } catch {
    // cancelled or error
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
