<template>
  <div class="node-table-editor">
    <div class="editor-toolbar">
      <el-button type="primary" size="small" @click="handleAdd">
        <el-icon><Plus /></el-icon> 添加节点
      </el-button>
    </div>
    <el-table :data="modelValue" border size="small" class="node-table">
      <el-table-column type="index" width="50" label="序号" />
      <el-table-column prop="nodeType" label="节点类型" min-width="130">
        <template #default="{ row }">
          <el-select v-model="row.nodeType" placeholder="节点类型" size="small">
            <el-option label="提交" value="Submit" />
            <el-option label="审批" value="Approve" />
            <el-option label="会签" value="Countersign" />
            <el-option label="通知" value="Notify" />
          </el-select>
        </template>
      </el-table-column>
      <el-table-column prop="approverName" label="审批人" min-width="160">
        <template #default="{ row }">
          <el-input
            v-model="row.approverName"
            placeholder="输入审批人姓名"
            size="small"
            @change="(v: string) => { row.approverId = row.approverId || ''; }"
          />
        </template>
      </el-table-column>
      <el-table-column label="操作" width="100" fixed="right">
        <template #default="{ $index }">
          <el-button link type="danger" size="small" @click="handleRemove($index)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-empty v-if="modelValue.length === 0" description="暂无审批节点，请点击「添加节点」" />
  </div>
</template>

<script setup lang="ts">
import { Plus } from '@element-plus/icons-vue';
import type { WorkflowNodeDto } from '@/api/workflow';

const props = defineProps<{
  modelValue: WorkflowNodeDto[];
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: WorkflowNodeDto[]): void;
}>();

function handleAdd() {
  const newNode: WorkflowNodeDto = {
    id: `node_${Date.now()}_${Math.random().toString(36).slice(2, 8)}`,
    nodeType: 'Approve',
    approverId: '',
    approverName: '',
    nextNodeId: undefined,
  };
  emit('update:modelValue', [...props.modelValue, newNode]);
}

function handleRemove(index: number) {
  const updated = [...props.modelValue];
  updated.splice(index, 1);
  emit('update:modelValue', updated);
}
</script>

<style scoped lang="scss">
@use '@/styles/variables.scss' as *;

.node-table-editor {
  min-width: 500px;
}

.editor-toolbar {
  margin-bottom: $wms-spacing-sm;
}

.node-table {
  width: 100%;
}
</style>
