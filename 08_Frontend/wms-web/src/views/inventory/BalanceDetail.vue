<template>
  <div class="page-container">
    <el-page-header title="返回" content="库存余额详情" @back="goBack" />

    <el-card shadow="hover" class="detail-card">
      <template #header>
        <div class="card-header">
          <span>库存明细</span>
          <div class="header-actions">
            <el-button v-if="balance?.inventoryStatusValue === 0" type="warning" @click="freezeVisible = true">冻结</el-button>
            <el-button v-if="balance?.inventoryStatusValue === 1" type="success" @click="handleUnfreeze">解冻</el-button>
          </div>
        </div>
      </template>

      <el-descriptions :column="3" border v-loading="loading">
        <el-descriptions-item label="物料编码">{{ balance?.materialCode }}</el-descriptions-item>
        <el-descriptions-item label="物料名称">{{ balance?.materialName }}</el-descriptions-item>
        <el-descriptions-item label="仓库">{{ balance?.warehouseName }}</el-descriptions-item>
        <el-descriptions-item label="库位">{{ balance?.locationName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="批次">{{ balance?.batchNumber || '-' }}</el-descriptions-item>
        <el-descriptions-item label="总数量">{{ balance?.quantity }}</el-descriptions-item>
        <el-descriptions-item label="可用数量">{{ balance?.availableQuantity }}</el-descriptions-item>
        <el-descriptions-item label="冻结数量">{{ balance?.frozenQuantity }}</el-descriptions-item>
        <el-descriptions-item label="预留数量">{{ balance?.reservedQuantity }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <WmsStatusTag v-if="balance" :status="mapInventoryStatus(balance.inventoryStatusValue)" type="inventory" />
        </el-descriptions-item>
      </el-descriptions>
    </el-card>

    <WmsDialog
      title="冻结库存"
      :visible="freezeVisible"
      show-footer
      width="500px"
      :confirm-loading="freezeSubmitting"
      @close="freezeVisible = false"
      @cancel="freezeVisible = false"
      @confirm="handleFreezeSubmit"
    >
      <el-form ref="freezeFormRef" :model="freezeForm" :rules="freezeRules" label-width="100px">
        <el-form-item label="冻结数量" prop="qty">
          <el-input-number v-model="freezeForm.qty" :min="0.01" :max="balance?.availableQuantity || 0" :precision="2" />
        </el-form-item>
        <el-form-item label="冻结原因" prop="reason">
          <el-input v-model="freezeForm.reason" type="textarea" :rows="3" placeholder="请输入冻结原因" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { getBalance, freezeBalance, unfreezeBalance } from '@/api/inventory';
import type { InventoryBalanceDto } from '@/api/inventory';
import { getFriendlyErrorMessage, parseAxiosError } from '@/utils/errorHandler';

const route = useRoute();
const router = useRouter();
const balanceId = route.params.id as string;

const balance = ref<InventoryBalanceDto | null>(null);
const loading = ref(false);
const freezeVisible = ref(false);
const freezeSubmitting = ref(false);
const freezeFormRef = ref<FormInstance>();
const freezeForm = ref({ qty: 0, reason: '' });
const freezeRules: FormRules = {
  qty: [{ required: true, message: '请输入冻结数量', trigger: 'blur' }],
  reason: [{ required: true, message: '请输入冻结原因', trigger: 'blur' }],
};

async function loadBalance() {
  loading.value = true;
  try {
    balance.value = await getBalance(balanceId);
  } catch {
    ElMessage.error('加载库存余额失败');
  } finally {
    loading.value = false;
  }
}

function mapInventoryStatus(status: number) {
  const map: Record<number, string> = { 0: 'Available', 1: 'Frozen', 2: 'PendingInspection', 3: 'Quarantined', 4: 'InTransit' };
  return map[status] || 'Available';
}

function goBack() {
  router.push('/inventory/balance');
}

async function handleFreezeSubmit() {
  if (!freezeFormRef.value || !balance.value) return;
  try {
    await freezeFormRef.value.validate();
  } catch {
    return;
  }
  freezeSubmitting.value = true;
  try {
    await freezeBalance(balance.value.id, { qty: freezeForm.value.qty, reason: freezeForm.value.reason });
    ElMessage.success('冻结成功');
    freezeVisible.value = false;
    loadBalance();
  } catch (err) {
    const friendlyMsg = getFriendlyErrorMessage(parseAxiosError(err))
    ElMessage.error(friendlyMsg);
  } finally {
    freezeSubmitting.value = false;
  }
}

async function handleUnfreeze() {
  if (!balance.value) return;
  try {
    await unfreezeBalance(balance.value.id);
    ElMessage.success('解冻成功');
    loadBalance();
  } catch (err) {
    const friendlyMsg = getFriendlyErrorMessage(parseAxiosError(err))
    ElMessage.error(friendlyMsg);
  }
}

onMounted(() => {
  loadBalance();
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
}
</style>
