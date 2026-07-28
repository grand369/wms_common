<template>
  <div class="page-container">
    <el-page-header title="返回" content="出库单详情" @back="goBack" />

    <el-card shadow="hover" class="detail-card">
      <template #header>
        <div class="card-header">
          <span>出库单号：{{ order?.orderNo }}</span>
          <div class="header-actions">
            <WmsStatusTag v-if="order" :status="mapDocumentStatus(order.status)" type="document" />
            <el-button v-if="order?.status === 0" type="primary" @click="handleAllocate">分配库存</el-button>
            <el-button v-if="order?.status === 1" type="warning" @click="handlePick">拣货</el-button>
            <el-button v-if="order?.status === 2" type="success" @click="handleShip">发货</el-button>
            <el-button v-if="order?.status === 2" type="primary" @click="handleComplete">完成</el-button>
            <el-button v-if="order && order.status < 3" type="danger" @click="handleCancel">取消</el-button>
          </div>
        </div>
      </template>

      <WmsSteps :steps="outboundSteps" :active-step="activeStep" />

      <el-descriptions :column="3" border class="order-info">
        <el-descriptions-item label="出库类型">{{ order?.orderType }}</el-descriptions-item>
        <el-descriptions-item label="仓库">{{ order?.warehouseName }}</el-descriptions-item>
        <el-descriptions-item label="客户">{{ order?.customerName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="计划数量">{{ order?.totalRequiredQuantity }}</el-descriptions-item>
        <el-descriptions-item label="已分配数量">{{ order?.totalAllocatedQuantity }}</el-descriptions-item>
        <el-descriptions-item label="已拣货数量">{{ order?.totalPickedQuantity }}</el-descriptions-item>
        <el-descriptions-item label="已发货数量">{{ order?.totalShippedQuantity }}</el-descriptions-item>
        <el-descriptions-item label="备注">{{ order?.remark || '-' }}</el-descriptions-item>
      </el-descriptions>

      <el-divider content-position="left">出库明细</el-divider>
      <el-table :data="order?.lines" border>
        <el-table-column type="index" width="50" />
        <el-table-column prop="lineNo" label="行号" width="80" />
        <el-table-column prop="materialCode" label="物料编码" />
        <el-table-column prop="materialName" label="物料名称" />
        <el-table-column prop="requiredQuantity" label="需求数量" align="right" />
        <el-table-column prop="allocatedQuantity" label="已分配" align="right" />
        <el-table-column prop="pickedQuantity" label="已拣货" align="right" />
        <el-table-column prop="shippedQuantity" label="已发货" align="right" />
        <el-table-column prop="batchNumber" label="批次" />
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
import { getOutboundOrder, allocateOutbound, pickOutbound, shipOutbound, completeOutbound, cancelOutbound } from '@/api/outbound';
import type { OutboundOrderDetailDto } from '@/api/outbound';
import type { WmsTimelineItem } from '@/components/common/WmsTimeline.vue';

const route = useRoute();
const router = useRouter();
const orderId = route.params.id as string;

const order = ref<OutboundOrderDetailDto | null>(null);
const loading = ref(false);
const outboundSteps = ['草稿', '已分配', '拣货', '发货', '完成'];

const activeStep = computed(() => {
  if (!order.value) return 0;
  const map: Record<number, number> = { 0: 0, 1: 1, 2: 2, 3: 3, 4: 4, 5: 0 };
  return map[order.value.status] || 0;
});

const timelineItems = computed<WmsTimelineItem[]>(() => {
  if (!order.value) return [];
  const items: WmsTimelineItem[] = [];
  items.push({ time: order.value.creationTime || '', status: 'Draft', description: '创建出库单', operator: '' });
  if (order.value.status >= 1) {
    items.push({ time: '', status: 'Confirmed', description: '出库单已分配', operator: '' });
  }
  if (order.value.status >= 2) {
    items.push({ time: '', status: 'InProgress', description: '出库单拣货中', operator: '' });
  }
  if (order.value.status >= 3) {
    items.push({ time: '', status: 'InProgress', description: '出库单发货中', operator: '' });
  }
  if (order.value.status === 4) {
    items.push({ time: '', status: 'Completed', description: '出库单已完成', operator: '' });
  }
  if (order.value.status === 5) {
    items.push({ time: '', status: 'Cancelled', description: '出库单已取消', operator: '' });
  }
  return items;
});

async function loadOrder() {
  loading.value = true;
  try {
    order.value = await getOutboundOrder(orderId);
  } catch {
    ElMessage.error('加载出库单失败');
  } finally {
    loading.value = false;
  }
}

function mapDocumentStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'Allocated', 2: 'Picking', 3: 'Shipped', 4: 'Completed', 5: 'Cancelled' };
  return map[status] || 'Draft';
}

function goBack() {
  router.push('/outbound/list');
}

async function handleAllocate() {
  try {
    await allocateOutbound(orderId);
    ElMessage.success('库存分配成功');
    loadOrder();
  } catch {
    ElMessage.error('分配失败');
  }
}

async function handlePick() {
  try {
    await pickOutbound(orderId, { lines: (order.value?.lines || []).map((l) => ({ lineId: l.id || '', pickedQty: l.allocatedQuantity })) });
    ElMessage.success('拣货成功');
    loadOrder();
  } catch {
    ElMessage.error('拣货失败');
  }
}

async function handleShip() {
  try {
    const trackingNo = await ElMessageBox.prompt('请输入物流单号', '发货', { inputValue: '' }).then(({ value }) => value).catch(() => '');
    await shipOutbound(orderId, { trackingNo });
    ElMessage.success('发货成功');
    loadOrder();
  } catch {
    ElMessage.error('发货失败');
  }
}

async function handleComplete() {
  try {
    await completeOutbound(orderId);
    ElMessage.success('出库单已完成');
    loadOrder();
  } catch {
    ElMessage.error('完成失败');
  }
}

async function handleCancel() {
  try {
    await ElMessageBox.confirm('确认取消该出库单？', '提示', { type: 'warning' });
    await cancelOutbound(orderId, { reason: '手动取消' });
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
