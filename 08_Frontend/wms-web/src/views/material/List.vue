<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="编码">
        <el-input v-model="filters.code" placeholder="请输入物料编码" clearable />
      </el-form-item>
      <el-form-item label="名称">
        <el-input v-model="filters.name" placeholder="请输入物料名称" clearable />
      </el-form-item>
      <el-form-item label="分类">
        <el-input v-model="filters.classificationName" placeholder="请输入分类" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="启用" :value="1" />
          <el-option label="停用" :value="0" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>物料列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建物料
            </el-button>
            <WmsExportButton :export-api="exportMaterials" filename="物料清单.xlsx" />
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
          <el-table-column prop="materialCode" label="物料编码" sortable />
          <el-table-column prop="materialName" label="物料名称" show-overflow-tooltip />
          <el-table-column prop="specification" label="规格" show-overflow-tooltip />
          <el-table-column prop="classificationName" label="分类" />
          <el-table-column prop="primaryUnitName" label="单位" />
          <el-table-column prop="materialTypeDescription" label="类型" />
          <el-table-column prop="isActive" label="状态" align="center" width="90">
            <template #default="{ row }">
              <WmsStatusTag :status="row.isActive ? 'Available' : 'Outsourced'" type="inventory" />
            </template>
          </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as MaterialDto)">详情</el-button>
            <el-button link type="primary" @click="handleEdit(row as MaterialDto)">编辑</el-button>
            <el-button link :type="(row as MaterialDto).isActive ? 'danger' : 'success'" @click="handleToggleStatus(row as MaterialDto)">
              {{ (row as MaterialDto).isActive ? '停用' : '启用' }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row as MaterialDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      :title="formData.id ? '编辑物料' : '新建物料'"
      :visible="visible"
      :confirm-loading="submitting"
      show-footer
      width="700px"
      @close="closeForm"
      @cancel="closeForm"
      @confirm="handleSubmit"
    >
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="物料编码" prop="materialCode">
              <el-input v-model="formData.materialCode" placeholder="请输入物料编码" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="物料名称" prop="materialName">
              <el-input v-model="formData.materialName" placeholder="请输入物料名称" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="分类" prop="classificationId">
              <el-select v-model="formData.classificationId" placeholder="请选择分类" clearable style="width: 100%">
                <el-option
                  v-for="item in classificationList"
                  :key="item.id"
                  :label="item.classificationName"
                  :value="item.id"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="物料类型" prop="materialType">
              <el-select v-model="formData.materialType" placeholder="请选择物料类型" style="width: 100%">
                <el-option label="原材料" :value="0" />
                <el-option label="半成品" :value="1" />
                <el-option label="成品" :value="2" />
                <el-option label="包装材料" :value="3" />
                <el-option label="辅料" :value="4" />
                <el-option label="工具" :value="5" />
                <el-option label="设备备件" :value="6" />
                <el-option label="其他" :value="7" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="基本单位" prop="primaryUnitId">
              <el-select v-model="formData.primaryUnitId" placeholder="请选择基本单位" style="width: 100%" @change="handlePrimaryUnitChange">
                <el-option
                  v-for="item in unitOptions"
                  :key="item.id"
                  :label="item.itemName"
                  :value="item.id"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="采购单位">
              <el-select v-model="formData.purchaseUnitCode" placeholder="请选择采购单位" clearable style="width: 100%" @change="handleUnitChange('purchase')">
                <el-option
                  v-for="item in unitOptions"
                  :key="item.itemCode"
                  :label="item.itemName"
                  :value="item.itemCode"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="库存单位">
              <el-select v-model="formData.inventoryUnitCode" placeholder="请选择库存单位" clearable style="width: 100%" @change="handleUnitChange('inventory')">
                <el-option
                  v-for="item in unitOptions"
                  :key="item.itemCode"
                  :label="item.itemName"
                  :value="item.itemCode"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="销售单位">
              <el-select v-model="formData.salesUnitCode" placeholder="请选择销售单位" clearable style="width: 100%" @change="handleUnitChange('sales')">
                <el-option
                  v-for="item in unitOptions"
                  :key="item.itemCode"
                  :label="item.itemName"
                  :value="item.itemCode"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="发料策略">
              <el-select v-model="formData.issueStrategyType" placeholder="请选择发料策略" clearable style="width: 100%">
                <el-option label="先进先出" :value="0" />
                <el-option label="后进先出" :value="1" />
                <el-option label="FEFO" :value="2" />
                <el-option label="指定批次" :value="3" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="规格">
              <el-input v-model="formData.specification" placeholder="请输入规格" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="批次管理">
              <el-switch v-model="formData.batchManagementEnabled" active-text="启用" inactive-text="关闭" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="序列号管理">
              <el-switch v-model="formData.serialManagementEnabled" active-text="启用" inactive-text="关闭" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="状态" prop="isActive">
          <el-radio-group v-model="formData.isActive">
            <el-radio :label="true">启用</el-radio>
            <el-radio :label="false">停用</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsExportButton from '@/components/common/WmsExportButton.vue';
import { useTable } from '@/hooks/useTable';
import { useForm } from '@/hooks/useForm';
import {
  createMaterial,
  updateMaterial,
  deleteMaterial,
  enableMaterial,
  disableMaterial,
  getClassifications,
  getIssueStrategies,
} from '@/api/material';
import { getDictionaryItemsByCode } from '@/api/dataDictionary';
import type {
  MaterialDto,
  CreateMaterialDto,
  UpdateMaterialDto,
  MaterialClassificationDto,
  MaterialIssueStrategyDto,
} from '@/api/material';
import type { DictionaryItemDto } from '@/api/dataDictionary';

const router = useRouter();
const classificationList = ref<MaterialClassificationDto[]>([]);
const strategyList = ref<MaterialIssueStrategyDto[]>([]);
const unitOptions = ref<DictionaryItemDto[]>([]);

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<MaterialDto>('/api/v1/material/materials');

const { formRef, formData, formRules, submitting, visible, openForm, closeForm, submitForm } = useForm<Partial<MaterialDto>>({
  materialCode: '',
  materialName: '',
  classificationId: '',
  materialType: 0,
  primaryUnitId: '',
  primaryUnitName: '',
  issueStrategyType: 0,
  specification: '',
  batchManagementEnabled: false,
  serialManagementEnabled: false,
  isActive: true,
  purchaseUnitCode: '',
  purchaseUnitName: '',
  inventoryUnitCode: '',
  inventoryUnitName: '',
  salesUnitCode: '',
  salesUnitName: '',
});

formRules.value = {
  materialCode: [{ required: true, message: '请输入物料编码', trigger: 'blur' }],
  materialName: [{ required: true, message: '请输入物料名称', trigger: 'blur' }],
  classificationId: [{ required: true, message: '请选择分类', trigger: 'change' }],
  materialType: [{ required: true, message: '请选择物料类型', trigger: 'change' }],
  primaryUnitId: [{ required: true, message: '请选择基本单位', trigger: 'change' }],
};

async function loadOptions() {
  try {
    const [clsRes, strategyRes, unitRes] = await Promise.all([
      getClassifications({ maxResultCount: 1000 }),
      getIssueStrategies({ maxResultCount: 1000 }),
      getDictionaryItemsByCode('SysUnit'),
    ]);
    classificationList.value = clsRes.items;
    strategyList.value = strategyRes.items;
    unitOptions.value = unitRes;
  } catch {
    ElMessage.error('加载选项失败');
  }
}

function handleCreate() {
  openForm();
}

function handleEdit(row: MaterialDto) {
  openForm({ ...row });
}

function handlePrimaryUnitChange() {
  const unit = unitOptions.value.find(u => u.id === formData.primaryUnitId);
  if (unit) {
    formData.primaryUnitName = unit.itemName;
  }
}

function handleUnitChange(type: 'purchase' | 'inventory' | 'sales') {
  const code = type === 'purchase' ? formData.purchaseUnitCode :
               type === 'inventory' ? formData.inventoryUnitCode :
               formData.salesUnitCode;
  const unit = unitOptions.value.find(u => u.itemCode === code);
  if (unit) {
    if (type === 'purchase') {
      formData.purchaseUnitName = unit.itemName;
    } else if (type === 'inventory') {
      formData.inventoryUnitName = unit.itemName;
    } else {
      formData.salesUnitName = unit.itemName;
    }
  }
}

function handleDetail(row: MaterialDto) {
  router.push(`/material/detail/${row.id}`);
}

async function handleToggleStatus(row: MaterialDto) {
  try {
    if (row.isActive) {
      await disableMaterial(row.id);
    } else {
      await enableMaterial(row.id);
    }
    ElMessage.success('操作成功');
    handleSearch();
  } catch {
    ElMessage.error('操作失败');
  }
}

async function handleDelete(row: MaterialDto) {
  try {
    await ElMessageBox.confirm('确认删除该物料？', '提示', { type: 'warning' });
    await deleteMaterial(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // Cancel
  }
}

async function handleSubmit() {
  const createData: CreateMaterialDto = {
    materialCode: formData.materialCode || '',
    materialName: formData.materialName || '',
    classificationId: formData.classificationId,
    materialType: formData.materialType || 0,
    primaryUnitId: formData.primaryUnitId || '',
    primaryUnitName: formData.primaryUnitName || '',
    issueStrategyType: formData.issueStrategyType,
    specification: formData.specification,
    batchManagementEnabled: formData.batchManagementEnabled,
    serialManagementEnabled: formData.serialManagementEnabled,
    isActive: formData.isActive,
    purchaseUnitCode: formData.purchaseUnitCode,
    purchaseUnitName: formData.purchaseUnitName,
    inventoryUnitCode: formData.inventoryUnitCode,
    inventoryUnitName: formData.inventoryUnitName,
    salesUnitCode: formData.salesUnitCode,
    salesUnitName: formData.salesUnitName,
  };
  const updateData: UpdateMaterialDto = {
    materialName: formData.materialName || '',
    classificationId: formData.classificationId,
    materialType: formData.materialType || 0,
    primaryUnitName: formData.primaryUnitName || '',
    issueStrategyType: formData.issueStrategyType,
    specification: formData.specification,
    batchManagementEnabled: formData.batchManagementEnabled,
    serialManagementEnabled: formData.serialManagementEnabled,
    isActive: formData.isActive,
    purchaseUnitCode: formData.purchaseUnitCode,
    purchaseUnitName: formData.purchaseUnitName,
    inventoryUnitCode: formData.inventoryUnitCode,
    inventoryUnitName: formData.inventoryUnitName,
    salesUnitCode: formData.salesUnitCode,
    salesUnitName: formData.salesUnitName,
  };
  const success = await submitForm(async () => {
    if (formData.id) {
      await updateMaterial(formData.id, updateData);
    } else {
      await createMaterial(createData);
    }
  }, formData.id ? '更新成功' : '创建成功');
  if (success) handleSearch();
}

async function exportMaterials() {
  return { fileUrl: '/api/wms/material/export', rowCount: total.value };
}

onMounted(() => {
  loadOptions();
});

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
