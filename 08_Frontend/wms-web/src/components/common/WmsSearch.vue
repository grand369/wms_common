<template>
  <el-form :inline="true" class="wms-search" @submit.prevent="handleSearch">
    <slot />
    <el-form-item>
      <el-button type="primary" @click="handleSearch">
        <el-icon><Search /></el-icon> Search
      </el-button>
      <el-button @click="handleReset">
        <el-icon><RefreshLeft /></el-icon> Reset
      </el-button>
    </el-form-item>
  </el-form>
</template>

<script setup lang="ts">
const emit = defineEmits<{
  search: [];
  reset: [];
}>();

function handleSearch() {
  emit('search');
}

function handleReset() {
  emit('reset');
}
</script>

<style scoped lang="scss">
.wms-search {
  margin-bottom: $spacing-md;
  background: $bg-white;
  padding: $spacing-md;
  border-radius: $radius-md;
  border: 1px solid $border-light;
}
</style>
