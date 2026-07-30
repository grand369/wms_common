<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="任务号">
        <el-input v-model="filters.taskNo" placeholder="请输入任务号" clearable />
      </el-form-item>
      <el-form-item label="任务类型">
        <el-select v-model="filters.taskTypeValue" placeholder="请选择任务类型" clearable>
          <el-option label="拣货任务" :value="1" />
          <el-option label="发货任务" :value="2" />
          <el-option label="移库任务" :value="3" />
          <el-option label="盘点任务" :value="4" />
          <el-option label="收货任务" :value="5" />
        </el-select>
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.taskStatusValue" placeholder="请选择状态" clearable>
          <el-option label="已创建" :value="0" />
          <el-option label="已分配" :value="1" />
          <el-option label="进行中" :value="2" />
          <el-option label="已挂起" :value="3" />
          <el-option label="已完成" :value="4" />
          <el-option label="已取消" :value="5" />
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
        <el-table-column prop="taskNo" label="任务号" width="180" />
        <el-table-column prop="taskTypeDescription" label="任务类型" width="120">
          <template #default="{ row }">
            {{ getTaskTypeText(row.taskTypeValue) }}
          </template>
        </el-table-column>
        <el-table-column prop="sourceOrderNo" label="来源单据" width="180" />
        <el-table-column prop="taskPriorityDescription" label="优先级" width="80">
          <template #default="{ row }">
            <el-tag :type="getPriorityTagType(row.taskPriorityValue)">{{ getPriorityText(row.taskPriorityValue) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="assignedUserName" label="执行人" width="120">
          <template #default="{ row }">
            {{ row.assignedUserName || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="expectedCompletionTime" label="截止时间" width="160">
          <template #default="{ row }">
            {{ row.expectedCompletionTime ? formatTime(row.expectedCompletionTime) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="taskStatusValue" label="状态" align="center" width="100">
          <template #default="{ row }">
            <WmsStatusTag :status="mapTaskStatus(row.taskStatusValue)" type="document" />
          </template>
        </el-table-column>
        <el-table-column label="操作" width="320" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row as TaskDto)">详情</el-button>
            <el-button 
              link type="primary" 
              :disabled="(row as TaskDto).taskStatusValue !== 0 && (row as TaskDto).taskStatusValue !== 1" 
              @click="handleAssign(row as TaskDto)"
            >分配</el-button>
            <el-button 
              link type="success" 
              :disabled="(row as TaskDto).taskStatusValue !== 1" 
              @click="handleStart(row as TaskDto)"
            >开始</el-button>
            <el-button 
              link type="warning" 
              :disabled="(row as TaskDto).taskStatusValue !== 2" 
              @click="handleSuspend(row as TaskDto)"
            >挂起</el-button>
            <el-button 
              link type="primary" 
              :disabled="(row as TaskDto).taskStatusValue !== 2" 
              @click="handleComplete(row as TaskDto)"
            >完成</el-button>
            <el-button 
              link type="success" 
              :disabled="(row as TaskDto).taskStatusValue !== 3" 
              @click="handleResume(row as TaskDto)"
            >恢复</el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <WmsUserSelector
      v-model="assignVisible"
      title="分配任务 - 选择执行人"
      @select="handleAssignSubmit"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsSignalRIndicator from '@/components/common/WmsSignalRIndicator.vue';
import WmsUserSelector, { type WmsUser } from '@/components/common/WmsUserSelector.vue';
import { useTable } from '@/hooks/useTable';
import { useSignalR } from '@/utils/signalr';
import { assignTask, startTask, suspendTask, completeTask, resumeTask, TaskStatusEnum } from '@/api/taskCenter';
import type { TaskDto } from '@/api/taskCenter';

const router = useRouter();
const { connected } = useSignalR('/signalr/task');

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<TaskDto>('/api/v1/task-center/tasks');

const currentTask = ref<TaskDto | null>(null);
const assignVisible = ref(false);

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

function formatTime(time: string): string {
  if (!time) return '-';
  return new Date(time).toLocaleString('zh-CN');
}

function handleCreate() {
  router.push('/task-center/create');
}

function handleDetail(row: TaskDto) {
  router.push(`/task-center/detail/${row.id}`);
}

function handleAssign(row: TaskDto) {
  currentTask.value = row;
  assignVisible.value = true;
}

async function handleAssignSubmit(user: WmsUser) {
  if (!currentTask.value) return;
  try {
    await assignTask(currentTask.value.id, { 
      userId: user.id, 
      userName: user.name || user.userName 
    });
    ElMessage.success(`已分配给用户：${user.name || user.userName}`);
    assignVisible.value = false;
    handleSearch();
  } catch {
    // Error handled by interceptor
  }
}

async function handleStart(row: TaskDto) {
  try {
    await startTask(row.id);
    ElMessage.success('任务已开始');
    handleSearch();
  } catch {
    // Error handled by interceptor
  }
}

async function handleSuspend(row: TaskDto) {
  try {
    const { value: reason } = await ElMessageBox.prompt('请输入挂起原因', '挂起任务', { 
      inputValue: '',
      inputValidator: (val: string) => {
        if (!val || !val.trim()) return '挂起原因不能为空';
        return true;
      }
    });
    await suspendTask(row.id, { reason: reason.trim() });
    ElMessage.success('任务已挂起');
    handleSearch();
  } catch (err) {
    if (err !== undefined && err !== 'cancel') {
      // Error handled by interceptor
    }
  }
}

async function handleComplete(row: TaskDto) {
  try {
    await completeTask(row.id);
    ElMessage.success('任务已完成');
    handleSearch();
  } catch {
    // Error handled by interceptor
  }
}

async function handleResume(row: TaskDto) {
  try {
    await resumeTask(row.id);
    ElMessage.success('任务已恢复');
    handleSearch();
  } catch {
    // Error handled by interceptor
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