<template>
  <div class="page-container">
    <el-row :gutter="16" class="location-layout">
      <el-col :span="5">
        <el-card shadow="hover" class="tree-card">
          <template #header>仓库-库区</template>
          <el-tree
            :data="warehouseTree"
            :props="treeProps"
            node-key="id"
            default-expand-all
            highlight-current
            @node-click="handleNodeClick"
          />
        </el-card>
      </el-col>
      <el-col :span="19">
        <WmsSearch @search="handleSearch" @reset="resetFilters">
          <el-form-item label="库位编码">
            <el-input v-model="filters.locationCode" placeholder="请输入库位编码" clearable />
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
              <span>库位列表</span>
              <div class="header-actions">
                <el-button type="primary" @click="handleCreate">新建库位</el-button>
                <el-button @click="batchDialogVisible = true">批量创建</el-button>
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
            <el-table-column prop="locationCode" label="库位编码" />
            <el-table-column prop="warehouseCode" label="仓库" />
            <el-table-column prop="areaCode" label="库区" />
            <el-table-column prop="barcodeId" label="条码ID" />
            <el-table-column prop="locationType" label="库位类型">
              <template #default="{ row }">
                {{ getLocationTypeLabel(row.locationType) }}
              </template>
            </el-table-column>
            <el-table-column prop="maxCapacity" label="最大容量" />
            <el-table-column prop="maxWeight" label="最大重量" />
            <el-table-column prop="row" label="行" />
            <el-table-column prop="column" label="列" />
            <el-table-column prop="layer" label="层" />
            <el-table-column prop="isActive" label="状态" align="center">
              <template #default="{ row }">
                <WmsStatusTag :status="row.isActive ? 'Available' : 'Outsourced'" type="inventory" />
              </template>
            </el-table-column>
            <el-table-column label="操作" width="180" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="handleEdit(row as LocationDto)">编辑</el-button>
                <el-button link type="danger" @click="handleDelete(row as LocationDto)">删除</el-button>
              </template>
            </el-table-column>
          </WmsTable>
        </el-card>
      </el-col>
    </el-row>

    <WmsDialog
      :title="formData.id ? '编辑库位' : '新建库位'"
      :visible="visible"
      :confirm-loading="submitting"
      show-footer
      width="600px"
      @close="closeForm"
      @cancel="closeForm"
      @confirm="handleSubmit"
    >
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-form-item label="所属库区" prop="areaId">
          <WmsAreaSelector v-model="formData.areaId" @change="handleAreaChange" :disabled="!!formData.id" />
        </el-form-item>
        <el-form-item label="仓库编码" prop="warehouseCode">
          <el-input v-model="formData.warehouseCode" placeholder="自动获取" disabled />
        </el-form-item>
        <el-form-item label="库区编码" prop="areaCode">
          <el-input v-model="formData.areaCode" placeholder="自动获取" disabled />
        </el-form-item>
        <el-form-item label="库位编码" prop="locationCode">
          <el-input v-model="formData.locationCode" placeholder="请输入库位编码" :disabled="!!formData.id" />
        </el-form-item>
        <el-form-item label="条码ID" prop="barcodeId">
          <el-input v-model="formData.barcodeId" placeholder="请输入条码ID" :disabled="!!formData.id" />
        </el-form-item>
        <el-form-item label="库位类型">
          <el-select v-model="formData.locationType" placeholder="请选择库位类型" style="width: 100%">
            <el-option label="普通库位" :value="0" />
            <el-option label="高架库位" :value="1" />
            <el-option label="地堆库位" :value="2" />
            <el-option label="冷冻库位" :value="3" />
            <el-option label="特殊库位" :value="4" />
          </el-select>
        </el-form-item>
        <el-form-item label="存储条件">
          <el-select v-model="formData.storageCondition" placeholder="请选择存储条件" style="width: 100%">
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
        <el-form-item label="最大重量">
          <el-input-number v-model="formData.maxWeight" :min="0" style="width: 100%" />
        </el-form-item>
        <el-form-item label="行号">
          <el-input v-model="formData.row" placeholder="如：A" />
        </el-form-item>
        <el-form-item label="列号">
          <el-input v-model="formData.column" placeholder="如：01" />
        </el-form-item>
        <el-form-item label="层号">
          <el-input v-model="formData.layer" placeholder="如：01" />
        </el-form-item>
      </el-form>
    </WmsDialog>

    <WmsDialog
      title="批量创建库位"
      :visible="batchDialogVisible"
      show-footer
      width="600px"
      @close="batchDialogVisible = false"
      @cancel="batchDialogVisible = false"
      @confirm="handleBatchSubmit"
    >
      <el-form label-width="100px">
        <el-form-item label="所属库区" required>
          <WmsAreaSelector v-model="batchForm.areaId" @change="handleBatchAreaChange" />
        </el-form-item>
        <el-form-item label="库区编码">
          <el-input v-model="batchForm.areaCode" placeholder="自动获取" disabled />
        </el-form-item>
        <el-form-item label="编码前缀">
          <el-input v-model="batchForm.prefix" placeholder="如：A-01-" />
        </el-form-item>
        <el-form-item label="起始编号">
          <el-input-number v-model="batchForm.startNumber" :min="1" />
        </el-form-item>
        <el-form-item label="数量">
          <el-input-number v-model="batchForm.count" :min="1" :max="100" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsAreaSelector from '@/components/common/WmsAreaSelector.vue';
