<template>
  <div class="page-container">
    <el-page-header title="返回" content="任务详情" @back="goBack" />

    <el-card shadow="hover" class="detail-card" v-loading="loading">
      <template #header v-if="task">
        <div class="card-header">
          <span>任务号：{{ task.taskNo }}</span>
          <div class="header-actions">
            <WmsStatusTag :status="mapTaskStatus(task.taskStatusValue)" type="document" />
            <el-button 
              v-if="task.taskStatusValue === 0 || task.taskStatusValue === 1" 
              type="primary" 
              @click="assignVisible = true"
            >分配</el-button>
            <el-button 
              v-if="task.taskStatusValue === 1" 
              type="success" 
              @click="handleStart"
            >开始</el-button>
            <el-button 
              v-if="task.taskStatusValue === 2" 
              type="warning" 
              @click="handleSuspend"
            >挂起</el-button>
            <el-button 
              v-if="task.taskStatusValue === 2" 
              type="primary" 
              @click="completeVisible = true"
            >完成</el-button>
            <el-button 
              v-if="task.taskStatusValue === 3" 
              type="success" 
              @click="handleResume"
            >恢复</el-button>
            <el-button 
              v-if="task.taskStatusValue === 0 || task.taskStatusValue === 1 || task.taskStatusValue === 3" 
              type="danger" 
              @click="handleCancel"
            >取消</el-button>
          </div>
        </div>
      </template>

      <el-descriptions v-if="task" :column="3" border>
        <el-descriptions-item label="任务类型">
          {{ getTaskTypeText(task.taskTypeValue) }}
        </el-descriptions-item>
        <el-descriptions-item label="优先级">
          <el-tag :type="getPriorityTagType(task.taskPriorityValue)">
            {{ getPriorityText(task.taskPriorityValue) }}
          </el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="状态">
          <WmsStatusTag :status="mapTaskStatus(task.taskStatusValue)" type="document" />
        </el-descriptions-item>
        <el-descriptions-item label="来源单据类型">
          {{ getSourceOrderTypeText(task.sourceOrderType) }}
        </el-descriptions-item>
        <el-descriptions-item label="来源单据号">
          {{ task.sourceOrderNo }}
        </el-descriptions-item>
        <el-descriptions-item label="仓库">{{ task.warehouseCode }}</el-descriptions-item>
        <el-descriptions-item label="执行人">
          {{ task.assignedUserName || '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="分配策略">
          {{ getAssignmentStrategyText(task.assignmentStrategyValue) }}
        </el-descriptions-item>
        <el-descriptions-item label="进度">{{ task.taskProgress }}%</el-descriptions-item>
        <el-descriptions-item label="截止时间">
          {{ task.expectedCompletionTime ? formatTime(task.expectedCompletionTime) : '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="开始时间">
          {{ task.actualStartTime ? formatTime(task.actualStartTime) : '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="完成时间">
          {{ task.actualCompletionTime ? formatTime(task.actualCompletionTime) : '-' }}
        </el-descriptions-item>
        <el-descriptions-item label="创建时间" :span="2">
          {{ formatTime(task.creationTime) }}
        </el-descriptions-item>
        <el-descriptions-item label="挂起原因" v-if="task.suspendedReason">
          {{ task.suspendedReason }}
        </el-descriptions-item>
        <el-descriptions-item label="备注" :span="3">
          {{ task.remark || '-' }}
        </el-descriptions-item>
      </el-descriptions>

      <template v-if="task">
        <el-divider content-position="left">任务进度</el-divider>
        <el-progress :percentage="task.taskProgress" :status="getProgressStatus(task.taskStatusValue)" />
      </template>
    </el-card>

    <WmsUserSelector
      v-model="assignVisible"
      title="分配任务 - 选择执行人"
      @select="handleAssignSubmit"
    />

    <WmsDialog
      title="完成任务"
      :visible="completeVisible"
      show-footer
      width="500px"
      :confirm-loading="submitting"
      @close="completeVisible = false"
      @cancel="completeVisible = false"
      @confirm="handleCompleteSubmit"
    >
      <el-form ref="completeFormRef" :model="completeForm" label-width="100px">
        <el-form-item label="完成备注">
          <el-input v-model="completeForm.remark" type="textarea" :rows="3" placeholder="请输入完成备注（可选）" />
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsUserSelector, { type WmsUser } from '@/components/common/WmsUserSelector.vue';
import { getTask, assignTask, startTask, completeTask, suspendTask, resumeTask, cancelTask, TaskStatusEnum } from '@/api/taskCenter';
import type { TaskDto } from '@/api/taskCenter';

const route = useRoute();
const router = useRouter();
const taskId = route.params.id as string;

const task = ref<TaskDto | null>(null);
const loading = ref(false);
const submitting = ref(false);

const assignVisible = ref(false);
const completeVisible = ref(false);
const completeFormRef = ref<FormInstance>();
const completeForm = ref({ remark: '' });

function getTaskTypeText(typeValue: number): string {
  const map: Record<number, string> = { 1: '拣货任务', 2: '发货任务', 3: '移库任务', 4: '盘点任务', 5: '收货任务' };
  return map[typeValue] || '未知';
}

function getPriorityText(priorityValue: number): string {
  const map: Record<number, string> = { 1: '低', 2: '中', 3: '高', 4: '紧急' };
  return map[priorityValue] || '未知';
}

function getPriorityTagType(priorityValue: number): 'info' | 'warning' | 'danger' | 'success' {
  const map: Record<number, 'info' | 'warning' | 'danger' | 'success'> = { 1: 'info', 2: 'success', 3: 'warning', 4: 'danger' };
  return map[priorityValue] || 'info';
}

function getSourceOrderTypeText(type: string): string {
  const map: Record<string, string> = { 
    'OutboundOrder': '出库单', 
    'InboundOrder': '入库单', 
    'TransferOrder': '调拨单', 
    'CycleCountPlan': '盘点单' 
  };
  return map[type] || type;
}

function getAssignmentStrategyText(strategyValue: number): string {
  const map: Record<number, string> = { 0: '手动分配', 1: '区域优先', 2: '技能匹配', 3: '负载均衡' };
  return map[strategyValue] || '未知';
}

function mapTaskStatus(status: number): string {
  const map: Record<number, string> = { 
    [TaskStatusEnum.Created]: 'Draft', 
    [TaskStatusEnum.Assigned]: 'Assigned', 
    [TaskStatusEnum.InProgress]: 'InProgress', 
    [TaskStatusEnum.Suspended]: 'Suspended', 
    [TaskStatusEnum.Completed]: 'Completed', 
    [TaskStatusEnum.Cancelled]: 'Cancelled' 
  };
  return map[status] || 'Draft';
}

function getProgressStatus(status: number): 'success' | 'warning' | 'exception' | undefined {
  if (status === TaskStatusEnum.Completed) return 'success';
  if (status === TaskStatusEnum.Suspended) return 'warning';
  if (status === TaskStatusEnum.Cancelled) return 'exception';
  return undefined;
}

function formatTime(time: string): string {
  if (!time) return '-';
  return new Date(time).toLocaleString('zh-CN');
}

async function loadTask() {
  loading.value = true;
  try {
    task.value = await getTask(taskId);
  } catch {
    // Error handled by interceptor
  } finally {
    loading.value = false;
  }
}

function goBack() {
  router.push('/task-center/list');
}

async function handleStart() {
  try {
    await startTask(taskId);
    ElMessage.success('任务已开始');
    loadTask();
  } catch {
    // Error handled by interceptor
  }
}

async function handleSuspend() {
  try {
    const { value: reason } = await ElMessageBox.prompt('请输入挂起原因', '挂起任务', { 
      inputValue: '',
      inputValidator: (val: string) => {
        if (!val || !val.trim()) return '挂起原因不能为空';
        return true;
      }
    });
    await suspendTask(taskId, { reason: reason.trim() });
    ElMessage.success('任务已挂起');
    loadTask();
  } catch (err) {
    if (err !== undefined && err !== 'cancel') {
      // Error handled by interceptor
    }
  }
}

async function handleResume() {
  try {
    await resumeTask(taskId);
    ElMessage.success('任务已恢复');
    loadTask();
  } catch {
    // Error handled by interceptor
  }
}

async function handleCancel() {
  try {
    await ElMessageBox.confirm('确认取消该任务？取消后无法恢复。', '取消任务', { type: 'warning' });
    const { value: reason } = await ElMessageBox.prompt('请输入取消原因', '取消任务', { 
      inputValue: '',
      inputValidator: (val: string) => {
        if (!val || !val.trim()) return '取消原因不能为空';
        return true;
      }
    });
    await cancelTask(taskId, { reason: reason.trim() });
    ElMessage.success('任务已取消');
    loadTask();
  } catch (err) {
    if (err !== undefined && err !== 'cancel') {
      // Error handled by interceptor
    }
  }
}

async function handleAssignSubmit(user: WmsUser) {
  try {
    await assignTask(taskId, { 
      userId: user.id, 
      userName: user.name || user.userName 
    });
    ElMessage.success(`已分配给用户：${user.name || user.userName}`);
    assignVisible.value = false;
    loadTask();
  } catch {
    // Error handled by interceptor
  }
}

async function handleCompleteSubmit() {
  if (!completeFormRef.value) return;
  submitting.value = true;
  try {
    await completeTask(taskId, { remark: completeForm.value.remark || undefined });
    ElMessage.success('任务已完成');
    completeVisible.value = false;
    loadTask();
  } catch {
    // Error handled by interceptor
  } finally {
    submitting.value = false;
  }
}

onMounted(() => {
  loadTask();
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
</style>