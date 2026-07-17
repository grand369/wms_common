<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="策略编码">
        <el-input v-model="filters.code" placeholder="请输入策略编码" clearable />
      </el-form-item>
      <el-form-item label="策略名称">
        <el-input v-model="filters.name" placeholder="请输入策略名称" clearable />
      </el-form-item>
      <el-form-item label="策略类型">
        <el-select v-model="filters.strategy" placeholder="请选择策略类型" clearable>
          <el-option label="先进先出" value="FIFO" />
          <el-option label="后进先出" value="LIFO" />
          <el-option label="按批次先进先出" value="FEFO" />
          <el-option label="指定批次" value="SpecifiedBatch" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>发料策略</span>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon> 新建策略
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
        <el-table-column prop="code" label="策略编码" />
        <el-table-column prop="name" label="策略名称" />
        <el-table-column prop="strategy" label="策略类型" />
        <el-table-column prop="description" label="说明" show-overflow-tooltip />
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row as MaterialIssueStrategyDto)">编辑</el-button>
            <el-button link type="danger" @click="handleDelete(row as MaterialIssueStrategyDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      :title="formData.id ? '编辑策略' : '新建策略'"
      :visible="visible"
      :confirm-loading="submitting"
      show-footer
      width="500px"
      @close="closeForm"
      @cancel="closeForm"
      @confirm="handleSubmit"
    >
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-form-item label="策略编码" prop="code">
          <el-input v-model="formData.code" placeholder="请输入策略编码" />
        </el-form-item>
        <el-form-item label="策略名称" prop="name">
          <el-input v-model="formData.name" placeholder="请输入策略名称" />
        </el-form-item>
        <el-form-item label="策略类型" prop="strategy">
          <el-select v-model="formData.strategy" placeholder="请选择策略类型" style="width: 100%">
            <el-option label="先进先出" value="FIFO" />
            <el-option label="后进先出" value="LIFO" />
            <el-option label="按批次先进先出" value="FEFO" />
            <el-option label="指定批次" value="SpecifiedBatch" />
          </el-select>
        </el-form-item>
        <el-form-item label="说明">
          <el-input v-model="formData.description" type="textarea" :rows="3" placeholder="请输入说明" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { Plus } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { useTable } from '@/hooks/useTable';
import { useForm } from '@/hooks/useForm';
import {
  createIssueStrategy,
  updateIssueStrategy,
  deleteIssueStrategy,
} from '@/api/material';
import type { MaterialIssueStrategyDto } from '@/api/material';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<MaterialIssueStrategyDto>('/api/v1/material/issue-strategies');

const { formRef, formData, formRules, submitting, visible, openForm, closeForm, submitForm } =
  useForm<Partial<MaterialIssueStrategyDto>>({
    code: '',
    name: '',
    strategy: 'FIFO',
    description: '',
  });

formRules.value = {
  code: [{ required: true, message: '请输入策略编码', trigger: 'blur' }],
  name: [{ required: true, message: '请输入策略名称', trigger: 'blur' }],
  strategy: [{ required: true, message: '请选择策略类型', trigger: 'change' }],
};

function handleCreate() {
  openForm();
}

function handleEdit(row: MaterialIssueStrategyDto) {
  openForm({ ...row });
}

async function handleDelete(row: MaterialIssueStrategyDto) {
  try {
    await ElMessageBox.confirm('确认删除该策略？', '提示', { type: 'warning' });
    await deleteIssueStrategy(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // Cancel
  }
}

async function handleSubmit() {
  const data: MaterialIssueStrategyDto = {
    id: formData.id || '',
    code: formData.code || '',
    name: formData.name || '',
    strategy: formData.strategy || 'FIFO',
    description: formData.description,
  };
  const success = await submitForm(async () => {
    if (formData.id) {
      await updateIssueStrategy(formData.id, data);
    } else {
      await createIssueStrategy(data);
    }
  }, formData.id ? '更新成功' : '创建成功');
  if (success) handleSearch();
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
</style>
