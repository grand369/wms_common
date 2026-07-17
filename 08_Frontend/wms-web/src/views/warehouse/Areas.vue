<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="编码">
        <el-input v-model="filters.areaCode" placeholder="请输入库区编码" clearable />
      </el-form-item>
      <el-form-item label="名称">
        <el-input v-model="filters.areaName" placeholder="请输入库区名称" clearable />
      </el-form-item>
      <el-form-item label="功能类型">
        <el-select v-model="filters.areaFunction" placeholder="请选择功能类型" clearable>
          <el-option label="收货区" :value="0" />
          <el-option label="存储区" :value="1" />
          <el-option label="拣货区" :value="2" />
          <el-option label="发货区" :value="3" />
          <el-option label="退货区" :value="4" />
          <el-option label="其他" :value="5" />
        </el-select>
      </el-form-item>
      <el-form-item label="仓库">
        <WmsWarehouseSelector v-model="filters.warehouseId" />
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>库区管理</span>
          <el-button type="primary" @click="handleCreate">新建库区</el-button>
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
        <el-table-column prop="areaCode" label="编码" />
        <el-table-column prop="areaName" label="名称" />
        <el-table-column prop="warehouseCode" label="所属仓库" />
        <el-table-column prop="areaFunction" label="功能类型">
          <template #default="{ row }">
            {{ getAreaFunctionLabel(row.areaFunction) }}
          </template>
        </el-table-column>
        <el-table-column prop="maxCapacity" label="最大容量" />
        <el-table-column prop="isActive" label="状态" align="center">
          <template #default="{ row }">
            <WmsStatusTag :status="row.isActive ? 'Available' : 'Outsourced'" type="inventory" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row as AreaDto)">编辑</el-button>
            <el-button link type="danger" @click="handleDelete(row as AreaDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      :title="formData.id ? '编辑库区' : '新建库区'"
      :visible="visible"
      :confirm-loading="submitting"
      show-footer
      width="600px"
      @close="closeForm"
      @cancel="closeForm"
      @confirm="handleSubmit"
    >
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-form-item label="所属仓库" prop="warehouseId">
          <WmsWarehouseSelector 
            :model-value="formData.warehouseId || ''" 
            @update:model-value="(val) => { formData.warehouseId = typeof val === 'string' ? val : val[0] || '' }"
            @change="handleWarehouseChange"
          />
        </el-form-item>
        <el-form-item label="仓库编码" prop="warehouseCode">
          <el-input v-model="formData.warehouseCode" placeholder="自动获取" disabled />
        </el-form-item>
        <el-form-item label="库区编码" prop="areaCode">
          <el-input v-model="formData.areaCode" placeholder="请输入库区编码" />
        </el-form-item>
        <el-form-item label="库区名称" prop="areaName">
          <el-input v-model="formData.areaName" placeholder="请输入库区名称" />
        </el-form-item>
        <el-form-item label="功能类型" prop="areaFunction">
          <el-select v-model="formData.areaFunction" placeholder="请选择功能类型" style="width: 100%">
            <el-option label="收货区" :value="0" />
            <el-option label="存储区" :value="1" />
            <el-option label="拣货区" :value="2" />
            <el-option label="发货区" :value="3" />
            <el-option label="退货区" :value="4" />
            <el-option label="其他" :value="5" />
          </el-select>
        </el-form-item>
        <el-form-item label="存储环境">
          <el-select v-model="formData.storageEnvironment" placeholder="请选择存储环境" style="width: 100%">
            <el-option label="常温" :value="0" />
            <el-option label="恒温" :value="1" />
            <el-option label="冷藏" :value="2" />
            <el-option label="冷冻" :value="3" />
            <el-option label="特殊" :value="4" />
          </el-select>
        </el-form-item>
        <el-form-item label="最大容量">
          <el-input-number v-model="formData.maxCapacity" :min="0" style="width: 100%" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsWarehouseSelector from '@/components/common/WmsWarehouseSelector.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import { useTable } from '@/hooks/useTable';
import { useForm } from '@/hooks/useForm';
import { createArea, updateArea, deleteArea } from '@/api/warehouse';
import type { AreaDto, CreateOrUpdateAreaDto } from '@/api/warehouse';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } = useTable<AreaDto>('/api/v1/warehouse/areas');
const { formRef, formData, formRules, submitting, visible, openForm, closeForm, submitForm } = useForm<Partial<AreaDto>>({
  warehouseId: '',
  warehouseCode: '',
  areaCode: '',
  areaName: '',
  areaFunction: 1,
  storageEnvironment: 0,
  maxCapacity: 0,
  isActive: true,
});

formRules.value = {
  warehouseId: [{ required: true, message: '请选择所属仓库', trigger: 'change' }],
  warehouseCode: [{ required: true, message: '仓库编码不能为空', trigger: 'change' }],
  areaCode: [{ required: true, message: '请输入库区编码', trigger: 'blur' }],
  areaName: [{ required: true, message: '请输入库区名称', trigger: 'blur' }],
  areaFunction: [{ required: true, message: '请选择功能类型', trigger: 'change' }],
};

const areaFunctionLabels: Record<number, string> = {
  0: '收货区',
  1: '存储区',
  2: '拣货区',
  3: '发货区',
  4: '退货区',
  5: '其他',
};

function getAreaFunctionLabel(value: number): string {
  return areaFunctionLabels[value] || '未知';
}

function handleWarehouseChange(warehouse: any) {
  if (warehouse) {
    formData.warehouseId = warehouse.id;
    formData.warehouseCode = warehouse.code;
  } else {
    formData.warehouseId = '';
    formData.warehouseCode = '';
  }
}

function handleCreate() {
  openForm();
}

function handleEdit(row: AreaDto) {
  openForm({ ...row });
}

async function handleDelete(row: AreaDto) {
  try {
    await ElMessageBox.confirm('确认删除该库区？', '提示', { type: 'warning' });
    await deleteArea(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // Cancel
  }
}

async function handleSubmit() {
  const data: CreateOrUpdateAreaDto = {
    warehouseId: formData.warehouseId || '',
    warehouseCode: formData.warehouseCode || '',
    areaCode: formData.areaCode || '',
    areaName: formData.areaName || '',
    areaFunction: formData.areaFunction ?? 1,
    storageEnvironment: formData.storageEnvironment ?? 0,
    maxCapacity: formData.maxCapacity,
    isActive: formData.isActive ?? true,
  };
  const success = await submitForm(async () => {
    if (formData.id) {
      await updateArea(formData.id, data);
    } else {
      await createArea(data);
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