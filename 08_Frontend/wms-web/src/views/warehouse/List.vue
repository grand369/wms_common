<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="编码">
        <el-input v-model="filters.warehouseCode" placeholder="请输入仓库编码" clearable />
      </el-form-item>
      <el-form-item label="名称">
        <el-input v-model="filters.warehouseName" placeholder="请输入仓库名称" clearable />
      </el-form-item>
      <el-form-item label="类型">
        <el-select v-model="filters.warehouseType" placeholder="请选择类型" clearable>
          <el-option label="原材料仓" :value="0" />
          <el-option label="成品仓" :value="1" />
          <el-option label="线边仓" :value="2" />
          <el-option label="半成品仓" :value="3" />
          <el-option label="辅料仓" :value="4" />
          <el-option label="备件仓" :value="5" />
          <el-option label="危化品仓" :value="6" />
          <el-option label="退货仓" :value="7" />
          <el-option label="冷链仓" :value="8" />
          <el-option label="常温仓" :value="9" />
          <el-option label="室外仓" :value="10" />
          <el-option label="临时仓" :value="11" />
        </el-select>
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
          <span>仓库列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建
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
        <el-table-column prop="warehouseCode" label="编码" />
        <el-table-column prop="warehouseName" label="名称" />
        <el-table-column prop="warehouseTypeDescription" label="类型" />
        <el-table-column prop="plantName" label="所属工厂" />
        <el-table-column prop="organizationUnitName" label="组织单元" />
        <el-table-column prop="address" label="地址" show-overflow-tooltip />
        <el-table-column prop="isActive" label="状态" align="center">
          <template #default="{ row }">
            <WmsStatusTag :status="row.isActive ? 'Available' : 'Outsourced'" type="inventory" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="220" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as WarehouseDto)">详情</el-button>
            <el-button link type="primary" @click="handleEdit(row as WarehouseDto)">编辑</el-button>
            <el-button link :type="(row as WarehouseDto).isActive ? 'danger' : 'success'" @click="handleToggleStatus(row as WarehouseDto)">
              {{ (row as WarehouseDto).isActive ? '停用' : '启用' }}
            </el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      :title="formData.id ? '编辑仓库' : '新建仓库'"
      :visible="visible"
      :confirm-loading="submitting"
      show-footer
      width="600px"
      @close="closeForm"
      @cancel="closeForm"
      @confirm="handleSubmit"
    >
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-form-item label="仓库编码" prop="warehouseCode">
          <el-input v-model="formData.warehouseCode" placeholder="请输入仓库编码" />
        </el-form-item>
        <el-form-item label="仓库名称" prop="warehouseName">
          <el-input v-model="formData.warehouseName" placeholder="请输入仓库名称" />
        </el-form-item>
        <el-form-item label="仓库类型" prop="warehouseType">
          <el-select v-model="formData.warehouseType" placeholder="请选择仓库类型" style="width: 100%">
            <el-option label="原材料仓" :value="0" />
            <el-option label="成品仓" :value="1" />
            <el-option label="线边仓" :value="2" />
            <el-option label="半成品仓" :value="3" />
            <el-option label="辅料仓" :value="4" />
            <el-option label="备件仓" :value="5" />
            <el-option label="危化品仓" :value="6" />
            <el-option label="退货仓" :value="7" />
            <el-option label="冷链仓" :value="8" />
            <el-option label="常温仓" :value="9" />
            <el-option label="室外仓" :value="10" />
            <el-option label="临时仓" :value="11" />
          </el-select>
        </el-form-item>
        <el-form-item label="所属工厂ID" prop="plantId">
          <el-input v-model="formData.plantId" placeholder="请输入工厂ID" />
        </el-form-item>
        <el-form-item label="工厂名称" prop="plantName">
          <el-input v-model="formData.plantName" placeholder="请输入工厂名称" />
        </el-form-item>
        <el-form-item label="组织单元ID" prop="organizationUnitId">
          <el-input v-model="formData.organizationUnitId" placeholder="请输入组织单元ID" />
        </el-form-item>
        <el-form-item label="组织单元名称" prop="organizationUnitName">
          <el-input v-model="formData.organizationUnitName" placeholder="请输入组织单元名称" />
        </el-form-item>
        <el-form-item label="库位层级数" prop="locationLevelCount">
          <el-select v-model="formData.locationLevelCount" placeholder="请选择库位层级数" style="width: 100%">
            <el-option label="3层" :value="3" />
            <el-option label="4层" :value="4" />
          </el-select>
        </el-form-item>
        <el-form-item label="存储条件" prop="storageConditionType">
          <el-select v-model="formData.storageConditionType" placeholder="请选择存储条件" style="width: 100%">
            <el-option label="常温" :value="0" />
            <el-option label="恒温" :value="1" />
            <el-option label="冷藏" :value="2" />
            <el-option label="冷冻" :value="3" />
            <el-option label="特殊" :value="4" />
          </el-select>
        </el-form-item>
        <el-form-item label="地址">
          <el-input v-model="formData.address" type="textarea" :rows="3" placeholder="请输入地址" />
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="formData.remark" placeholder="请输入备注" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import { useTable } from '@/hooks/useTable';
import { useForm } from '@/hooks/useForm';
import { createWarehouse, updateWarehouse, enableWarehouse, disableWarehouse } from '@/api/warehouse';
import type { WarehouseDto, CreateOrUpdateWarehouseDto } from '@/api/warehouse';