import { useTable } from '@/hooks/useTable';
import { useForm } from '@/hooks/useForm';
import { createLocation, batchCreateLocations, updateLocation, deleteLocation, getWarehouses, getAreas } from '@/api/warehouse';
import type { LocationDto, CreateOrUpdateLocationDto, LocationUpdateDto } from '@/api/warehouse';

interface TreeNode {
  id: string;
  name: string;
  type: 'warehouse' | 'area';
  children?: TreeNode[];
}

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } = useTable<LocationDto>('/api/v1/warehouse/locations');
const { formRef, formData, formRules, submitting, visible, openForm, closeForm, submitForm } = useForm<Partial<LocationDto>>({
  warehouseId: '',
  warehouseCode: '',
  areaId: '',
  areaCode: '',
  locationCode: '',
  barcodeId: '',
  locationType: 0,
  storageCondition: 0,
  maxCapacity: 0,
  maxWeight: 0,
  row: '',
  column: '',
  layer: '',
  isActive: true,
});

const warehouseTree = ref<TreeNode[]>([]);
const batchDialogVisible = ref(false);
const batchForm = reactive({
  warehouseId: '',
  warehouseCode: '',
  areaId: '',
  areaCode: '',
  prefix: '',
  startNumber: 1,
  count: 10,
});

const treeProps = {
  label: 'name',
  children: 'children',
};

const locationTypeLabels: Record<number, string> = {
  0: '普通库位',
  1: '高架库位',
  2: '地堆库位',
  3: '冷冻库位',
  4: '特殊库位',
};

formRules.value = {
  areaId: [{ required: true, message: '请选择所属库区', trigger: 'change' }],
  locationCode: [{ required: true, message: '请输入库位编码', trigger: 'blur' }],
  barcodeId: [{ required: true, message: '请输入条码ID', trigger: 'blur' }],
};

function getLocationTypeLabel(value: number): string {
  return locationTypeLabels[value] || '未知';
}

function handleAreaChange(area: { id: string; code: string; name: string; warehouseId: string; warehouseCode: string } | null) {
  if (area) {
    formData.areaId = area.id;
    formData.areaCode = area.code;
    formData.warehouseId = area.warehouseId;
    formData.warehouseCode = area.warehouseCode;
  } else {
    formData.areaId = '';
    formData.areaCode = '';
    formData.warehouseId = '';
    formData.warehouseCode = '';
  }
}

