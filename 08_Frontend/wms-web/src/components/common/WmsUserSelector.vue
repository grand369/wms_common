<template>
  <el-dialog
    v-model="visible"
    :title="title"
    width="800px"
    destroy-on-close
    @open="handleOpen"
  >
    <div class="user-selector">
      <el-form :inline="true" class="search-form">
        <el-form-item label="用户名">
          <el-input v-model="filters.userName" placeholder="请输入用户名" clearable />
        </el-form-item>
        <el-form-item label="姓名">
          <el-input v-model="filters.name" placeholder="请输入姓名" clearable />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="searchUsers">搜索</el-button>
          <el-button @click="resetFilters">重置</el-button>
        </el-form-item>
      </el-form>

      <el-table
        v-loading="loading"
        :data="users"
        border
        highlight-current-row
        @current-change="handleCurrentChange"
        @row-click="handleRowClick"
      >
        <el-table-column prop="userName" label="用户名" width="150" />
        <el-table-column prop="name" label="姓名" width="120" />
        <el-table-column prop="email" label="邮箱" width="200" />
        <el-table-column prop="phoneNumber" label="手机号" width="130" />
        <el-table-column prop="isActive" label="状态" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">
              {{ row.isActive ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination">
        <el-pagination
          v-model:current-page="pagination.currentPage"
          v-model:page-size="pagination.pageSize"
          :total="total"
          :page-sizes="[10, 20, 50]"
          layout="total, sizes, prev, pager, next"
          @size-change="searchUsers"
          @current-change="searchUsers"
        />
      </div>
    </div>

    <template #footer>
      <el-button @click="visible = false">取消</el-button>
      <el-button type="primary" :disabled="!selectedUser" @click="confirmSelect">确定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue';
import { get } from '@/api';

export interface WmsUser {
  id: string;
  userName: string;
  name: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
}

const props = withDefaults(defineProps<{
  modelValue?: boolean;
  title?: string;
}>(), {
  modelValue: false,
  title: '选择用户',
});

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  'select': [user: WmsUser];
}>();

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
});

const loading = ref(false);
const users = ref<WmsUser[]>([]);
const total = ref(0);
const selectedUser = ref<WmsUser | null>(null);

const filters = reactive({
  userName: '',
  name: '',
});

const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
});

async function handleOpen() {
  selectedUser.value = null;
  filters.userName = '';
  filters.name = '';
  pagination.currentPage = 1;
  await searchUsers();
}

async function searchUsers() {
  loading.value = true;
  try {
    const params = {
      userName: filters.userName || undefined,
      name: filters.name || undefined,
      skipCount: (pagination.currentPage - 1) * pagination.pageSize,
      maxResultCount: pagination.pageSize,
    };
    const result = await get<{ items: WmsUser[]; totalCount: number }>('/api/identity/users', { params });
    users.value = result.items;
    total.value = result.totalCount;
  } catch {
    // Error handled by interceptor
  } finally {
    loading.value = false;
  }
}

function resetFilters() {
  filters.userName = '';
  filters.name = '';
  pagination.currentPage = 1;
  searchUsers();
}

function handleCurrentChange(row: WmsUser | null) {
  selectedUser.value = row;
}

function handleRowClick(row: WmsUser) {
  selectedUser.value = row;
}

function confirmSelect() {
  if (selectedUser.value) {
    emit('select', selectedUser.value);
    visible.value = false;
  }
}
</script>

<style scoped lang="scss">
.user-selector {
  min-height: 300px;
}
.search-form {
  margin-bottom: 16px;
}
.pagination {
  margin-top: 16px;
  display: flex;
  justify-content: flex-end;
}
</style>