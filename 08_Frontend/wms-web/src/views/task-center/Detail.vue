<template>
  <div class="page-container">
    <el-page-header title="返回" content="任务详情" @back="goBack" />

    <el-card shadow="hover" class="detail-card">
      <template #header>
        <div class="card-header">
          <span>任务号：{{ task?.taskNo }}</span>
          <div class="header-actions">
            <WmsStatusTag v-if="task" :status="mapTaskStatus(task.status)" type="document" />
            <el-button v-if="task?.status === 0" type="primary" @click="assignVisible = true">分配</el-button>
            <el-button v-if="task?.status === 0" type="success" @click="handleStart">开始</el-button>
            <el-button v-if="task?.status === 1" type="warning" @click="handleSuspend">挂起</el-button>
            <el-button v-if="task?.status === 1" type="primary" @click="completeVisible = true">完成</el-button>
            <el-button v-if="task?.status === 3" type="success" @click="handleResume">恢复</el-button>
            <el-button v-if="task?.status === 1 || task?.status === 3" type="danger" @click="exceptionVisible = true">异常</el-button>
          </div>
        </div>
      </template>

      <el-descriptions :column="3" border>
        <el-descriptions-item label="任务类型">{{ task?.taskType }}</el-descriptions-item>
        <el-descriptions-item label="来源单据">{{ task?.sourceDocType }}</el-descriptions-item>
        <el-descriptions-item label="来源单号">{{ task?.sourceDocId }}</el-descriptions-item>
        <el-descriptions-item label="优先级">{{ task?.priority }}</el-descriptions-item>
        <el-descriptions-item label="执行人">{{ task?.assigneeName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="截止时间">{{ task?.dueTime || '-' }}</el-descriptions-item>
        <el-descriptions-item label="完成时间">{{ task?.completedTime || '-' }}</el-descriptions-item>
      </el-descriptions>

      <el-divider content-position="left">任务评论</el-divider>
      <div class="comment-input">
        <el-input v-model="newComment" type="textarea" :rows="2" placeholder="请输入评论" />
        <el-button type="primary" @click="handleAddComment">发表评论</el-button>
      </div>
      <el-timeline>
        <el-timeline-item v-for="(comment, index) in comments" :key="index" :timestamp="comment.creationTime">
          <p>{{ comment.content }}</p>
          <p class="comment-author">— {{ comment.creatorName }}</p>
        </el-timeline-item>
      </el-timeline>
    </el-card>

    <WmsDialog
      title="分配任务"
      :visible="assignVisible"
      show-footer
      width="400px"
      :confirm-loading="submitting"
      @close="assignVisible = false"
      @cancel="assignVisible = false"
      @confirm="handleAssignSubmit"
    >
      <el-form ref="assignFormRef" :model="assignForm" :rules="assignRules" label-width="100px">
        <el-form-item label="执行人" prop="assigneeId">
          <el-input v-model="assignForm.assigneeId" placeholder="请输入执行人ID" />
        </el-form-item>
      </el-form>
    </WmsDialog>

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
        <el-form-item label="完成结果">
          <el-input v-model="completeForm.result" type="textarea" :rows="3" placeholder="请输入完成结果" />
        </el-form-item>
      </el-form>
    </WmsDialog>

    <WmsDialog
      title="上报异常"
      :visible="exceptionVisible"
      show-footer
      width="500px"
      :confirm-loading="submitting"
      @close="exceptionVisible = false"
      @cancel="exceptionVisible = false"
      @confirm="handleExceptionSubmit"
    >
      <el-form ref="exceptionFormRef" :model="exceptionForm" :rules="exceptionRules" label-width="100px">
        <el-form-item label="异常类型" prop="exceptionType">
          <el-select v-model="exceptionForm.exceptionType" placeholder="请选择异常类型" style="width: 100%">
            <el-option label="物料缺失" value="MissingMaterial" />
            <el-option label="库位异常" value="LocationError" />
            <el-option label="设备故障" value="EquipmentFailure" />
            <el-option label="其他" value="Other" />
          </el-select>
        </el-form-item>
        <el-form-item label="异常描述" prop="description">
          <el-input v-model="exceptionForm.description" type="textarea" :rows="3" placeholder="请输入异常描述" />
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
import { getTask, assignTask, startTask, completeTask, suspendTask, resumeTask, reportException, addTaskComment } from '@/api/taskCenter';
import type { TaskDto, TaskCommentDto } from '@/api/taskCenter';

const route = useRoute();
const router = useRouter();
const taskId = route.params.id as string;

const task = ref<TaskDto | null>(null);
const comments = ref<TaskCommentDto[]>([]);
const newComment = ref('');
const loading = ref(false);
const submitting = ref(false);

const assignVisible = ref(false);
const completeVisible = ref(false);
const exceptionVisible = ref(false);
const assignFormRef = ref<FormInstance>();
const completeFormRef = ref<FormInstance>();
const exceptionFormRef = ref<FormInstance>();
const assignForm = ref({ assigneeId: '' });
const completeForm = ref({ result: '' });
const exceptionForm = ref({ exceptionType: '', description: '' });

const assignRules: FormRules = {
  assigneeId: [{ required: true, message: '请输入执行人ID', trigger: 'blur' }],
};
const exceptionRules: FormRules = {
  exceptionType: [{ required: true, message: '请选择异常类型', trigger: 'change' }],
  description: [{ required: true, message: '请输入异常描述', trigger: 'blur' }],
};

async function loadTask() {
  loading.value = true;
  try {
    task.value = await getTask(taskId);
  } catch {
    ElMessage.error('加载任务失败');
  } finally {
    loading.value = false;
  }
}

function mapTaskStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'InProgress', 2: 'Completed', 3: 'Cancelled', 4: 'InProgress' };
  return map[status] || 'Draft';
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
    ElMessage.error('开始失败');
  }
}

