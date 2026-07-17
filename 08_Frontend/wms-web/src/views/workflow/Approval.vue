<template>
  <div class="page-container">
    <WmsSearch @search="handleSearch" @reset="resetFilters">
      <el-form-item label="流程名称">
        <el-input v-model="filters.workflowName" placeholder="请输入流程名称" clearable />
      </el-form-item>
      <el-form-item label="业务类型">
        <el-input v-model="filters.businessEntityType" placeholder="请输入业务类型" clearable />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="filters.status" placeholder="请选择状态" clearable>
          <el-option label="待审批" :value="0" />
          <el-option label="已通过" :value="1" />
          <el-option label="已驳回" :value="2" />
          <el-option label="已取消" :value="3" />
        </el-select>
      </el-form-item>
    </WmsSearch>

    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>审批实例列表</span>
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
        <el-table-column prop="workflowName" label="流程名称" min-width="160" show-overflow-tooltip />
        <el-table-column prop="businessEntityType" label="业务类型" width="120" />
        <el-table-column prop="applicantName" label="申请人" width="100">
          <template #default="{ row }">
            {{ row.applicantName || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="currentNodeName" label="当前节点" width="120">
          <template #default="{ row }">
            <el-tag size="small" type="warning" v-if="row.status === 0">
              {{ row.currentNodeName || '-' }}
            </el-tag>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column prop="creationTime" label="发起时间" width="160">
          <template #default="{ row }">
            {{ formatDate(row.creationTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)" size="small" effect="dark">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right" align="center">
          <template #default="{ row }">
            <el-button
              link
              type="primary"
              :disabled="(row as ApprovalInstanceDto).status !== 0"
              @click="handleApprove(row as ApprovalInstanceDto)"
            >
              审批
            </el-button>
            <el-button
              link
              type="info"
              @click="handleViewHistory(row as ApprovalInstanceDto)"
            >
              历史
            </el-button>
          </template>
        </el-table-column>
      </WmsTable>
    </el-card>

    <!-- Approval Dialog -->
    <WmsDialog
      :visible="approvalVisible"
      :title="`审批: ${approvalTarget?.workflowName || ''}`"
      width="500px"
      :show-footer="false"
      @close="approvalVisible = false"
    >
      <div class="approval-info">
        <el-descriptions :column="2" border size="small">
          <el-descriptions-item label="流程">{{ approvalTarget?.workflowName }}</el-descriptions-item>
          <el-descriptions-item label="业务类型">{{ approvalTarget?.businessEntityType }}</el-descriptions-item>
          <el-descriptions-item label="申请人">{{ approvalTarget?.applicantName || '-' }}</el-descriptions-item>
          <el-descriptions-item label="当前节点">{{ approvalTarget?.currentNodeName || '-' }}</el-descriptions-item>
        </el-descriptions>
      </div>

      <el-divider />

      <el-form label-width="80px">
        <el-form-item label="审批意见">
          <el-input
            v-model="approvalComment"
            type="textarea"
            :rows="3"
            placeholder="请输入审批意见（选填）"
          />
        </el-form-item>
      </el-form>

      <div class="approval-actions">
        <el-button @click="approvalVisible = false">取消</el-button>
        <el-button type="danger" :loading="rejectLoading" @click="handleReject">
          驳回
        </el-button>
        <el-button type="success" :loading="approveLoading" @click="handlePass">
          通过
        </el-button>
      </div>
    </WmsDialog>

    <!-- History Dialog -->
    <WmsDialog
      :visible="historyVisible"
      title="审批历史"
      width="600px"
      :show-footer="false"
      @close="historyVisible = false"
    >
      <el-timeline v-if="historyItems.length > 0">
        <el-timeline-item
          v-for="item in historyItems"
          :key="item.id"
          :timestamp="formatDate(item.operationTime)"
          :type="getHistoryType(item.action)"
        >
          <div class="history-item">
            <div class="history-title">
              <span class="history-node">{{ item.nodeName }}</span>
              <el-tag
                :type="getActionTagType(item.action)"
                size="small"
                effect="dark"
              >
                {{ item.action }}
              </el-tag>
            </div>
            <div class="history-operator" v-if="item.operatorName">
              操作人: {{ item.operatorName }}
            </div>
            <div class="history-comment" v-if="item.comment">
              {{ item.comment }}
            </div>
          </div>
        </el-timeline-item>
      </el-timeline>
      <el-empty v-else description="暂无审批历史" />
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import WmsSearch from '@/components/common/WmsSearch.vue';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { useTable } from '@/hooks/useTable';
import { formatDate } from '@/utils/format';
import {
  getApprovalInstances,
  approveInstance,
  rejectInstance,
  getApprovalHistory,
  type ApprovalInstanceDto,
  type ApprovalHistoryDto,
} from '@/api/workflow';

const { loading, tableData, total, pagination, filters, handlePageChange, handleSizeChange, handleSearch, resetFilters } =
  useTable<ApprovalInstanceDto>('/api/v1/workflow/instances');

// ── Status Helpers ───────────────────────────────────────────
const statusMap: Record<number, { text: string; type: 'warning' | 'success' | 'danger' | 'info' }> = {
  0: { text: '待审批', type: 'warning' },
  1: { text: '已通过', type: 'success' },
  2: { text: '已驳回', type: 'danger' },
  3: { text: '已取消', type: 'info' },
};

function getStatusText(status: number): string {
  return statusMap[status]?.text || `未知(${status})`;
}

function getStatusType(status: number): string {
  return statusMap[status]?.type || 'info';
}

// ── Approval Dialog ──────────────────────────────────────────
const approvalVisible = ref(false);
const approvalTarget = ref<ApprovalInstanceDto | null>(null);
const approvalComment = ref('');
const approveLoading = ref(false);
const rejectLoading = ref(false);

function handleApprove(row: ApprovalInstanceDto) {
  approvalTarget.value = row;
  approvalComment.value = '';
  approvalVisible.value = true;
}

async function handlePass() {
  if (!approvalTarget.value) return;
  approveLoading.value = true;
  try {
    await approveInstance(approvalTarget.value.id, {
      comment: approvalComment.value || undefined,
    });
    ElMessage.success('审批通过');
    approvalVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('审批操作失败');
  } finally {
    approveLoading.value = false;
  }
}

async function handleReject() {
  if (!approvalTarget.value) return;
  rejectLoading.value = true;
  try {
    await rejectInstance(approvalTarget.value.id, {
      comment: approvalComment.value || undefined,
    });
    ElMessage.success('已驳回');
    approvalVisible.value = false;
    handleSearch();
  } catch {
    ElMessage.error('驳回操作失败');
  } finally {
    rejectLoading.value = false;
  }
}

// ── History Dialog ───────────────────────────────────────────
const historyVisible = ref(false);
const historyItems = ref<ApprovalHistoryDto[]>([]);

async function handleViewHistory(row: ApprovalInstanceDto) {
  historyVisible.value = true;
  historyItems.value = [];
  try {
    const result = await getApprovalHistory(row.id);
    historyItems.value = result.items || [];
  } catch {
    ElMessage.error('加载审批历史失败');
  }
}

function getHistoryType(action: string): 'primary' | 'success' | 'danger' | 'warning' | 'info' {
  const map: Record<string, string> = {
    'Approve': 'success',
    'Reject': 'danger',
    'Submit': 'primary',
    'Cancel': 'info',
  };
  return (map[action] as any) || 'info';
}

function getActionTagType(action: string): string {
  const map: Record<string, string> = {
    'Approve': 'success',
    'Reject': 'danger',
    'Submit': '',
    'Cancel': 'info',
  };
  return map[action] || '';
}

handleSearch();
</script>

<style scoped lang="scss">
@use '@/styles/variables.scss' as *;

.page-container {
  padding: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.approval-info {
  margin-bottom: 8px;
}

.approval-actions {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}

.history-item {
  .history-title {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 4px;
  }

  .history-node {
    font-weight: 600;
    font-size: $wms-font-size-body;
  }

  .history-operator {
    font-size: $wms-font-size-small;
    color: $wms-text-secondary;
    margin-bottom: 2px;
  }

  .history-comment {
    font-size: $wms-font-size-small;
    color: $wms-text-regular;
    margin-top: 4px;
    padding: 4px 8px;
    background: $wms-bg-base;
    border-radius: $wms-radius-sm;
  }
}
</style>
