<template>
  <div class="page-container">
    <el-page-header title="返回" content="入库单详情" @back="goBack" />

    <el-card shadow="hover" class="detail-card">
      <template #header>
        <div class="card-header">
          <span>入库单号：{{ order?.inboundOrderNo }}</span>
          <div class="header-actions">
            <WmsStatusTag v-if="order" :status="mapDocumentStatus(order.inboundStatusValue)" type="document" />
            <el-button v-if="order?.inboundStatusValue === 1" type="warning" @click="handleQualityInspect">质检</el-button>
            <el-button v-if="order?.inboundStatusValue === 4" type="success" @click="showPutawayDialog = true">上架</el-button>
            <!--<el-button v-if="order?.inboundStatusValue === 4" type="primary" @click="handleComplete">完成</el-button>-->
            <el-button v-if="order && (order.inboundStatusValue === 0 || order.inboundStatusValue === 1)" type="danger" @click="handleCancel">取消</el-button>
          </div>
        </div>
      </template>

      <WmsSteps :steps="inboundSteps" :active-step="activeStep" />

      <el-descriptions :column="3" border class="order-info">
        <el-descriptions-item label="入库类型">{{ order?.inboundTypeName }}</el-descriptions-item>
        <el-descriptions-item label="仓库">{{ order?.warehouseCode }}</el-descriptions-item>
        <el-descriptions-item label="供应商">{{ order?.supplierName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="采购订单号">{{ order?.purchaseOrderNo || '-' }}</el-descriptions-item>
        <el-descriptions-item label="计划数量">{{ order?.totalPlanQuantity }}</el-descriptions-item>
        <el-descriptions-item label="已收数量">{{ order?.totalReceivedQuantity }}</el-descriptions-item>
        <el-descriptions-item label="备注">{{ order?.remark || '-' }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ order?.creationTime }}</el-descriptions-item>
      </el-descriptions>

      <el-divider content-position="left">入库明细</el-divider>
      <el-table :data="order?.lines" border>
        <el-table-column type="index" width="50" />
        <el-table-column prop="lineNo" label="行号" width="80" />
        <el-table-column prop="materialCode" label="物料编码" />
        <el-table-column prop="materialName" label="物料名称" />
        <el-table-column prop="planQuantity" label="计划数量" align="right" />
        <el-table-column prop="receivedQuantity" label="已收数量" align="right" />
        <el-table-column prop="batchNumber" label="批次" />
        <el-table-column prop="qualityStatusName" label="质检状态" />
        <el-table-column prop="remark" label="备注" />
      </el-table>

      <el-divider content-position="left">状态记录</el-divider>
      <WmsTimeline :items="timelineItems" status-type="document" />
    </el-card>

    <!-- 质检弹窗 -->
    <el-dialog v-model="showQualityInspectDialog" title="质检确认" width="800px" :close-on-click-modal="false">
      <el-form :model="qualityInspectForm" label-width="100px">
        <el-table :data="qualityInspectLines" border>
          <el-table-column prop="lineNo" label="行号" width="80" />
          <el-table-column prop="materialCode" label="物料编码" />
          <el-table-column prop="materialName" label="物料名称" />
          <el-table-column prop="receivedQuantity" label="已收数量" align="right" />
          <el-table-column prop="batchNumber" label="批次" />
          <el-table-column label="质检结果" width="180">
            <template #default="scope">
              <el-select
                v-model="scope.row.qualityResultValue"
                placeholder="请选择质检结果"
                class="w-full"
              >
                <el-option label="合格" :value="1" />
                <el-option label="不合格" :value="2" />
                <el-option label="跳过" :value="3" />
              </el-select>
            </template>
          </el-table-column>
        </el-table>
      </el-form>
      <template #footer>
        <el-button @click="showQualityInspectDialog = false">取消</el-button>
        <el-button type="danger" @click="confirmQualityInspect(false)">不通过</el-button>
        <el-button type="primary" @click="confirmQualityInspect(true)">通过</el-button>
      </template>
    </el-dialog>

    <!-- 上架弹窗 -->
    <el-dialog v-model="showPutawayDialog" title="上架确认" width="800px" :close-on-click-modal="false">
      <el-form :model="putawayForm" label-width="100px">
        <el-table :data="putawayLines" border>
          <el-table-column prop="lineNo" label="行号" width="80" />
          <el-table-column prop="materialCode" label="物料编码" />
          <el-table-column prop="materialName" label="物料名称" />
          <el-table-column prop="receivedQuantity" label="已收数量" align="right" />
          <el-table-column label="上架库位" min-width="200">
            <template #default="scope">
              <wms-location-selector
                v-model="scope.row.putawayLocationId"
                :warehouse-id="scope.row.putawayWarehouseId"
                :area-id="scope.row.putawayAreaId"
                @warehouse-change="(w: any) => onPutawayWarehouseChange(scope.$index, w)"
                @area-change="(a: any) => onPutawayAreaChange(scope.$index, a)"
                @change="(l: any) => onPutawayLocationChange(scope.$index, l)"
              />
            </template>
          </el-table-column>
          <el-table-column label="上架数量" width="120">
            <template #default="scope">
              <el-input-number
                v-model="scope.row.quantity"
                :min="0"
                :max="scope.row.receivedQuantity"
                :precision="4"
              />
            </template>
          </el-table-column>
        </el-table>
      </el-form>
      <template #footer>
        <el-button @click="showPutawayDialog = false">取消</el-button>
        <el-button type="primary" @click="confirmPutaway">确认上架</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsSteps from '@/components/common/WmsSteps.vue';
import WmsTimeline from '@/components/common/WmsTimeline.vue';
import WmsLocationSelector from '@/components/common/WmsLocationSelector.vue';
import { getInboundOrder, qualityInspectInbound, putawayInbound, completeInbound, cancelInbound } from '@/api/inbound';
import type { InboundOrderDetailDto, InboundPutawayCommandDto, InboundQualityInspectCommandDto } from '@/api/inbound';
import type { WmsTimelineItem } from '@/components/common/WmsTimeline.vue';

const route = useRoute();
const router = useRouter();
const orderId = route.params.id as string;

const order = ref<InboundOrderDetailDto | null>(null);
const loading = ref(false);
const inboundSteps = ['草稿', '已确认', '质检', '上架', '完成'];

const showQualityInspectDialog = ref(false);
const qualityInspectLines = ref<any[]>([]);
const qualityInspectForm = ref({});

const showPutawayDialog = ref(false);
const putawayLines = ref<any[]>([]);
const putawayForm = ref({});

const activeStep = computed(() => {
  if (!order.value) return 0;
  const map: Record<number, number> = { 
    0: 0, // Draft
    1: 1, // Confirmed
    2: 2, // Inspecting
    3: 2, // Isolated (still in quality inspection phase)
    4: 3, // Putaway
    5: 4, // Completed
    6: 4, // Closed
    7: 0  // Cancelled
  };
  return map[order.value.inboundStatusValue] || 0;
});

const timelineItems = computed<WmsTimelineItem[]>(() => {
  if (!order.value) return [];
  const items: WmsTimelineItem[] = [];
  const status = order.value.inboundStatusValue;
  
  // 创建入库单
  items.push({ time: order.value.creationTime || '', status: 'Draft', description: '创建入库单', operator: '' });
  
  // 入库单已确认
  if (status >= 1) {
    items.push({ time: '', status: 'Confirmed', description: '入库单已确认', operator: '' });
  }
  
  // 质检环节
  if (status >= 2) {
    if (status === 3) {
      items.push({ time: '', status: 'InProgress', description: '入库单已隔离', operator: '' });
    } else {
      items.push({ time: '', status: 'Confirmed', description: '质检通过', operator: '' });
    }
  }
  
  // 上架环节
  if (status >= 4) {
    items.push({ time: '', status: 'Confirmed', description: '上架完成', operator: '' });
  }
  
  // 已完成
  if (status === 5) {
    items.push({ time: '', status: 'Completed', description: '入库单已完成', operator: '' });
  }
  
  // 已关闭
  if (status === 6) {
    items.push({ time: '', status: 'Completed', description: '入库单已关闭', operator: '' });
  }
  
  // 已取消
  if (status === 7) {
    items.push({ time: '', status: 'Cancelled', description: '入库单已取消', operator: '' });
  }
  
  return items;
});

async function loadOrder() {
  loading.value = true;
  try {
    order.value = await getInboundOrder(orderId);
  } catch {
    ElMessage.error('加载入库单失败');
  } finally {
    loading.value = false;
  }
}

function mapDocumentStatus(status: number) {
  const map: Record<number, string> = { 
    0: 'Draft', 
    1: 'Confirmed', 
    2: 'Inspecting', 
    3: 'Isolated', 
    4: 'Putaway', 
    5: 'Completed', 
    6: 'Closed', 
    7: 'Cancelled' 
  };
  return map[status] || 'Draft';
}

function goBack() {
  router.push('/inbound/list');
}

function handleQualityInspect() {
  if (!order.value) return;
  qualityInspectLines.value = (order.value.lines || []).map((l) => ({
    lineId: l.id || '',
    lineNo: l.lineNo,
    materialCode: l.materialCode,
    materialName: l.materialName,
    receivedQuantity: l.receivedQuantity,
    batchNumber: l.batchNumber,
    qualityResultValue: 1,
  }));
  showQualityInspectDialog.value = true;
}

async function confirmQualityInspect(passed: boolean) {
  const validLines = qualityInspectLines.value.filter((l) => l.lineId);

  if (validLines.length === 0) {
    ElMessage.warning('没有可质检的行');
    return;
  }

  try {
    const data: InboundQualityInspectCommandDto = {
      idempotencyId: Date.now().toString(),
      lines: validLines.map((l) => ({
        lineId: l.lineId,
        qualityResultValue: passed ? 1 : 2,
      })),
    };

    await qualityInspectInbound(orderId, data);
    ElMessage.success('质检已记录');
    showQualityInspectDialog.value = false;
    loadOrder();
  } catch {
    ElMessage.error('操作失败');
  }
}

function openPutawayDialog() {
  if (!order.value) return;
  putawayLines.value = (order.value.lines || []).map((l) => ({
    lineId: l.id || '',
    lineNo: l.lineNo,
    materialCode: l.materialCode,
    materialName: l.materialName,
    receivedQuantity: l.receivedQuantity,
    quantity: l.receivedQuantity,
    putawayWarehouseId: l.putawayWarehouseId || order.value.warehouseId,
    putawayWarehouseCode: l.putawayWarehouseCode || order.value.warehouseCode,
    putawayAreaId: l.putawayAreaId || '',
    putawayAreaCode: l.putawayAreaCode || '',
    putawayLocationId: l.putawayLocationId || '',
    putawayLocationCode: l.putawayLocationCode || '',
  }));
  showPutawayDialog.value = true;
}

function onPutawayWarehouseChange(index: number, warehouse: any) {
  if (warehouse) {
    putawayLines.value[index].putawayWarehouseId = warehouse.id;
    putawayLines.value[index].putawayWarehouseCode = warehouse.code;
  } else {
    putawayLines.value[index].putawayWarehouseId = '';
    putawayLines.value[index].putawayWarehouseCode = '';
  }
  putawayLines.value[index].putawayAreaId = '';
  putawayLines.value[index].putawayAreaCode = '';
  putawayLines.value[index].putawayLocationId = '';
  putawayLines.value[index].putawayLocationCode = '';
}

function onPutawayAreaChange(index: number, area: any) {
  if (area) {
    putawayLines.value[index].putawayAreaId = area.id;
    putawayLines.value[index].putawayAreaCode = area.code;
  } else {
    putawayLines.value[index].putawayAreaId = '';
    putawayLines.value[index].putawayAreaCode = '';
  }
  putawayLines.value[index].putawayLocationId = '';
  putawayLines.value[index].putawayLocationCode = '';
}

function onPutawayLocationChange(index: number, location: any) {
  if (location) {
    putawayLines.value[index].putawayLocationId = location.id;
    putawayLines.value[index].putawayLocationCode = location.code;
  } else {
    putawayLines.value[index].putawayLocationId = '';
    putawayLines.value[index].putawayLocationCode = '';
  }
}

async function confirmPutaway() {
  const validLines = putawayLines.value.filter(
    (l) => l.lineId && l.putawayLocationId && l.quantity > 0
  );

  if (validLines.length === 0) {
    ElMessage.warning('请选择上架库位并输入上架数量');
    return;
  }

  try {
    const data: InboundPutawayCommandDto = {
      idempotencyId: Date.now().toString(),
      lines: validLines.map((l) => ({
        lineId: l.lineId,
        putawayWarehouseId: l.putawayWarehouseId,
        putawayWarehouseCode: l.putawayWarehouseCode,
        putawayAreaId: l.putawayAreaId,
        putawayAreaCode: l.putawayAreaCode,
        putawayLocationId: l.putawayLocationId,
        putawayLocationCode: l.putawayLocationCode,
        quantity: l.quantity,
      })),
    };

    await putawayInbound(orderId, data);
    ElMessage.success('上架成功');
    showPutawayDialog.value = false;
    loadOrder();
  } catch {
    ElMessage.error('上架失败');
  }
}

async function handlePutaway() {
  openPutawayDialog();
}

async function handleComplete() {
  try {
    await completeInbound(orderId);
    ElMessage.success('入库单已完成');
    loadOrder();
  } catch {
    ElMessage.error('完成失败');
  }
}

async function handleCancel() {
  try {
    await ElMessageBox.confirm('确认取消该入库单？', '提示', { type: 'warning' });
    await cancelInbound(orderId, { reason: '手动取消' });
    ElMessage.success('取消成功');
    loadOrder();
  } catch {
    ElMessage.error('取消失败');
  }
}

watch(showPutawayDialog, (val) => {
  if (val) {
    openPutawayDialog();
  }
});

onMounted(() => {
  loadOrder();
});
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.detail-card {
  margin-top: 16px;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.header-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}
.order-info {
  margin-top: 16px;
}
</style>