async function handleSuspend() {
  try {
    await suspendTask(taskId, { reason: '手动挂起' });
    ElMessage.success('任务已挂起');
    loadTask();
  } catch {
    ElMessage.error('挂起失败');
  }
}

async function handleResume() {
  try {
    await resumeTask(taskId);
    ElMessage.success('任务已恢复');
    loadTask();
  } catch {
    ElMessage.error('恢复失败');
  }
}

async function handleAssignSubmit() {
  if (!assignFormRef.value) return;
  try {
    await assignFormRef.value.validate();
  } catch {
    return;
  }
  submitting.value = true;
  try {
    await assignTask(taskId, { assigneeId: assignForm.value.assigneeId });
    ElMessage.success('分配成功');
    assignVisible.value = false;
    loadTask();
  } catch {
    ElMessage.error('分配失败');
  } finally {
    submitting.value = false;
  }
}

async function handleCompleteSubmit() {
  if (!completeFormRef.value) return;
  submitting.value = true;
  try {
    await completeTask(taskId, { result: completeForm.value.result });
    ElMessage.success('任务已完成');
    completeVisible.value = false;
    loadTask();
  } catch {
    ElMessage.error('完成失败');
  } finally {
    submitting.value = false;
  }
}

async function handleExceptionSubmit() {
  if (!exceptionFormRef.value) return;
  try {
    await exceptionFormRef.value.validate();
  } catch {
    return;
  }
  submitting.value = true;
  try {
    await reportException(taskId, { exceptionType: exceptionForm.value.exceptionType, description: exceptionForm.value.description });
    ElMessage.success('异常已上报');
    exceptionVisible.value = false;
    loadTask();
  } catch {
    ElMessage.error('上报失败');
  } finally {
    submitting.value = false;
  }
}

async function handleAddComment() {
  if (!newComment.value.trim()) {
    ElMessage.warning('请输入评论内容');
    return;
  }
  try {
    const comment = await addTaskComment(taskId, { content: newComment.value });
    comments.value.unshift(comment);
    newComment.value = '';
    ElMessage.success('评论已发表');
  } catch {
    ElMessage.error('发表失败');
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
.comment-input {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
  .el-button {
    flex-shrink: 0;
  }
}
.comment-author {
  color: #909399;
  font-size: 12px;
  margin-top: 4px;
}
</style>
