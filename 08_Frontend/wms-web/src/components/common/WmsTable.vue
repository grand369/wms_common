<template>
  <el-table
    v-loading="loading ?? false"
    :data="data"
    :border="border"
    :stripe="stripe"
    :row-key="rowKey"
    :default-sort="defaultSort"
    @selection-change="handleSelectionChange"
    @sort-change="handleSortChange"
    style="width: 100%"
  >
    <el-table-column v-if="selectable" type="selection" width="55" />
    <slot />
  </el-table>
  <el-pagination
    v-if="showPagination"
    class="wms-pagination"
    :current-page="currentPage"
    :page-size="pageSize"
    :page-sizes="pageSizes"
    :total="total"
    layout="total, sizes, prev, pager, next, jumper"
    @size-change="handleSizeChange"
    @current-change="handleCurrentChange"
  />
</template>

<script setup lang="ts">
import type { Sort } from 'element-plus';

interface TableSortChange {
  column: any;
  prop: string | null;
  order: 'ascending' | 'descending' | null;
}

withDefaults(defineProps<{
  data: any[];
  loading?: boolean;
  total?: number;
  currentPage?: number;
  pageSize?: number;
  pageSizes?: number[];
  rowKey?: string;
  border?: boolean;
  stripe?: boolean;
  selectable?: boolean;
  showPagination?: boolean;
  defaultSort?: Sort;
}>(), {
  showPagination: true,
  border: true,
  stripe: false,
  selectable: false,
});

const emit = defineEmits<{
  selectionChange: [selection: any[]];
  sortChange: [sort: TableSortChange];
  pageChange: [page: number];
  sizeChange: [size: number];
}>();

function handleSelectionChange(selection: any[]) {
  emit('selectionChange', selection);
}

function handleSortChange(sort: TableSortChange) {
  emit('sortChange', sort);
}

function handleCurrentChange(page: number) {
  emit('pageChange', page);
}

function handleSizeChange(size: number) {
  emit('sizeChange', size);
}
</script>

<style scoped lang="scss">
.wms-pagination {
  margin-top: $spacing-md;
  justify-content: flex-end;
}
</style>
