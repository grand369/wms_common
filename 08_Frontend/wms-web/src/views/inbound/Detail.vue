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
            <el-button v-if="order?.inboundStatusValue === 2" type="success" @click="handlePutaway">上架</el-button>
            <el-button v-if="order?.inboundStatusValue === 2" type="primary" @click="handleComplete">完成</el-button>
            <el-button v-if="order && order.inboundStatusValue < 3" type="danger" @click="handleCancel">取消</el-button>
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsSteps from '@/components/common/WmsSteps.vue';
import WmsTimeline from '@/components/common/WmsTimeline.vue';
import { getInboundOrder, qualityInspectInbound, putawayInbound, completeInbound, cancelInbound } from '@/api/inbound';
import type { InboundOrderDetailDto } from '@/api/inbound';
import type { WmsTimelineItem } from '@/components/common/WmsTimeline.vue';

const route = useRoute();
const router = useRouter();
const orderId = route.params.id as string;

const order = ref<InboundOrderDetailDto | null>(null);
const loading = ref(false);
const inboundSteps = ['草稿', '已确认', '质检', '上架', '完成'];

const activeStep = computed(() => {
  if (!order.value) return 0;
  const map: Record<number, number> = { 0: 0, 1: 1, 2: 3, 3: 4, 4: 0 };
  return map[order.value.inboundStatusValue] || 0;
});

const timelineItems = computed<WmsTimelineItem[]>(() => {
  if (!order.value) return [];
  const items: WmsTimelineItem[] = [];
  items.push({ time: order.value.creationTime || '', status: 'Draft', description: '创建入库单', operator: '' });
  if (order.value.inboundStatusValue >= 1) {
    items.push({ time: '', status: 'Confirmed', description: '入库单已确认', operator: '' });
  }
  if (order.value.inboundStatusValue >= 2) {
    items.push({ time: '', status: 'InProgress', description: '入库单质检/上架中', operator: '' });
  }
  if (order.value.inboundStatusValue === 3) {
    items.push({ time: '', status: 'Completed', description: '入库单已完成', operator: '' });
  }
  if (order.value.inboundStatusValue === 4) {
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
  const map: Record<number, string> = { 0: 'Draft', 1: 'Confirmed', 2: 'InProgress', 3: 'Completed', 4: 'Cancelled' };
  return map[status] || 'Draft';
}

function goBack() {
  router.push('/inbound/list');
}

async function handleQualityInspect() {
  try {
    const passed = await ElMessageBox.confirm('质检是否通过？', '质检', {
      confirmButtonText: '通过',
      cancelButtonText: '不通过',
      type: 'warning',
    }).then(() => true).catch(() => false);
    await qualityInspectInbound(orderId, { passed });
    ElMessage.success('质检已记录');
    loadOrder();
  } catch {
    ElMessage.error('操作失败');
  }
}

async function handlePutaway() {
  try {
    await putawayInbound(orderId);
    ElMessage.success('上架成功');
    loadOrder();
  } catch {
    ElMessage.error('上架失败');
  }
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
