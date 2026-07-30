<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="编码">
        <el-input v-model="filters.supplierCode" placeholder="请输入供应商编码" clearable />
      </el-form-item>
      <el-form-item label="名称">
        <el-input v-model="filters.supplierName" placeholder="请输入供应商名称" clearable />
      </el-form-item>
      <el-form-item label="类型">
        <el-select v-model="filters.supplierType" placeholder="请选择类型" clearable>
          <el-option label="普通供应商" :value="1" />
          <el-option label="战略供应商" :value="2" />
          <el-option label="委外加工商" :value="3" />
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
          <span>供应商列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建供应商
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
          <el-table-column prop="supplierCode" label="供应商编码" sortable />
          <el-table-column prop="supplierName" label="供应商名称" show-overflow-tooltip />
          <el-table-column prop="shortName" label="简称" show-overflow-tooltip />
          <el-table-column prop="supplierTypeDescription" label="类型" />
          <el-table-column prop="contactName" label="联系人" />
          <el-table-column prop="contactPhone" label="联系电话" />
          <el-table-column prop="isActive" label="状态" align="center" width="90">
            <template #default="{ row }">
              <WmsStatusTag :status="row.isActive ? 'Available' : 'Outsourced'" type="inventory" />
            </template>
          </el-table-column>
        <el-table-column label="操作" width="240" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row as SupplierDto)">编辑</el-button>
            <el-button link :type="row.isActive ? 'danger' : 'success'" @click="handleToggleStatus(row as SupplierDto)">
              {{ row.isActive ? '停用' : '启用' }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row as SupplierDto)">删除</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      :title="formData.id ? '编辑供应商' : '新建供应商'"
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
            <el-form-item label="供应商编码" prop="supplierCode" v-if="!formData.id">
              <el-input v-model="formData.supplierCode" placeholder="请输入供应商编码" />
            </el-form-item>
            <el-form-item label="供应商编码" v-else>
              <el-input :value="formData.supplierCode" disabled />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="供应商名称" prop="supplierName">
              <el-input v-model="formData.supplierName" placeholder="请输入供应商名称" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="简称">
              <el-input v-model="formData.shortName" placeholder="请输入简称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="供应商类型">
              <el-select v-model="formData.supplierType" placeholder="请选择类型">
                <el-option label="普通供应商" :value="1" />
                <el-option label="战略供应商" :value="2" />
                <el-option label="委外加工商" :value="3" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="联系人">
              <el-input v-model="formData.contactName" placeholder="请输入联系人" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话">
              <el-input v-model="formData.contactPhone" placeholder="请输入联系电话" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="联系邮箱">
              <el-input v-model="formData.contactEmail" placeholder="请输入联系邮箱" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="税号">
              <el-input v-model="formData.taxId" placeholder="请输入税号" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="开户行">
              <el-input v-model="formData.bankName" placeholder="请输入开户行" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="银行账号">
              <el-input v-model="formData.bankAccount" placeholder="请输入银行账号" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="地址">
              <el-input v-model="formData.address" placeholder="请输入地址" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="城市">
              <el-input v-model="formData.city" placeholder="请输入城市" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="省份">
              <el-input v-model="formData.province" placeholder="请输入省份" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="邮政编码">
              <el-input v-model="formData.postalCode" placeholder="请输入邮政编码" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="ERP编码">
              <el-input v-model="formData.erpSupplierCode" placeholder="请输入ERP供应商编码" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态">
              <el-switch v-model="formData.isActive" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <el-form-item label="备注">
              <el-input v-model="formData.remark" type="textarea" :rows="3" placeholder="请输入备注" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import { getSuppliers, createSupplier, updateSupplier, deleteSupplier, enableSupplier, disableSupplier } from '@/api/supplier';
import type { SupplierDto, CreateSupplierDto, UpdateSupplierDto } from '@/api/supplier';

const loading = ref(false);
const tableData = ref<SupplierDto[]>([]);
const total = ref(0);
const visible = ref(false);
const submitting = ref(false);

const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  pageSizes: [10, 20, 50, 100],
});

const filters = reactive({
  supplierCode: '',
  supplierName: '',
  supplierType: undefined as number | undefined,
  isActive: undefined as boolean | undefined,
});

