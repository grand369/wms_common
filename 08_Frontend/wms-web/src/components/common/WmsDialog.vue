<template>
  <el-dialog
    :model-value="visible"
    :title="title"
    :width="width"
    :destroy-on-close="destroyOnClose"
    :close-on-click-modal="closeOnClickModal"
    @update:model-value="handleVisibleChange"
    @close="handleClose"
  >
    <slot />
    <template #footer v-if="showFooter">
      <el-button @click="handleCancel">取消</el-button>
      <el-button type="primary" :loading="confirmLoading" @click="handleConfirm">
        确认
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
defineProps<{
  title: string;
  width?: string;
  visible: boolean;
  destroyOnClose?: boolean;
  closeOnClickModal?: boolean;
  showFooter?: boolean;
  confirmLoading?: boolean;
}>();

const emit = defineEmits<{
  close: [];
  cancel: [];
  confirm: [];
}>();

function handleVisibleChange() {
  emit('close');
}

function handleClose() {
  emit('close');
}

function handleCancel() {
  emit('cancel');
}

function handleConfirm() {
  emit('confirm');
}
</script>
