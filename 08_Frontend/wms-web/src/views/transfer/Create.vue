<template>
  <div class="page-container">
    <el-page-header title="返回" :content="editingId ? '编辑调拨单' : '新建调拨单'" @back="goBack" />

    <el-card shadow="hover" class="form-card">
      <template #header>
        <span>调拨单信息</span>
      </template>

      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="120px">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="源仓库" prop="fromWarehouseId">
              <WmsWarehouseSelector v-model="formData.fromWarehouseId" placeholder="请选择源仓库" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="目标仓库" prop="toWarehouseId">
              <WmsWarehouseSelector v-model="formData.toWarehouseId" placeholder="请选择目标仓库" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="计划日期">
              <el-date-picker
                v-model="formData.planDate"
                type="date"
                placeholder="请选择计划日期"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <el-divider content-position="left">调拨明细</el-divider>
      <WmsOrderLineEditor v-model:lines="lines" mode="transfer" />

      <div class="form-actions">
        <el-button @click="goBack">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">保存</el-button>
        <el-button type="success" :loading="submitting" @click="handleSubmitAndApprove">保存并提交审批</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsWarehouseSelector from '@/components/common/WmsWarehouseSelector.vue';
import WmsOrderLineEditor from '@/components/common/WmsOrderLineEditor.vue';
import { createTransfer, updateTransfer, getTransfer, approveTransfer } from '@/api/transfer';
import type { CreateOrUpdateTransferDto, TransferLineDto } from '@/api/transfer';
import type { WmsOrderLine } from '@/components/common/WmsOrderLineEditor.vue';

const router = useRouter();
const route = useRoute();
const editingId = route.query.id as string | undefined;

const formRef = ref<FormInstance>();
const formData = ref<CreateOrUpdateTransferDto>({
  fromWarehouseId: '',
  toWarehouseId: '',
  planDate: '',
  lines: [],
});
const formRules: FormRules = {
  fromWarehouseId: [{ required: true, message: '请选择源仓库', trigger: 'change' }],
  toWarehouseId: [{ required: true, message: '请选择目标仓库', trigger: 'change' }],
};

const lines = ref<WmsOrderLine[]>([]);
const submitting = ref(false);
const createdId = ref<string | null>(null);

async function loadOrder() {
  if (!editingId) return;
  try {
    const order = await getTransfer(editingId);
    formData.value = {
      fromWarehouseId: order.fromWarehouseId,
      toWarehouseId: order.toWarehouseId,
      planDate: order.planDate,
      lines: order.lines || [],
    };
    lines.value = (order.lines || []).map((l) => ({
      materialId: l.materialId,
      materialCode: l.materialCode,
      materialName: l.materialName,
      quantity: l.qty,
      unit: '',
      locationId: '',
      remarks: l.batchNo,
    }));
  } catch {
    ElMessage.error('加载调拨单失败');
  }
}

function goBack() {
  router.push('/transfer/list');
}

function buildLines(): TransferLineDto[] {
  return lines.value
    .filter((l) => l.materialId && l.quantity > 0)
    .map((l) => ({
      materialId: l.materialId,
      materialCode: l.materialCode,
      materialName: l.materialName,
      qty: l.quantity,
      batchNo: l.remarks || undefined,
    }));
}

async function doSubmit(submitForApprove: boolean) {
  if (!formRef.value) return;
  try {
    await formRef.value.validate();
  } catch {
    return;
  }
  if (formData.value.fromWarehouseId === formData.value.toWarehouseId) {
    ElMessage.warning('源仓库与目标仓库不能相同');
    return;
  }
  const payload: CreateOrUpdateTransferDto = {
    ...formData.value,
    lines: buildLines(),
  };
  if (payload.lines.length === 0) {
    ElMessage.warning('请至少添加一行调拨明细');
    return;
  }
  submitting.value = true;
  try {
    let result: any;
    if (editingId) {
      result = await updateTransfer(editingId, payload);
    } else {
      result = await createTransfer(payload);
    }
    createdId.value = result.id || editingId;
    if (submitForApprove && result.status === 0) {
      await approveTransfer(result.id);
    }
    ElMessage.success('保存成功');
    router.push('/transfer/list');
  } catch {
    ElMessage.error('保存失败');
  } finally {
    submitting.value = false;
  }
}

function handleSubmit() {
  doSubmit(false);
}

function handleSubmitAndApprove() {
  doSubmit(true);
}

onMounted(() => {
  loadOrder();
});
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.form-card {
  margin-top: 16px;
}
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 24px;
}
</style>
