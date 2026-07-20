<template>
  <div class="page-container">
    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <div class="header-left">
            <el-button link type="primary" @click="goBack">
              <el-icon><ArrowLeft /></el-icon> 返回
            </el-button>
            <span>{{ dictionaryName }} - 字典项管理</span>
          </div>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon> 新建字典项
          </el-button>
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
        <el-table-column prop="itemCode" label="项编码" />
        <el-table-column prop="itemName" label="项名称" show-overflow-tooltip />
        <el-table-column prop="itemValue" label="项值" show-overflow-tooltip />
        <el-table-column prop="description" label="描述" show-overflow-tooltip />
        <el-table-column prop="sortOrder" label="排序" width="80" />
        <el-table-column prop="isActive" label="状态" align="center" width="90">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'">
              {{ row.isActive ? '启用' : '停用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row as DictionaryItemDto)">编辑</el-button>
            <el-button link type="danger" @click="handleDelete(row as DictionaryItemDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      :title="formData.id ? '编辑字典项' : '新建字典项'"
      :visible="visible"
      :confirm-loading="submitting"
      show-footer
      width="500px"
      @close="closeForm"
      @cancel="closeForm"
      @confirm="handleSubmit"
    >
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-form-item label="项编码" prop="itemCode">
          <el-input v-model="formData.itemCode" placeholder="请输入项编码" :disabled="!!formData.id" />
        </el-form-item>
        <el-form-item label="项名称" prop="itemName">
          <el-input v-model="formData.itemName" placeholder="请输入项名称" />
        </el-form-item>
        <el-form-item label="项值">
          <el-input v-model="formData.itemValue" placeholder="请输入项值" />
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
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { Plus, ArrowLeft } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { useTable } from '@/hooks/useTable';
import { useForm } from '@/hooks/useForm';
import {
  getDictionaryItems,
  getDictionary,
  createDictionaryItem,
  updateDictionaryItem,
  deleteDictionaryItem,
} from '@/api/dataDictionary';
import type { DictionaryItemDto, DictionaryItemCreateDto, DictionaryItemUpdateDto, DictionaryDto } from '@/api/dataDictionary';

const router = useRouter();
const route = useRoute();
const dictionaryId = route.params.id as string;

const dictionaryName = ref('');
const loading = ref(false);
const tableData = ref<DictionaryItemDto[]>([]);
const total = ref(0);

const { pagination, handlePageChange, handleSizeChange } = useTable<DictionaryItemDto>('/api/v1/data-dictionary/items');

const { formRef, formData, formRules, submitting, visible, openForm, closeForm, submitForm } =
  useForm<Partial<DictionaryItemDto>>({
    itemCode: '',
    itemName: '',
    itemValue: '',
    description: '',
    sortOrder: 0,
    isActive: true,
  });

formRules.value = {
  itemCode: [{ required: true, message: '请输入项编码', trigger: 'blur' }],
  itemName: [{ required: true, message: '请输入项名称', trigger: 'blur' }],
};

async function loadDictionaryInfo() {
  try {
    const res = await getDictionary(dictionaryId);
    dictionaryName.value = res.dictionaryName;
  } catch {
    dictionaryName.value = '字典项管理';
  }
}

async function handleSearch() {
  loading.value = true;
  try {
    const res = await getDictionaryItems(dictionaryId);
    tableData.value = res;
    total.value = res.length;
  } catch {
    ElMessage.error('加载字典项失败');
  } finally {
    loading.value = false;
  }
}

function goBack() {
  router.push('/data-dictionary/list');
}

function handleCreate() {
  openForm({
    itemCode: '',
    itemName: '',
    itemValue: '',
    description: '',
    sortOrder: tableData.value.length + 1,
    isActive: true,
  });
}

function handleEdit(row: DictionaryItemDto) {
  openForm({ ...row });
}

async function handleDelete(row: DictionaryItemDto) {
  try {
    await ElMessageBox.confirm('确认删除该字典项？', '提示', { type: 'warning' });
    await deleteDictionaryItem(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // Cancel
  }
}

async function handleSubmit() {
  if (formData.id) {
    const updateData: DictionaryItemUpdateDto = {
      itemName: formData.itemName || '',
      itemValue: formData.itemValue,
      description: formData.description,
      sortOrder: formData.sortOrder ?? 0,
      isActive: formData.isActive,
    };
    const success = await submitForm(async () => {
      await updateDictionaryItem(formData.id!, updateData);
    }, '更新成功');
    if (success) {
      handleSearch();
    }
  } else {
    const createData: DictionaryItemCreateDto = {
      dictionaryId,
      itemCode: formData.itemCode || '',
      itemName: formData.itemName || '',
      itemValue: formData.itemValue,
      description: formData.description,
      sortOrder: formData.sortOrder ?? tableData.value.length + 1,
      isActive: formData.isActive ?? true,
    };
    const success = await submitForm(async () => {
      await createDictionaryItem(createData);
    }, '创建成功');
    if (success) {
      handleSearch();
    }
  }
}

onMounted(() => {
  loadDictionaryInfo();
  handleSearch();
});
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
.header-left {
  display: flex;
  align-items: center;
  gap: 12px;
}
</style>
