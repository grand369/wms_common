<template>
  <div class="page-container">
    <el-page-header title="返回" content="新建出库单" @back="goBack" />

    <el-card shadow="hover" class="form-card">
      <template #header>
        <span>出库单信息</span>
      </template>

      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="120px">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="出库类型" prop="outboundTypeValue">
              <el-select v-model="formData.outboundTypeValue" placeholder="请选择出库类型" style="width: 100%">
                <el-option label="生产出库" :value="1" />
                <el-option label="销售出库" :value="2" />
                <el-option label="退货出库" :value="3" />
                <el-option label="调拨出库" :value="4" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="仓库" prop="warehouseId">
              <WmsWarehouseSelector
                v-model="formData.warehouseId"
                @change="onWarehouseChange"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="紧急出库">
              <el-switch v-model="formData.isEmergency" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="超发比例">
              <el-input-number
                v-model="formData.overIssueRatio"
                :min="0"
                :max="1"
                :step="0.1"
                :precision="2"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="24">
            <el-form-item label="备注">
              <el-input
                v-model="formData.remark"
                type="textarea"
                :rows="2"
                placeholder="请输入备注"
                maxlength="1000"
                show-word-limit
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <el-divider content-position="left">出库明细</el-divider>
      <WmsOrderLineEditor v-model:lines="lines" mode="outbound" />

      <div class="form-actions">
        <el-button @click="goBack">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">保存</el-button>
        <el-button type="success" :loading="submitting" @click="handleSubmitAndAllocate">保存并分配</el-button>
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
import {
  createOutboundOrder,
  updateOutboundOrder,
  getOutboundOrder,
  allocateOutbound
} from '@/api/outbound';
import type {
  CreateOrUpdateOutboundOrderDto,
  OutboundOrderLineDto,
  OutboundAllocateCommandDto,
  OutboundOrderOutputDto
} from '@/api/outbound';
import type { WmsOrderLine } from '@/components/common/WmsOrderLineEditor.vue';
import type { WmsWarehouse } from '@/components/common/WmsWarehouseSelector.vue';
import { getFriendlyErrorMessage, parseAxiosError } from '@/utils/errorHandler';

const router = useRouter();
const route = useRoute();
const editingId = route.query.id as string | undefined;

const formRef = ref<FormInstance>();
const formData = ref<CreateOrUpdateOutboundOrderDto>({
  outboundTypeValue: 0,
  warehouseId: '',
  warehouseCode: '',
  overIssueRatio: 0,
  isEmergency: false,
  remark: '',
  lines: [],
});

const formRules: FormRules = {
  outboundTypeValue: [{ required: true, message: '请选择出库类型', trigger: 'change' }],
  warehouseId: [{ required: true, message: '请选择仓库', trigger: 'change' }],
};

const lines = ref<WmsOrderLine[]>([]);
const submitting = ref(false);

function onWarehouseChange(warehouse: WmsWarehouse | WmsWarehouse[] | null) {
  if (warehouse && !Array.isArray(warehouse)) {
    formData.value.warehouseCode = warehouse.code;
  } else {
    formData.value.warehouseCode = '';
  }
}

async function loadOrder() {
  if (!editingId) return;
  try {
    const order = await getOutboundOrder(editingId);
    formData.value = {
      outboundTypeValue: order.outboundTypeValue,
      warehouseId: order.warehouseId,
      warehouseCode: order.warehouseCode,
      overIssueRatio: 0,
      isEmergency: false,
      remark: order.remark,
      lines: [],
    };
    lines.value = (order.lines || []).map((l) => ({
      _id: l.id,
      materialId: l.materialId,
      materialCode: l.materialCode,
      materialName: l.materialName,
      quantity: l.requiredQuantity,
      unit: '',
      locationId: '',
      remarks: l.remark,
    }));
  } catch {
    ElMessage.error('加载出库单失败');
  }
}

function goBack() {
  router.push('/outbound/list');
}

function buildLines(): OutboundOrderLineDto[] {
  return lines.value
    .filter((l) => l.materialId && l.quantity > 0)
    .map((l) => ({
      id: l._id,
      materialId: l.materialId,
      materialCode: l.materialCode || '',
      materialName: l.materialName || '',
      requiredQuantity: l.quantity,
      issueStrategyValue: 0,
      batchNumber: l.batchNumber,
      remark: l.remarks,
    }));
}

function buildAllocateCommand(orderResult: OutboundOrderOutputDto): OutboundAllocateCommandDto {
  const idempotencyId = `alloc_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  const allocateLines = orderResult.lines.map((line) => ({
    lineId: line.id,
    allocatedQuantity: line.requiredQuantity,
  }));
  return {
    idempotencyId,
    lines: allocateLines,
  };
}

async function doSubmit(allocate: boolean) {
  if (!formRef.value) return;
  try {
    await formRef.value.validate();
  } catch {
    return;
  }

  if (!formData.value.warehouseCode) {
    ElMessage.warning('请先选择仓库');
    return;
  }

  const payload: CreateOrUpdateOutboundOrderDto = {
    ...formData.value,
    lines: buildLines(),
  };

  if (payload.lines.length === 0) {
    ElMessage.warning('请至少添加一行出库明细');
    return;
  }

  submitting.value = true;
  try {
    let result: OutboundOrderOutputDto;
    if (editingId) {
      result = await updateOutboundOrder(editingId, payload);
      ElMessage.success('更新成功');
    } else {
      result = await createOutboundOrder(payload);
      ElMessage.success('保存成功');
    }

    if (allocate) {
      if (result.lines && result.lines.length > 0) {
        const allocateCommand = buildAllocateCommand(result);
        await allocateOutbound(result.id, allocateCommand);
        ElMessage.success('分配成功');
      } else {
        ElMessage.warning('出库单明细为空，无法分配');
      }
    }

    router.push('/outbound/list');
  } catch (err: any) {
    const friendlyMsg = getFriendlyErrorMessage(parseAxiosError(err))
    ElMessage.error(friendlyMsg);
  } finally {
    submitting.value = false;
  }
}

function handleSubmit() {
  doSubmit(false);
}

function handleSubmitAndAllocate() {
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
