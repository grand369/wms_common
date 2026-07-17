<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="任务号">
        <el-input v-model="filters.taskNo" placeholder="请输入任务号" clearable />
      </el-form-item>
      <el-form-item label="任务类型">
        <el-input v-model="filters.taskType" placeholder="请输入任务类型" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="待处理" :value="0" />
          <el-option label="进行中" :value="1" />
          <el-option label="已完成" :value="2" />
          <el-option label="已挂起" :value="3" />
          <el-option label="异常" :value="4" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>任务列表</span>
          <div class="header-actions">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> 新建任务
            </el-button>
            <WmsSignalRIndicator :status="connected ? 'connected' : 'disconnected'" show-label />
          </div>
        </div>
      </template>

      <WmsTable
        :data="tableData"
        :loading="loading"
        :total="total"
        v-model:current-page="pagination.currentPage"
        v-model:page-size="pagination.pageSize"
        :page-sizes="pagination.pageSizes"
        @page-change="handlePageChange"
        @size-change="handleSizeChange"
      >
        <el-table-column prop="taskNo" label="任务号" />
        <el-table-column prop="taskType" label="任务类型" />
        <el-table-column prop="sourceDocType" label="来源单据" />
        <el-table-column prop="priority" label="优先级" />
        <el-table-column prop="assigneeName" label="执行人" />
        <el-table-column prop="dueTime" label="截止时间" />
        <el-table-column prop="status" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapTaskStatus(row.status)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="260" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as TaskDto)">详情</el-button>
            <el-button link type="primary" :disabled="(row as TaskDto).status !== 0" @click="handleAssign(row as TaskDto)">分配</el-button>
            <el-button link type="success" :disabled="(row as TaskDto).status !== 0" @click="handleStart(row as TaskDto)">开始</el-button>
            <el-button link type="danger" :disabled="(row as TaskDto).status !== 1" @click="handleSuspend(row as TaskDto)">挂起</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsDialog
      title="分配任务"
      :visible="assignVisible"
      show-footer
      width="400px"
      :confirm-loading="assignSubmitting"
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
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsSignalRIndicator from '@/components/common/WmsSignalRIndicator.vue';
import { useTable } from '@/hooks/useTable';
import { useSignalR } from '@/utils/signalr';
import { assignTask, startTask, suspendTask } from '@/api/taskCenter';
import type { TaskDto } from '@/api/taskCenter';

const router = useRouter();
const { connected } = useSignalR('/signalr/task');

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<TaskDto>('/api/v1/task-center/tasks');

const currentTask = ref<TaskDto | null>(null);
const assignVisible = ref(false);
const assignSubmitting = ref(false);
const assignFormRef = ref<FormInstance>();
const assignForm = ref({ assigneeId: '' });
const assignRules: FormRules = {
  assigneeId: [{ required: true, message: '请输入执行人ID', trigger: 'blur' }],
};

function mapTaskStatus(status: number) {
  const map: Record<number, string> = { 0: 'Draft', 1: 'InProgress', 2: 'Completed', 3: 'Cancelled', 4: 'InProgress' };
  return map[status] || 'Draft';
}

function handleCreate() {
  router.push('/task-center/create');
}

function handleDetail(row: TaskDto) {
  router.push(`/task-center/detail/${row.id}`);
}

function handleAssign(row: TaskDto) {
  currentTask.value = row;
  assignForm.value = { assigneeId: '' };
  assignVisible.value = true;
}

async function handleAssignSubmit() {
  if (!assignFormRef.value || !currentTask.value) return;
  try {
    await assignFormRef.value.validate();
  } catch {
    return;
  }
  assignSubmitting.value = true;
  try {
    await assignTask(currentTask.value.id, { assigneeId: assignForm.value.assigneeId });
    ElMessage.success('分配成功');
    assignVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('分配失败');
  } finally {
    assignSubmitting.value = false;
  }
}

async function handleStart(row: TaskDto) {
  try {
    await startTask(row.id);
    ElMessage.success('任务已开始');
    handleSearch();
  } catch {
    ElMessage.error('开始失败');
  }
}

async function handleSuspend(row: TaskDto) {
  try {
    await suspendTask(row.id, { reason: '手动挂起' });
    ElMessage.success('任务已挂起');
    handleSearch();
  } catch {
    ElMessage.error('挂起失败');
  }
}

handleSearch();
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
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