const router = useRouter();
const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } = useTable<WarehouseDto>('/api/v1/warehouse/warehouses');
const { formRef, formData, formRules, submitting, visible, openForm, closeForm, submitForm } = useForm<Partial<WarehouseDto>>({
  warehouseCode: '',
  warehouseName: '',
  warehouseType: 0,
  plantId: '',
  plantName: '',
  organizationUnitId: '',
  organizationUnitName: '',
  locationLevelCount: 3,
  storageConditionType: 0,
  address: '',
  remark: '',
  isActive: true,
});

formRules.value = {
  warehouseCode: [{ required: true, message: '请输入仓库编码', trigger: 'blur' }],
  warehouseName: [{ required: true, message: '请输入仓库名称', trigger: 'blur' }],
  warehouseType: [{ required: true, message: '请选择仓库类型', trigger: 'change' }],
  plantId: [{ required: true, message: '请输入工厂ID', trigger: 'blur' }],
  plantName: [{ required: true, message: '请输入工厂名称', trigger: 'blur' }],
  organizationUnitId: [{ required: true, message: '请输入组织单元ID', trigger: 'blur' }],
  organizationUnitName: [{ required: true, message: '请输入组织单元名称', trigger: 'blur' }],
  locationLevelCount: [{ required: true, message: '请选择库位层级数', trigger: 'change' }],
};

function handleCreate() {
  openForm();
}

function handleEdit(row: WarehouseDto) {
  openForm({ ...row });
}

function handleDetail(row: WarehouseDto) {
  router.push(`/warehouse/detail/${row.id}`);
}

async function handleToggleStatus(row: WarehouseDto) {
  try {
    if (row.isActive) {
      await disableWarehouse(row.id);
    } else {
      await enableWarehouse(row.id);
    }
    ElMessage.success('操作成功');
    handleSearch();
  } catch {
    ElMessage.error('操作失败');
  }
}

async function handleSubmit() {
  const data: CreateOrUpdateWarehouseDto = {
    warehouseCode: formData.warehouseCode || '',
    warehouseName: formData.warehouseName || '',
    warehouseType: formData.warehouseType ?? 0,
    plantId: formData.plantId || '',
    plantName: formData.plantName || '',
    organizationUnitId: formData.organizationUnitId || '',
    organizationUnitName: formData.organizationUnitName || '',
    locationLevelCount: formData.locationLevelCount ?? 3,
    storageConditionType: formData.storageConditionType ?? 0,
    address: formData.address,
    remark: formData.remark,
    isActive: formData.isActive ?? true,
  };
  const success = await submitForm(async () => {
    if (formData.id) {
      await updateWarehouse(formData.id, data);
    } else {
      await createWarehouse(data);
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
.header-actions {
  display: flex;
  gap: 8px;
}
</style>