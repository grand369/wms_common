<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="字典编码">
        <el-input v-model="filters.dictionaryCode" placeholder="请输入字典编码" clearable />
      </el-form-item>
      <el-form-item label="字典名称">
        <el-input v-model="filters.dictionaryName" placeholder="请输入字典名称" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.isActive" placeholder="请选择状态" clearable>
          <el-option label="启用" :value="true" />
          <el-option label="停用" :value="false" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>数据字典管理</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建字典
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
        <el-table-column prop="dictionaryCode" label="字典编码" />
        <el-table-column prop="dictionaryName" label="字典名称" show-overflow-tooltip />
        <el-table-column prop="description" label="描述" show-overflow-tooltip />
        <el-table-column prop="sortOrder" label="排序" width="80" />
        <el-table-column prop="isActive" label="状态" align="center" width="90">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'">
              {{ row.isActive ? '启用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="creationTime" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatDate(row.creationTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleManageItems(row as DictionaryDto)">字典项</el-button>
            <el-button link type="primary" @click="handleEdit(row as DictionaryDto)">编辑</el-button>
            <el-button link type="danger" @click="handleDelete(row as DictionaryDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      :title="formData.id ? '编辑字典' : '新建字典'"
      :visible="visible"
      :confirm-loading="submitting"
      show-footer
      width="500px"
      @close="closeForm"
      @cancel="closeForm"
      @confirm="handleSubmit"
    >
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-form-item label="字典编码" prop="dictionaryCode">
          <el-input v-model="formData.dictionaryCode" placeholder="请输入字典编码" :disabled="!!formData.id" />
        </el-form-item>
        <el-form-item label="字典名称" prop="dictionaryName">
          <el-input v-model="formData.dictionaryName" placeholder="请输入字典名称" />
        </el-form-item>
        <el-form-item label="描述">
          <el-input v-model="formData.description" type="textarea" placeholder="请输入描述" :rows="3" />
        </el-form-item>
        <el-form-item label="排序">
          <el-input-number v-model="formData.sortOrder" :min="0" :max="9999" placeholder="排序" />
        </el-form-item>
        <el-form-item label="启用状态">
          <el-switch v-model="formData.isActive" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsSearch from '@/components/common/WmsSearch.vue';
import { useTable } from '@/hooks/useTable';
import { useForm } from '@/hooks/useForm';
import {
  getDictionaries,
  createDictionary,
  updateDictionary,
  deleteDictionary,
} from '@/api/dataDictionary';
import type { DictionaryDto, DictionaryCreateDto, DictionaryUpdateDto } from '@/api/dataDictionary';

const router = useRouter();

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<DictionaryDto>('/api/v1/data-dictionary/dictionaries');

const { formRef, formData, formRules, submitting, visible, openForm, closeForm, submitForm } =
  useForm<Partial<DictionaryDto>>({
    dictionaryCode: '',
    dictionaryName: '',
    description: '',
    sortOrder: 0,
    isActive: true,
  });

formRules.value = {
  dictionaryCode: [{ required: true, message: '请输入字典编码', trigger: 'blur' }],
  dictionaryName: [{ required: true, message: '请输入字典名称', trigger: 'blur' }],
};

function formatDate(dateStr?: string) {
  if (!dateStr) return '-';
  return new Date(dateStr).toLocaleString('zh-CN');
}

function handleCreate() {
  openForm({
    dictionaryCode: '',
    dictionaryName: '',
    description: '',
    sortOrder: 0,
    isActive: true,
  });
}

function handleEdit(row: DictionaryDto) {
  openForm({ ...row });
}

function handleManageItems(row: DictionaryDto) {
  router.push(`/data-dictionary/items/${row.id}`);
}

async function handleDelete(row: DictionaryDto) {
  try {
    await ElMessageBox.confirm('确认删除该字典？', '提示', { type: 'warning' });
    await deleteDictionary(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // Cancel
  }
}

async function handleSubmit() {
  if (formData.id) {
    const updateData: DictionaryUpdateDto = {
      dictionaryName: formData.dictionaryName || '',
      description: formData.description,
      sortOrder: formData.sortOrder ?? 0,
      isActive: formData.isActive,
    };
    const success = await submitForm(async () => {
      await updateDictionary(formData.id!, updateData);
    }, '更新成功');
    if (success) {
      handleSearch();
    }
  } else {
    const createData: DictionaryCreateDto = {
      dictionaryCode: formData.dictionaryCode || '',
      dictionaryName: formData.dictionaryName || '',
      description: formData.description,
      sortOrder: formData.sortOrder ?? 0,
      isActive: formData.isActive ?? true,
    };
    const success = await submitForm(async () => {
      await createDictionary(createData);
    }, '创建成功');
    if (success) {
      handleSearch();
    }
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
}
</style>