function handleBatchAreaChange(area: { id: string; code: string; name: string; warehouseId: string; warehouseCode: string } | null) {
  if (area) {
    batchForm.warehouseId = area.warehouseId;
    batchForm.warehouseCode = area.warehouseCode;
    batchForm.areaId = area.id;
    batchForm.areaCode = area.code;
  } else {
    batchForm.warehouseId = '';
    batchForm.warehouseCode = '';
    batchForm.areaId = '';
    batchForm.areaCode = '';
  }
}

async function loadTree() {
  try {
    const res = await getWarehouses({ maxResultCount: 9999, skipCount: 0 });
    const items = res.items || [];
    warehouseTree.value = [];
    for (const wh of items) {
      const areasRes = await getAreas({ warehouseId: wh.id, maxResultCount: 9999, skipCount: 0 });
      const children = (areasRes.items || []).map(area => ({
        id: area.id,
        name: area.areaName,
        type: 'area' as const,
        warehouseId: wh.id,
      }));
      warehouseTree.value.push({
        id: wh.id,
        name: wh.warehouseName,
        type: 'warehouse' as const,
        children,
      });
    }
  } catch {
    ElMessage.error('加载仓库树失败');
  }
}

function handleNodeClick(node: TreeNode) {
  if (node.type === 'area') {
    filters.areaId = node.id;
    handleSearch();
  }
}

function handleCreate() {
  openForm();
}

function handleEdit(row: LocationDto) {
  openForm({ ...row });
}

async function handleDelete(row: LocationDto) {
  try {
    await ElMessageBox.confirm('确认删除该库位？', '提示', { type: 'warning' });
    await deleteLocation(row.id);
    ElMessage.success('删除成功');
    handleSearch();
  } catch {
    // Cancel
  }
}

async function handleSubmit() {
  const success = await submitForm(async () => {
    if (formData.id) {
      const updateData = {
        locationType: formData.locationType ?? 0,
        storageCondition: formData.storageCondition ?? 0,
        maxCapacity: formData.maxCapacity,
        maxWeight: formData.maxWeight,
        row: formData.row,
        column: formData.column,
        layer: formData.layer,
        isActive: formData.isActive ?? true,
      };
      await updateLocation(formData.id, updateData);
    } else {
      const createData: CreateOrUpdateLocationDto = {
        warehouseId: formData.warehouseId || '',
        warehouseCode: formData.warehouseCode || '',
        areaId: formData.areaId || '',
        areaCode: formData.areaCode || '',
        locationCode: formData.locationCode || '',
        barcodeId: formData.barcodeId || '',
        locationType: formData.locationType ?? 0,
        storageCondition: formData.storageCondition ?? 0,
        maxCapacity: formData.maxCapacity,
        maxWeight: formData.maxWeight,
        row: formData.row,
        column: formData.column,
        layer: formData.layer,
        isActive: formData.isActive ?? true,
      };
      await createLocation(createData);
    }
  }, formData.id ? '更新成功' : '创建成功');
  if (success) handleSearch();
}

async function handleBatchSubmit() {
  try {
    const lines: CreateOrUpdateLocationDto[] = [];
    for (let i = 0; i < batchForm.count; i++) {
      const num = batchForm.startNumber + i;
      const locationCode = `${batchForm.prefix}${num}`;
      lines.push({
        warehouseId: batchForm.warehouseId,
        warehouseCode: batchForm.warehouseCode,
        areaId: batchForm.areaId,
        areaCode: batchForm.areaCode,
        locationCode,
        barcodeId: locationCode,
        isActive: true,
      });
    }
    await batchCreateLocations(lines);
    ElMessage.success('批量创建成功');
    batchDialogVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('批量创建失败');
  }
}

onMounted(() => {
  loadTree();
});

handleSearch();
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.location-layout {
  height: 100%;
}
.tree-card {
  height: 100%;
  min-height: 600px;
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