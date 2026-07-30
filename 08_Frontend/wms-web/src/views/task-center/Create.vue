<template>
  <div class="page-container">
    <el-page-header title="返回" content="新建任务" @back="goBack" />

    <el-card shadow="hover" class="form-card">
      <el-form ref="formRef" :model="form" :rules="rules" label-width="120px" class="task-form">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="任务类型" prop="taskTypeValue">
              <el-select v-model="form.taskTypeValue" placeholder="请选择任务类型" style="width: 100%">
                <el-option label="拣货任务" :value="1" />
                <el-option label="发货任务" :value="2" />
                <el-option label="移库任务" :value="3" />
                <el-option label="盘点任务" :value="4" />
                <el-option label="收货任务" :value="5" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="优先级" prop="taskPriorityValue">
              <el-select v-model="form.taskPriorityValue" placeholder="请选择优先级" style="width: 100%">
                <el-option label="低" :value="1" />
                <el-option label="中" :value="2" />
                <el-option label="高" :value="3" />
                <el-option label="紧急" :value="4" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="仓库" prop="warehouseId">
              <WmsWarehouseSelector v-model="form.warehouseId" @change="onWarehouseChange" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="来源单据类型" prop="sourceOrderType">
              <el-select v-model="form.sourceOrderType" placeholder="请选择单据类型" style="width: 100%" @change="onSourceTypeChange">
                <el-option label="出库单" value="OutboundOrder" />
                <el-option label="入库单" value="InboundOrder" />
                <el-option label="调拨单" value="TransferOrder" />
                <el-option label="盘点单" value="CycleCountPlan" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="来源单据" prop="sourceOrderId">
              <div class="source-order-input">
                <el-input v-model="form.sourceOrderNo" placeholder="请选择来源单据" readonly>
                  <template #suffix>
                    <el-tag v-if="form.sourceOrderId" type="success" size="small">已选择</el-tag>
                  </template>
                </el-input>
                <el-button type="primary" @click="openSourceOrderDialog">选择</el-button>
              </div>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="截止时间" prop="expectedCompletionTime">
              <el-date-picker v-model="form.expectedCompletionTime" type="datetime" placeholder="请选择截止时间" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="分配策略" prop="assignmentStrategyValue">
              <el-select v-model="form.assignmentStrategyValue" placeholder="请选择分配策略" style="width: 100%">
                <el-option label="手动分配" :value="0" />
                <el-option label="区域优先" :value="1" />
                <el-option label="技能匹配" :value="2" />
                <el-option label="负载均衡" :value="3" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="form.remark" type="textarea" :rows="3" placeholder="请输入备注" />
        </el-form-item>
      </el-form>

      <div class="form-actions">
        <el-button @click="goBack">取消</el-button>
        <el-button type="primary" :loading="submitting" @click="handleSubmit">创建任务</el-button>
      </div>
    </el-card>

    <!-- 来源单据选择对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="70%"
      destroy-on-close
    >
      <div class="dialog-toolbar">
        <el-input v-model="dialogSearch" placeholder="搜索单据号" clearable style="width: 300px" @keyup.enter="searchSourceOrders" />
        <el-button type="primary" @click="searchSourceOrders">搜索</el-button>
      </div>
      <el-table v-loading="dialogLoading" :data="dialogData" border highlight-current-row @current-change="onSelectOrder">
        <el-table-column prop="orderNo" label="单据号" width="180" />
        <el-table-column prop="statusName" label="状态" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusTagType(row.statusValue)">{{ row.statusName }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="warehouseCode" label="仓库" width="120" />
        <el-table-column prop="creationTime" label="创建时间" width="180">
          <template #default="{ row }">
            {{ formatTime(row.creationTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="remark" label="备注" />
      </el-table>
      <div class="dialog-pagination">
        <el-pagination
          v-model:current-page="dialogPage"
          v-model:page-size="dialogPageSize"
          :total="dialogTotal"
          :page-sizes="[10, 20, 50]"
          layout="total, sizes, prev, pager, next"
          @size-change="searchSourceOrders"
          @current-change="searchSourceOrders"
        />
      </div>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" :disabled="!selectedOrder" @click="confirmSelect">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsWarehouseSelector, { type WmsWarehouse } from '@/components/common/WmsWarehouseSelector.vue';
import { createTask, type CreateOrUpdateTaskDto } from '@/api/taskCenter';
import { getOutboundOrders } from '@/api/outbound';
import { getInboundOrders } from '@/api/inbound';
import type { OutboundOrderDto } from '@/api/outbound';
import type { InboundOrderDto } from '@/api/inbound';

const router = useRouter();
const formRef = ref<FormInstance>();
const submitting = ref(false);

const form = reactive({
  taskTypeValue: 1,
  taskPriorityValue: 2,
  warehouseId: '',
  warehouseCode: '',
  sourceOrderType: 'OutboundOrder',
  sourceOrderId: '',
  sourceOrderNo: '',
  expectedCompletionTime: '' as string | Date,
  assignmentStrategyValue: 0,
  remark: '',
});

const rules: FormRules = {
  taskTypeValue: [{ required: true, message: '请选择任务类型', trigger: 'change' }],
  taskPriorityValue: [{ required: true, message: '请选择优先级', trigger: 'change' }],
  warehouseId: [{ required: true, message: '请选择仓库', trigger: 'change' }],
  sourceOrderType: [{ required: true, message: '请选择来源单据类型', trigger: 'change' }],
  sourceOrderId: [{ required: true, message: '请选择来源单据', trigger: 'change' }],
};

// 对话框状态
const dialogVisible = ref(false);
const dialogLoading = ref(false);
const dialogData = ref<any[]>([]);
const dialogSearch = ref('');
const dialogPage = ref(1);
const dialogPageSize = ref(10);
const dialogTotal = ref(0);
const selectedOrder = ref<any>(null);

const dialogTitle = computed(() => {
  const typeNames: Record<string, string> = {
    OutboundOrder: '选择出库单',
    InboundOrder: '选择入库单',
    TransferOrder: '选择调拨单',
    CycleCountPlan: '选择盘点单',
  };
  return typeNames[form.sourceOrderType] || '选择来源单据';
});

function onWarehouseChange(warehouse: WmsWarehouse | WmsWarehouse[] | null) {
  if (warehouse && !Array.isArray(warehouse)) {
    form.warehouseCode = warehouse.code;
  }
}

function onSourceTypeChange() {
  form.sourceOrderId = '';
  form.sourceOrderNo = '';
}

function openSourceOrderDialog() {
  selectedOrder.value = null;
  dialogSearch.value = '';
  dialogPage.value = 1;
  dialogVisible.value = true;
  searchSourceOrders();
}

async function searchSourceOrders() {
  dialogLoading.value = true;
  try {
    const params = {
      keyword: dialogSearch.value || undefined,
      skipCount: (dialogPage.value - 1) * dialogPageSize.value,
      maxResultCount: dialogPageSize.value,
    };

    if (form.sourceOrderType === 'OutboundOrder') {
      const res = await getOutboundOrders(params);
      dialogData.value = res.items.map((item: OutboundOrderDto) => ({
        id: item.id,
        orderNo: item.outboundOrderNo,
        statusValue: item.outboundStatusValue,
        statusName: item.outboundStatusName,
        warehouseCode: item.warehouseCode,
        creationTime: item.creationTime,
        remark: item.remark,
      }));
      dialogTotal.value = res.totalCount;
    } else if (form.sourceOrderType === 'InboundOrder') {
      const res = await getInboundOrders(params);
      dialogData.value = res.items.map((item: InboundOrderDto) => ({
        id: item.id,
        orderNo: item.inboundOrderNo,
        statusValue: item.inboundStatusValue,
        statusName: item.inboundStatusName,
        warehouseCode: item.warehouseCode,
        creationTime: item.creationTime,
        remark: item.remark,
      }));
      dialogTotal.value = res.totalCount;
    } else {
      // TransferOrder, CycleCountPlan - placeholder
      dialogData.value = [];
      dialogTotal.value = 0;
      ElMessage.warning('该单据类型暂不支持选择');
    }
  } catch {
    // Error handled by interceptor
  } finally {
    dialogLoading.value = false;
  }
}

function onSelectOrder(row: any) {
  selectedOrder.value = row;
}

function confirmSelect() {
  if (selectedOrder.value) {
    form.sourceOrderId = selectedOrder.value.id;
    form.sourceOrderNo = selectedOrder.value.orderNo;
    dialogVisible.value = false;
  }
}

function getStatusTagType(status: number): 'success' | 'warning' | 'info' | 'danger' {
  if (status === 0) return 'info';
  if (status <= 2) return 'warning';
  if (status === 3) return 'success';
  if (status >= 4) return 'danger';
  return 'info';
}

function formatTime(time: string): string {
  if (!time) return '';
  return new Date(time).toLocaleString('zh-CN');
}

function goBack() {
  router.push('/task-center/list');
}

async function handleSubmit() {
  if (!formRef.value) return;
  try {
    await formRef.value.validate();
  } catch {
    return;
  }
  submitting.value = true;
  try {
    const payload: CreateOrUpdateTaskDto = {
      taskTypeValue: form.taskTypeValue,
      taskPriorityValue: form.taskPriorityValue,
      sourceOrderType: form.sourceOrderType,
      sourceOrderId: form.sourceOrderId,
      sourceOrderNo: form.sourceOrderNo,
      warehouseId: form.warehouseId,
      warehouseCode: form.warehouseCode,
      assignmentStrategyValue: form.assignmentStrategyValue,
      expectedCompletionTime: form.expectedCompletionTime
        ? (form.expectedCompletionTime instanceof Date
            ? form.expectedCompletionTime.toISOString()
            : String(form.expectedCompletionTime))
        : undefined,
      remark: form.remark || undefined,
    };
    await createTask(payload);
    ElMessage.success('任务创建成功');
    router.push('/task-center/list');
  } catch {
    // Error handled by interceptor
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.form-card {
  max-width: 900px;
  margin: 0 auto;
}
.task-form {
  padding: 16px 0;
}
.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding-top: 16px;
  border-top: 1px solid #ebeef5;
}
.source-order-input {
  display: flex;
  gap: 8px;
  width: 100%;
}
.source-order-input .el-input {
  flex: 1;
}
.dialog-toolbar {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
}
.dialog-pagination {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}
</style>