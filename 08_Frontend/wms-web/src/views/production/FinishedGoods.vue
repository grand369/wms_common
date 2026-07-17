<template>
  <div class="page-container">
    <el-page-header title="返回" content="成品入库" @back="goBack" />

    <el-card shadow="hover" class="form-card">
      <template #header>
        <span>成品入库信息</span>
      </template>

      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="120px">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="工单号" prop="workOrderId">
              <el-input v-model="formData.workOrderId" placeholder="请输入工单号" />
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
            <el-form-item label="入库日期">
              <el-date-picker v-model="formData.inboundDate" type="date" placeholder="请选择入库日期" format="YYYY-MM-DD" value-format="YYYY-MM-DD" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <el-divider content-position="left">成品明细</el-divider>
      <WmsOrderLineEditor v-model:lines="lines" mode="inbound" />

      <div class="form-actions">
        <el-button @click="goBack">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">保存</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsWarehouseSelector from '@/components/common/WmsWarehouseSelector.vue';
import WmsOrderLineEditor from '@/components/common/WmsOrderLineEditor.vue';
import { createFinishedGoodsInbound } from '@/api/production';
import type { CreateFinishedGoodsDto, FinishedGoodsLineDto } from '@/api/production';
import type { WmsOrderLine } from '@/components/common/WmsOrderLineEditor.vue';

const router = useRouter();

const formRef = ref<FormInstance>();
const formData = ref<CreateFinishedGoodsDto>({
  workOrderId: '',
  warehouseId: '',
  inboundDate: '',
  lines: [],
});
const formRules: FormRules = {
  warehouseId: [{ required: true, message: '请选择仓库', trigger: 'change' }],
};

const lines = ref<WmsOrderLine[]>([]);
const submitting = ref(false);

function goBack() {
  router.push('/production');
}

function buildLines(): FinishedGoodsLineDto[] {
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

async function handleSubmit() {
  if (!formRef.value) return;
  try {
    await formRef.value.validate();
  } catch {
    return;
  }
  const payload: CreateFinishedGoodsDto = {
    ...formData.value,
    lines: buildLines(),
  };
  if (payload.lines.length === 0) {
    ElMessage.warning('请至少添加一行成品明细');
    return;
  }
  submitting.value = true;
  try {
    await createFinishedGoodsInbound(payload);
    ElMessage.success('保存成功');
    router.push('/production');
  } catch {
    ElMessage.error('保存失败');
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped lang="scss">
.page-container { padding: 0; }
.form-card { margin-top: 16px; }
.form-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 24px; }
</style>
