<template>
  <div class="page-container">
    <el-page-header title="返回" content="调拨单详情" @back="goBack" />

    <el-card shadow="hover" class="detail-card">
      <template #header>
        <div class="card-header">
          <span>调拨单号：{{ order?.transferNo }}</span>
          <div class="header-actions">
            <WmsStatusTag v-if="order" :status="mapStatus(order.status)" type="document" />
            <el-button
              v-if="order?.status === 1"
              type="success"
              :loading="actionLoading"
              @click="handleApprove"
            >
              审批
            </el-button>
            <el-button
              v-if="order?.status === 2"
              type="primary"
              :loading="actionLoading"
              @click="handleOutboundConfirm"
            >
              源仓确认出库
            </el-button>
            <el-button
              v-if="order?.status === 4"
              type="primary"
              :loading="actionLoading"
              @click="handleInboundConfirm"
            >
              目标仓确认入库
            </el-button>
            <el-button
              v-if="order?.status === 5"
              type="success"
              :loading="actionLoading"
              @click="handleComplete"
            >
              完成
            </el-button>
            <el-button
              v-if="order && order.status < 6"
              type="danger"
              :loading="actionLoading"
              @click="handleCancel"
            >
              取消
            </el-button>
          </div>
        </div>
      </template>

      <WmsSteps :steps="transferSteps" :active-step="activeStep" />

      <el-descriptions :column="3" border class="order-info">
        <el-descriptions-item label="源仓库">{{ order?.fromWarehouseName }}</el-descriptions-item>
        <el-descriptions-item label="目标仓库">{{ order?.toWarehouseName }}</el-descriptions-item>
        <el-descriptions-item label="计划日期">{{ order?.planDate || '-' }}</el-descriptions-item>
      </el-descriptions>

      <el-divider content-position="left">调拨明细</el-divider>
      <el-table :data="order?.lines" border>
        <el-table-column type="index" width="50" />
        <el-table-column prop="materialCode" label="物料编码" />
        <el-table-column prop="materialName" label="物料名称" show-overflow-tooltip />
        <el-table-column prop="qty" label="数量" align="right" />
        <el-table-column prop="batchNo" label="批次" />
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
import {
  getTransfer,
  approveTransfer,
  outboundConfirmTransfer,
  inboundConfirmTransfer,
  completeTransfer,
  cancelTransfer,
} from '@/api/transfer';
import type { TransferDetailDto } from '@/api/transfer';
import type { WmsTimelineItem } from '@/components/common/WmsTimeline.vue';

const route = useRoute();
const router = useRouter();
const orderId = route.params.id as string;

const order = ref<TransferDetailDto | null>(null);
const loading = ref(false);
const actionLoading = ref(false);
const transferSteps = ['草稿', '待审批', '审批通过', '源仓出库', '在途', '目标仓入库', '已完成'];

const activeStep = computed(() => {
  if (!order.value) return 0;
  const map: Record<number, number> = { 0: 0, 1: 1, 2: 2, 3: 2, 4: 3, 5: 4, 6: 6, 7: 0 };
  return map[order.value.status] || 0;
});

const timelineItems = computed<WmsTimelineItem[]>(() => {
  if (!order.value) return [];
  const items: WmsTimelineItem[] = [];
  items.push({ time: order.value.planDate || '', status: 'Draft', description: '创建调拨单', operator: '' });
  if (order.value.status >= 1) {
    items.push({ time: '', status: 'Confirmed', description: '调拨单已提交待审批', operator: '' });
  }
  if (order.value.status >= 2) {
    items.push({ time: '', status: 'Confirmed', description: '调拨单审批通过', operator: '' });
  }
  if (order.value.status >= 3) {
    items.push({ time: '', status: 'InProgress', description: '源仓库已确认出库', operator: '' });
  }
  if (order.value.status >= 4) {
    items.push({ time: '', status: 'InProgress', description: '调拨物料在途运输', operator: '' });
  }
  if (order.value.status >= 5) {
    items.push({ time: '', status: 'InProgress', description: '目标仓库已确认入库', operator: '' });
  }
  if (order.value.status === 6) {
    items.push({ time: '', status: 'Completed', description: '调拨单已完成', operator: '' });
  }
  if (order.value.status === 7) {
    items.push({ time: '', status: 'Cancelled', description: '调拨单已取消', operator: '' });
  }
  return items;
});

async function loadOrder() {
  loading.value = true;
  try {
    order.value = await getTransfer(orderId);
  } catch {
    ElMessage.error('加载调拨单失败');
  } finally {
    loading.value = false;
  }
}

function mapStatus(status: number) {
  const map: Record<number, string> = {
    0: 'Draft',
    1: 'Confirmed',
    2: 'Approved',
    3: 'InProgress',
    4: 'InProgress',
    5: 'InProgress',
    6: 'Completed',
    7: 'Cancelled',
  };
  return map[status] || 'Draft';
}

function goBack() {
  router.push('/transfer/list');
}

async function handleApprove() {
  actionLoading.value = true;
  try {
    await approveTransfer(orderId);
    ElMessage.success('审批成功');
    await loadOrder();
  } catch {
    ElMessage.error('审批失败');
  } finally {
    actionLoading.value = false;
  }
}

async function handleOutboundConfirm() {
  actionLoading.value = true;
  try {
    await outboundConfirmTransfer(orderId);
    ElMessage.success('源仓出库确认成功');
    await loadOrder();
  } catch {
    ElMessage.error('源仓出库确认失败');
  } finally {
    actionLoading.value = false;
  }
}

async function handleInboundConfirm() {
  actionLoading.value = true;
  try {
    await inboundConfirmTransfer(orderId);
    ElMessage.success('目标仓入库确认成功');
    await loadOrder();
  } catch {
    ElMessage.error('目标仓入库确认失败');
  } finally {
    actionLoading.value = false;
  }
}

async function handleComplete() {
  actionLoading.value = true;
  try {
    await completeTransfer(orderId);
    ElMessage.success('调拨单已完成');
    await loadOrder();
  } catch {
    ElMessage.error('完成失败');
  } finally {
    actionLoading.value = false;
  }
}

async function handleCancel() {
  try {
    await ElMessageBox.confirm('确认取消该调拨单？', '提示', { type: 'warning' });
  } catch {
    return;
  }
  actionLoading.value = true;
  try {
    await cancelTransfer(orderId, { reason: '手动取消' });
    ElMessage.success('取消成功');
    await loadOrder();
  } catch {
    ElMessage.error('取消失败');
  } finally {
    actionLoading.value = false;
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
