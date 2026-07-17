<template>
  <div class="page-container">
    <el-page-header title="返回" content="新建入库单" @back="goBack" />

    <el-card shadow="hover" class="form-card">
      <template #header>
        <span>入库单信息</span>
      </template>

      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="120px">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="入库类型" prop="orderType">
              <el-select v-model="formData.orderType" placeholder="请选择入库类型" style="width: 100%">
                <el-option label="采购入库" value="Purchase" />
                <el-option label="退货入库" value="Return" />
                <el-option label="生产入库" value="Production" />
                <el-option label="调拨入库" value="Transfer" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="仓库" prop="warehouseId">
              <WmsWarehouseSelector v-model="formData.warehouseId" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="供应商">
              <el-input v-model="formData.supplierId" placeholder="请输入供应商ID" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="计划日期">
              <el-date-picker v-model="formData.planDate" type="date" placeholder="请选择计划日期" format="YYYY-MM-DD" value-format="YYYY-MM-DD" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <el-divider content-position="left">入库明细</el-divider>
      <WmsOrderLineEditor v-model:lines="lines" mode="inbound" />

      <div class="form-actions">
        <el-button @click="goBack">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">保存</el-button>
        <el-button type="success" :loading="submitting" @click="handleSubmitAndConfirm">保存并确认</el-button>
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
import { createInboundOrder, updateInboundOrder, getInboundOrder, confirmInbound } from '@/api/inbound';
import type { CreateOrUpdateInboundOrderDto, InboundOrderLineDto } from '@/api/inbound';
import type { WmsOrderLine } from '@/components/common/WmsOrderLineEditor.vue';

const router = useRouter();
const route = useRoute();
const editingId = route.query.id as string | undefined;

const formRef = ref<FormInstance>();
const formData = ref<CreateOrUpdateInboundOrderDto>({
  orderType: '',
  warehouseId: '',
  supplierId: '',
  planDate: '',
  lines: [],
});
const formRules: FormRules = {
  orderType: [{ required: true, message: '请选择入库类型', trigger: 'change' }],
  warehouseId: [{ required: true, message: '请选择仓库', trigger: 'change' }],
};

const lines = ref<WmsOrderLine[]>([]);
const submitting = ref(false);

async function loadOrder() {
  if (!editingId) return;
  try {
    const order = await getInboundOrder(editingId);
    formData.value = {
      orderType: order.orderType,
      warehouseId: order.warehouseId,
      supplierId: order.supplierId,
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
      remarks: '',
    }));
  } catch {
    ElMessage.error('加载入库单失败');
  }
}

function goBack() {
  router.push('/inbound/list');
}

function buildLines(): InboundOrderLineDto[] {
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

async function doSubmit(confirm: boolean) {
  if (!formRef.value) return;
  try {
    await formRef.value.validate();
  } catch {
    return;
  }
  const payload: CreateOrUpdateInboundOrderDto = {
    ...formData.value,
    lines: buildLines(),
  };
  if (payload.lines.length === 0) {
    ElMessage.warning('请至少添加一行入库明细');
    return;
  }
  submitting.value = true;
  try {
    let result: any;
    if (editingId) {
      result = await updateInboundOrder(editingId, payload);
    } else {
      result = await createInboundOrder(payload);
    }
    if (confirm) {
      await confirmInbound(result.id);
    }
    ElMessage.success('保存成功');
    router.push('/inbound/list');
  } catch {
    ElMessage.error('保存失败');
  } finally {
    submitting.value = false;
  }
}

function handleSubmit() {
  doSubmit(false);
}

function handleSubmitAndConfirm() {
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