const formRef = ref<FormInstance>();
const formData = reactive<Partial<CreateSupplierDto & UpdateSupplierDto> & { id?: string }>({
  id: undefined,
  supplierCode: '',
  supplierName: '',
  shortName: '',
  supplierType: 1,
  contactName: '',
  contactPhone: '',
  contactEmail: '',
  address: '',
  city: '',
  province: '',
  postalCode: '',
  taxId: '',
  bankName: '',
  bankAccount: '',
  isActive: true,
  remark: '',
  erpSupplierCode: '',
});

const formRules: FormRules = {
  supplierCode: [{ required: true, message: '请输入供应商编码', trigger: 'blur' }],
  supplierName: [{ required: true, message: '请输入供应商名称', trigger: 'blur' }],
};

async function loadData() {
  loading.value = true;
  try {
    const params = {
      ...filters,
      skipCount: (pagination.currentPage - 1) * pagination.pageSize,
      maxResultCount: pagination.pageSize,
    };
    const res = await getSuppliers(params);
    tableData.value = res.items;
    total.value = res.totalCount;
  } catch {
    ElMessage.error('加载失败');
  } finally {
    loading.value = false;
  }
}

function handleSearch() {
  pagination.currentPage = 1;
  loadData();
}

function resetFilters() {
  filters.supplierCode = '';
  filters.supplierName = '';
  filters.supplierType = undefined;
  filters.isActive = undefined;
  pagination.currentPage = 1;
  loadData();
}

function handlePageChange() {
  loadData();
}

function handleSizeChange() {
  pagination.currentPage = 1;
  loadData();
}

function handleCreate() {
  Object.assign(formData, {
    id: undefined,
    supplierCode: '',
    supplierName: '',
    shortName: '',
    supplierType: 1,
    contactName: '',
    contactPhone: '',
    contactEmail: '',
    address: '',
    city: '',
    province: '',
    postalCode: '',
    taxId: '',
    bankName: '',
    bankAccount: '',
    isActive: true,
    remark: '',
    erpSupplierCode: '',
  });
  visible.value = true;
}

function handleEdit(row: SupplierDto) {
  Object.assign(formData, {
    id: row.id,
    supplierCode: row.supplierCode,
    supplierName: row.supplierName,
    shortName: row.shortName,
    supplierType: row.supplierType,
    contactName: row.contactName,
    contactPhone: row.contactPhone,
    contactEmail: row.contactEmail,
    address: row.address,
    city: row.city,
    province: row.province,
    postalCode: row.postalCode,
    taxId: row.taxId,
    bankName: row.bankName,
    bankAccount: row.bankAccount,
    isActive: row.isActive,
    remark: row.remark,
    erpSupplierCode: row.erpSupplierCode,
  });
  visible.value = true;
}

function prepareFormData() {
  const data: any = { ...formData };
  // Convert empty strings to null for optional fields
  const optionalFields = [
    'contactEmail', 'contactPhone', 'contactName', 'shortName',
    'address', 'city', 'province', 'postalCode', 'taxId',
    'bankName', 'bankAccount', 'remark', 'erpSupplierCode'
  ];
  optionalFields.forEach(field => {
    if (data[field] === '' || data[field] === undefined) {
      data[field] = null;
    }
  });
  return data;
}

async function handleSubmit() {
  if (!formRef.value) return;
  try {
    await formRef.value.validate();
  } catch {
    return;
  }
  submitting.value = true;
  try {
    const data = prepareFormData();
    if (formData.id) {
      await updateSupplier(formData.id, data as UpdateSupplierDto);
    } else {
      await createSupplier(data as CreateSupplierDto);
    }
    ElMessage.success('保存成功');
    visible.value = false;
    loadData();
  } catch {
    ElMessage.error('保存失败');
  } finally {
    submitting.value = false;
  }
}

function closeForm() {
  visible.value = false;
}

async function handleToggleStatus(row: SupplierDto) {
  try {
    if (row.isActive) {
      await disableSupplier(row.id);
      ElMessage.success('已停用');
    } else {
      await enableSupplier(row.id);
      ElMessage.success('已启用');
    }
    loadData();
  } catch {
    ElMessage.error('操作失败');
  }
}

async function handleDelete(row: SupplierDto) {
  try {
    await ElMessageBox.confirm('确定要删除该供应商吗？', '提示', { type: 'warning' });
    await deleteSupplier(row.id);
    ElMessage.success('删除成功');
    loadData();
  } catch {
    // 用户取消或操作失败
  }
}

loadData();
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
  gap: 12px;
}
</style>
