<template>
  <div class="page-container">
    <el-page-header title="返回" :content="editingId ? '编辑入库单' : '新建入库单'" @back="goBack" />

    <el-card shadow="hover" class="form-card">
      <template #header>
        <span>入库单信息</span>
      </template>

      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="120px">
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="入库类型" prop="inboundTypeValue">
              <el-select v-model="formData.inboundTypeValue" placeholder="请选择入库类型" style="width: 100%">
                <el-option label="采购入库" :value="1" />
                <el-option label="生产入库" :value="2" />
                <el-option label="退货入库" :value="3" />
                <el-option label="调拨入库" :value="4" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="仓库" prop="warehouseId">
              <WmsWarehouseSelector v-model="formData.warehouseId" @change="onWarehouseChange" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="供应商">
              <el-select
                v-model="formData.supplierId"
                placeholder="请选择供应商"
                clearable
                filterable
                style="width: 100%"
                @change="onSupplierChange"
              >
                <el-option
                  v-for="item in supplierOptions"
                  :key="item.id"
                  :label="`${item.supplierCode} - ${item.supplierName}`"
                  :value="item.id"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="采购订单号">
              <el-input v-model="formData.purchaseOrderNo" placeholder="请输入采购订单号" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="12">
            <el-form-item label="采购订单ID">
              <div class="purchase-order-id-input">
                <el-input v-model="formData.purchaseOrderId" placeholder="对接第三方系统时自动填充" readonly />
                <el-button type="primary" size="small" @click="openPurchaseOrderSelector">选择</el-button>
              </div>
              <span class="form-hint">未对接第三方系统时可留空，手动输入采购订单号即可</span>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="备注">
              <el-input v-model="formData.remark" placeholder="请输入备注" />
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
import type { CreateOrUpdateInboundOrderDto, InboundOrderLineDto, InboundConfirmCommandDto } from '@/api/inbound';
import type { WmsOrderLine } from '@/components/common/WmsOrderLineEditor.vue';
import { getActiveSuppliers } from '@/api/supplier';
import type { SupplierDto } from '@/api/supplier';

const router = useRouter();
const route = useRoute();
const editingId = route.query.id as string | undefined;

const formRef = ref<FormInstance>();
const formData = ref<CreateOrUpdateInboundOrderDto>({
  inboundTypeValue: 0,
  warehouseId: '',
  warehouseCode: '',
  supplierId: '',
  supplierName: '',
  purchaseOrderId: '',
  purchaseOrderNo: '',
  productionOrderId: '',
  returnOrderId: '',
  overReceiptRatio: 0,
  qualityInspectionRequired: true,
  remark: '',
  lines: [],
});
const formRules: FormRules = {
  inboundTypeValue: [{ required: true, message: '请选择入库类型', trigger: 'change' }],
  warehouseId: [{ required: true, message: '请选择仓库', trigger: 'change' }],
};

const lines = ref<WmsOrderLine[]>([]);
const submitting = ref(false);
const supplierOptions = ref<SupplierDto[]>([]);

async function loadOrder() {
  if (!editingId) return;
  try {
    const order = await getInboundOrder(editingId);
    formData.value = {
      inboundTypeValue: order.inboundTypeValue,
      warehouseId: order.warehouseId,
      warehouseCode: order.warehouseCode || '',
      supplierId: order.supplierId,
      supplierName: order.supplierName,
      purchaseOrderId: order.purchaseOrderId,
      purchaseOrderNo: order.purchaseOrderNo,
      productionOrderId: order.productionOrderId,
      returnOrderId: order.returnOrderId,
      overReceiptRatio: order.overReceiptRatio || 0,
      qualityInspectionRequired: order.qualityInspectionRequired !== false,
      remark: order.remark,
      lines: order.lines || [],
    };
    
    // 直接使用后端返回的字段映射行数据
    lines.value = (order.lines || []).map((l) => ({
      materialId: l.materialId,
      materialCode: l.materialCode || '',
      materialName: l.materialName || '',
      quantity: l.planQuantity || 0,
      unit: l.unit || '',
      warehouseId: l.putawayWarehouseId || '',
      warehouseCode: l.putawayWarehouseCode || '',
      areaId: l.putawayAreaId || '',
      areaCode: l.putawayAreaCode || '',
      locationId: l.putawayLocationId || '',
      locationCode: l.putawayLocationCode || '',
      remarks: l.remark || '',
    }));
  } catch {
    ElMessage.error('加载入库单失败');
  }
}

function goBack() {
  router.push('/inbound/list');
}

function onWarehouseChange(warehouse: any) {
  if (warehouse) {
    formData.value.warehouseCode = warehouse.code || '';
  }
}

async function loadSuppliers() {
  try {
    const res = await getActiveSuppliers();
    supplierOptions.value = res.items || [];
  } catch {
    ElMessage.error('加载供应商列表失败');
  }
}

function onSupplierChange(supplierId: string) {
  const supplier = supplierOptions.value.find(s => s.id === supplierId);
  if (supplier) {
    formData.value.supplierName = supplier.supplierName;
  } else {
    formData.value.supplierName = '';
  }
}

function openPurchaseOrderSelector() {
  ElMessage.info('采购订单选择功能将在对接第三方系统后启用');
}

function buildLines(): InboundOrderLineDto[] {
  return lines.value
    .filter((l) => l.materialId && l.quantity > 0)
    .map((l, index) => ({
      lineNo: index + 1,
      materialId: l.materialId,
      materialCode: l.materialCode || '',
      materialName: l.materialName || '',
      unit: l.unit || '',
      planQuantity: l.quantity,
      receivedQuantity: 0,
      putawayWarehouseId: l.warehouseId || undefined,
      putawayWarehouseCode: l.warehouseCode || undefined,
      putawayAreaId: l.areaId || undefined,
      putawayAreaCode: l.areaCode || undefined,
      putawayLocationId: l.locationId || undefined,
      putawayLocationCode: l.locationCode || undefined,
      batchNumber: l.batchNumber,
      expiryDate: l.expiryDate,
      productionDate: l.productionDate,
      remark: l.remarks || undefined,
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
  debugger
  if (payload.lines.length === 0) {
    ElMessage.warning('请至少添加一行入库明细');
    return;
  }
  if (!payload.warehouseCode) {
    ElMessage.warning('请选择仓库');
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
    if (confirm && result?.id) {
      const confirmData: InboundConfirmCommandDto = {
        idempotencyId: Date.now().toString(),
        lines: payload.lines.map((l, index) => ({
          lineId: result.lines?.[index]?.id || '',
          receivedQuantity: l.planQuantity,
          batchNumber: l.batchNumber,
        })),
      };
      await confirmInbound(result.id, confirmData);
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
  loadSuppliers();
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
.purchase-order-id-input {
  display: flex;
  gap: 8px;
}
.form-hint {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
  display: block;
}
</style>
