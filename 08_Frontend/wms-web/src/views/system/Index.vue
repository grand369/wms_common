<template>
  <div class="system-container">
    <!-- Content Area -->
    <div class="system-content">
      <!-- ── Users Sub-page ──────────────────────────────── -->
      <template v-if="activeMenu === 'users'">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>用户管理</span>
              <el-button type="primary" @click="handleUserCreate">
                <el-icon><Plus /></el-icon> 新建用户
              </el-button>
            </div>
          </template>

          <el-form :inline="true" class="search-form">
            <el-form-item label="用户名">
              <el-input v-model="userFilters.userName" placeholder="请输入用户名" clearable />
            </el-form-item>
            <el-form-item label="姓名">
              <el-input v-model="userFilters.name" placeholder="请输入姓名" clearable />
            </el-form-item>
            <el-form-item label="状态">
              <el-select v-model="userFilters.isActive" placeholder="请选择状态" clearable>
                <el-option label="启用" :value="true" />
                <el-option label="禁用" :value="false" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleUserSearch">查询</el-button>
              <el-button @click="resetUserFilters">重置</el-button>
            </el-form-item>
          </el-form>

          <WmsTable
            :data="userTableData"
            :loading="userLoading"
            :total="userTotal"
            v-model:current-page="userPagination.currentPage"
            v-model:page-size="userPagination.pageSize"
            :page-sizes="userPagination.pageSizes"
            @page-change="handleUserPageChange"
            @size-change="handleUserSizeChange"
          >
            <el-table-column prop="userName" label="用户名" show-overflow-tooltip />
            <el-table-column prop="name" label="姓名" />
            <el-table-column prop="email" label="邮箱" show-overflow-tooltip />
            <el-table-column prop="phoneNumber" label="手机号" />
            <el-table-column prop="isActive" label="状态" align="center" width="90">
              <template #default="{ row }">
                <el-tag :type="row.isActive ? 'success' : 'danger'" size="small">
                  {{ row.isActive ? '启用' : '禁用' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="creationTime" label="创建时间" width="180" />
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="handleUserEdit(row)">编辑</el-button>
                <el-button
                  link
                  :type="row.isActive ? 'danger' : 'success'"
                  @click="handleToggleUserStatus(row)"
                >
                  {{ row.isActive ? '禁用' : '启用' }}
                </el-button>
              </template>
            </el-table-column>
          </WmsTable>
        </el-card>

        <!-- User Dialog -->
        <WmsDialog
          :title="userFormData.id ? '编辑用户' : '新建用户'"
          :visible="userDialogVisible"
          :confirm-loading="userSubmitting"
          show-footer
          width="600px"
          @close="closeUserDialog"
          @cancel="closeUserDialog"
          @confirm="handleUserSubmit"
        >
          <el-form ref="userFormRef" :model="userFormData" label-width="90px">
            <el-form-item label="用户名" required>
              <el-input v-model="userFormData.userName" placeholder="请输入用户名" />
            </el-form-item>
            <el-form-item label="姓名">
              <el-input v-model="userFormData.name" placeholder="请输入姓名" />
            </el-form-item>
            <el-form-item label="邮箱">
              <el-input v-model="userFormData.email" placeholder="请输入邮箱" />
            </el-form-item>
            <el-form-item label="手机号">
              <el-input v-model="userFormData.phoneNumber" placeholder="请输入手机号" />
            </el-form-item>
            <el-form-item label="启用状态">
              <el-switch v-model="userFormData.isActive" active-text="启用" inactive-text="禁用" />
            </el-form-item>
            <el-form-item v-if="!userFormData.id" label="密码">
              <el-input v-model="userFormData.password" type="password" placeholder="请输入密码" show-password />
            </el-form-item>
          </el-form>
        </WmsDialog>
      </template>

      <!-- ── Roles Sub-page ──────────────────────────────── -->
      <template v-if="activeMenu === 'roles'">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>角色管理</span>
              <el-button type="primary" @click="handleRoleCreate">
                <el-icon><Plus /></el-icon> 新建角色
              </el-button>
            </div>
          </template>

          <el-form :inline="true" class="search-form">
            <el-form-item label="角色名">
              <el-input v-model="roleFilters.name" placeholder="请输入角色名" clearable />
            </el-form-item>
            <el-form-item label="显示名">
              <el-input v-model="roleFilters.displayName" placeholder="请输入显示名" clearable />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="handleRoleSearch">查询</el-button>
              <el-button @click="resetRoleFilters">重置</el-button>
            </el-form-item>
          </el-form>

          <WmsTable
            :data="roleTableData"
            :loading="roleLoading"
            :total="roleTotal"
            v-model:current-page="rolePagination.currentPage"
            v-model:page-size="rolePagination.pageSize"
            :page-sizes="rolePagination.pageSizes"
            @page-change="handleRolePageChange"
            @size-change="handleRoleSizeChange"
          >
            <el-table-column prop="name" label="角色名" />
            <el-table-column prop="displayName" label="显示名" />
            <el-table-column prop="isDefault" label="默认角色" align="center" width="100">
              <template #default="{ row }">
                <el-tag :type="row.isDefault ? 'primary' : 'info'" size="small">
                  {{ row.isDefault ? '是' : '否' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="isPublic" label="公开角色" align="center" width="100">
              <template #default="{ row }">
                <el-tag :type="row.isPublic ? 'success' : 'info'" size="small">
                  {{ row.isPublic ? '是' : '否' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="200" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="handleRoleEdit(row)">编辑</el-button>
                <el-button link type="warning" @click="handleRoleAssignPermission(row)">权限分配</el-button>
              </template>
            </el-table-column>
          </WmsTable>
        </el-card>

        <!-- Role Dialog -->
        <WmsDialog
          :title="roleFormData.id ? '编辑角色' : '新建角色'"
          :visible="roleDialogVisible"
          :confirm-loading="roleSubmitting"
          show-footer
          width="500px"
          @close="closeRoleDialog"
          @cancel="closeRoleDialog"
          @confirm="handleRoleSubmit"
        >
          <el-form ref="roleFormRef" :model="roleFormData" label-width="90px">
            <el-form-item label="角色名" required>
              <el-input v-model="roleFormData.name" placeholder="请输入角色名" />
            </el-form-item>
            <el-form-item label="显示名" required>
              <el-input v-model="roleFormData.displayName" placeholder="请输入显示名" />
            </el-form-item>
            <el-form-item label="默认角色">
              <el-switch v-model="roleFormData.isDefault" />
            </el-form-item>
            <el-form-item label="公开角色">
              <el-switch v-model="roleFormData.isPublic" />
            </el-form-item>
          </el-form>
        </WmsDialog>

        <!-- Permission Assignment Dialog -->
        <WmsDialog
          :title="`权限分配 - ${permAssignRoleName}`"
          :visible="permAssignDialogVisible"
          :confirm-loading="permAssignSaving"
          show-footer
          width="720px"
          @close="closePermAssignDialog"
          @cancel="closePermAssignDialog"
          @confirm="handlePermAssignSave"
        >
          <div v-loading="permAssignLoading" class="perm-assign-tree">
            <el-tree
              ref="permAssignTreeRef"
              :data="permAssignGroups"
              :props="permAssignTreeProps"
              node-key="name"
              show-checkbox
              default-expand-all
              :default-checked-keys="permAssignGrantedKeys"
              :check-strictly="false"
            >
              <template #default="{ data }">
                <span class="perm-assign-node">
                  <span class="perm-assign-node-label">{{ data.displayName || data.name }}</span>
                  <el-tag v-if="data.isGranted" type="success" size="small">已授权</el-tag>
                </span>
              </template>
            </el-tree>
            <el-empty v-if="!permAssignLoading && permAssignGroups.length === 0" description="暂无权限数据" />
          </div>
        </WmsDialog>
      </template>

      <!-- ── Permissions Sub-page ─────────────────────────── -->
      <template v-if="activeMenu === 'permissions'">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>权限管理</span>
              <el-button type="primary" @click="handleRefreshPermissions">
                <el-icon><Refresh /></el-icon> 刷新
              </el-button>
            </div>
          </template>
          <div class="permission-tree" v-loading="permLoading">
            <el-tree
              :data="permissionTree"
              :props="permissionTreeProps"
              node-key="id"
              default-expand-all
              :highlight-current="true"
            >
              <template #default="{ data }">
                <span class="permission-node">
                  <el-icon><component :is="data.children && data.children.length > 0 ? FolderOpened : Document" /></el-icon>
                  <span class="permission-node-label">{{ data.displayName || data.name }}</span>
                  <el-tag v-if="data.isGranted" type="success" size="small" class="permission-tag">已授权</el-tag>
                </span>
              </template>
            </el-tree>
            <el-empty v-if="!permLoading && permissionTree.length === 0" description="暂无权限数据" />
          </div>
        </el-card>
      </template>

      <!-- ── Organization Sub-page ────────────────────────── -->
      <template v-if="activeMenu === 'organization'">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>组织架构</span>
              <el-button type="primary" @click="handleAddOrgRoot">
                <el-icon><Plus /></el-icon> 添加部门
              </el-button>
            </div>
          </template>
          <div class="org-tree" v-loading="orgLoading">
            <el-tree
              :data="orgTree"
              :props="orgTreeProps"
              node-key="id"
              default-expand-all
            >
              <template #default="{ data }">
                <span class="org-node">
                  <el-icon><FolderOpened /></el-icon>
                  <span class="org-node-label">{{ data.displayName || data.name }}</span>
                  <span class="org-node-actions">
                    <el-button link type="primary" size="small" @click.stop="handleEditOrg(data)">
                      <el-icon><Edit /></el-icon>
                    </el-button>
                    <el-button link type="primary" size="small" @click.stop="handleAddOrgChild(data)">
                      <el-icon><Plus /></el-icon>
                    </el-button>
                  </span>
                </span>
              </template>
            </el-tree>
            <el-empty v-if="!orgLoading && orgTree.length === 0" description="暂无组织数据" />
          </div>
        </el-card>

        <!-- Org Dialog -->
        <WmsDialog
          :title="orgFormData.id ? '编辑部门' : '新增部门'"
          :visible="orgDialogVisible"
          :confirm-loading="orgSubmitting"
          show-footer
          width="500px"
          @close="closeOrgDialog"
          @cancel="closeOrgDialog"
          @confirm="handleOrgSubmit"
        >
          <el-form ref="orgFormRef" :model="orgFormData" label-width="90px">
            <el-form-item label="部门名称" required>
              <el-input v-model="orgFormData.displayName" placeholder="请输入部门名称" />
            </el-form-item>
            <el-form-item label="上级部门">
              <el-input :model-value="orgFormData.parentName" disabled />
            </el-form-item>
            <el-form-item label="编码">
              <el-input v-model="orgFormData.code" placeholder="请输入编码(可选)" />
            </el-form-item>
          </el-form>
        </WmsDialog>
      </template>

      <!-- ── Settings Sub-page ─────────────────────────────── -->
      <template v-if="activeMenu === 'settings'">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>系统设置</span>
            </div>
          </template>
          <el-form
            ref="settingsFormRef"
            :model="settingsForm"
            label-width="120px"
            class="settings-form"
            v-loading="settingsLoading"
          >
            <el-form-item label="系统名称">
              <el-input v-model="settingsForm.systemName" placeholder="请输入系统名称" />
            </el-form-item>
            <el-form-item label="系统Logo">
              <el-upload
                action="#"
                :auto-upload="false"
                :show-file-list="false"
                accept="image/*"
              >
                <el-button type="primary">
                  <el-icon><Upload /></el-icon> 上传Logo
                </el-button>
              </el-upload>
            </el-form-item>
            <el-form-item label="默认语言">
              <el-select v-model="settingsForm.defaultLanguage" placeholder="请选择默认语言">
                <el-option label="简体中文" value="zh-CN" />
                <el-option label="English" value="en-US" />
              </el-select>
            </el-form-item>
            <el-form-item label="默认时区">
              <el-select v-model="settingsForm.timezone" placeholder="请选择时区">
                <el-option label="Asia/Shanghai (UTC+8)" value="Asia/Shanghai" />
                <el-option label="Asia/Tokyo (UTC+9)" value="Asia/Tokyo" />
                <el-option label="America/New_York (UTC-5)" value="America/New_York" />
                <el-option label="Europe/London (UTC+0)" value="Europe/London" />
              </el-select>
            </el-form-item>
            <el-form-item label="默认分页大小">
              <el-input-number
                v-model="settingsForm.defaultPageSize"
                :min="10"
                :max="100"
                :step="10"
              />
            </el-form-item>
            <el-form-item label="会话超时(分钟)">
              <el-input-number
                v-model="settingsForm.sessionTimeout"
                :min="5"
                :max="480"
                :step="5"
              />
            </el-form-item>
            <el-form-item label="启用操作日志">
              <el-switch v-model="settingsForm.enableAuditLog" />
            </el-form-item>
            <el-form-item label="启用通知">
              <el-switch v-model="settingsForm.enableNotification" />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" :loading="settingsSaving" @click="handleSaveSettings">
                <el-icon><Check /></el-icon> 保存设置
              </el-button>
              <el-button @click="handleResetSettings">重置</el-button>
            </el-form-item>
          </el-form>
        </el-card>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useRoute } from 'vue-router';
import {
  Plus,
  Refresh,
  FolderOpened,
  Document,
  Edit,
  Upload,
  Check,
} from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { useTable } from '@/hooks/useTable';
import { get, put } from '@/api';

const route = useRoute();

// ── Active Menu Detection ────────────────────────────────────
const activeMenu = computed(() => {
  const path = route.path;
  if (path.includes('/system/users')) return 'users';
  if (path.includes('/system/roles')) return 'roles';
  if (path.includes('/system/permissions')) return 'permissions';
  if (path.includes('/system/organization')) return 'organization';
  if (path.includes('/system/settings')) return 'settings';
  return 'users';
});

// =================================================================
// Users Sub-page
// =================================================================
interface UserDto {
  id: string;
  userName: string;
  name: string;
  email: string;
  phoneNumber: string;
  isActive: boolean;
  creationTime: string;
}

const {
  loading: userLoading,
  tableData: userTableData,
  total: userTotal,
  pagination: userPagination,
  filters: userFilters,
  handlePageChange: handleUserPageChange,
  handleSizeChange: handleUserSizeChange,
  handleSearch: handleUserSearch,
  resetFilters: resetUserFilters,
} = useTable<UserDto>('/api/identity/users');

// User Form
const userDialogVisible = ref(false);
const userSubmitting = ref(false);
const userFormRef = ref();
const userFormData = reactive<Partial<UserDto> & { password?: string }>({
  id: '',
  userName: '',
  name: '',
  email: '',
  phoneNumber: '',
  isActive: true,
  password: '',
});

function handleUserCreate() {
  Object.assign(userFormData, {
    id: '',
    userName: '',
    name: '',
    email: '',
    phoneNumber: '',
    isActive: true,
    password: '',
  });
  userDialogVisible.value = true;
}

function handleUserEdit(row: UserDto) {
  Object.assign(userFormData, { ...row, password: '' });
  userDialogVisible.value = true;
}

async function handleToggleUserStatus(row: UserDto) {
  try {
    await ElMessageBox.confirm(
      `确认${row.isActive ? '禁用' : '启用'}该用户？`,
      '提示',
      { type: 'warning' }
    );
    // API call would go here
    ElMessage.success('操作成功');
    handleUserSearch();
  } catch {
    // Canceled
  }
}

function closeUserDialog() {
  userDialogVisible.value = false;
}

async function handleUserSubmit() {
  userSubmitting.value = true;
  try {
    // API call would go here
    ElMessage.success(userFormData.id ? '更新成功' : '创建成功');
    closeUserDialog();
    handleUserSearch();
  } catch {
    ElMessage.error('操作失败');
  } finally {
    userSubmitting.value = false;
  }
}

// =================================================================
// Roles Sub-page
// =================================================================
interface RoleDto {
  id: string;
  name: string;
  displayName: string;
  isDefault: boolean;
  isPublic: boolean;
}

const {
  loading: roleLoading,
  tableData: roleTableData,
  total: roleTotal,
  pagination: rolePagination,
  filters: roleFilters,
  handlePageChange: handleRolePageChange,
  handleSizeChange: handleRoleSizeChange,
  handleSearch: handleRoleSearch,
  resetFilters: resetRoleFilters,
} = useTable<RoleDto>('/api/identity/roles');

// Role Form
const roleDialogVisible = ref(false);
const roleSubmitting = ref(false);
const roleFormRef = ref();
const roleFormData = reactive<Partial<RoleDto>>({
  id: '',
  name: '',
  displayName: '',
  isDefault: false,
  isPublic: false,
});

function handleRoleCreate() {
  Object.assign(roleFormData, {
    id: '',
    name: '',
    displayName: '',
    isDefault: false,
    isPublic: false,
  });
  roleDialogVisible.value = true;
}

function handleRoleEdit(row: RoleDto) {
  Object.assign(roleFormData, { ...row });
  roleDialogVisible.value = true;
}

function handleRoleAssignPermission(row: RoleDto) {
  permAssignRoleId.value = row.id;
  permAssignRoleName.value = row.displayName || row.name;
  permAssignDialogVisible.value = true;
  loadRolePermissions(row.name);
}

// ── Permission Assignment Dialog ──────────────────────────
const permAssignDialogVisible = ref(false);
const permAssignLoading = ref(false);
const permAssignSaving = ref(false);
const permAssignRoleId = ref('');
const permAssignRoleName = ref('');
const permAssignGroups = ref<any[]>([]);
const permAssignGrantedKeys = ref<string[]>([]);
const permAssignTreeRef = ref();
const permAssignTreeProps = {
  children: 'children',
  label: 'displayName',
};

/** Build a tree structure from ABP permission groups response */
function buildPermissionTree(groups: any[]): any[] {
  const tree: any[] = [];
  for (const group of groups) {
    const groupNode: any = {
      name: group.name,
      displayName: group.displayName,
      children: [],
    };
    // Build parent-child structure from flat permissions
    const permMap = new Map<string, any>();
    for (const perm of group.permissions || []) {
      permMap.set(perm.name, {
        name: perm.name,
        displayName: perm.displayName,
        parentName: perm.parentName,
        isGranted: perm.isGranted,
        children: [],
      });
    }
    // Attach children to parents, roots to group
    for (const [, permNode] of permMap) {
      if (permNode.parentName && permMap.has(permNode.parentName)) {
        permMap.get(permNode.parentName)!.children.push(permNode);
      } else {
        groupNode.children.push(permNode);
      }
    }
    tree.push(groupNode);
  }
  return tree;
}

/** Collect all granted leaf permission names from the groups */
function collectGrantedKeys(groups: any[]): string[] {
  const keys: string[] = [];
  for (const group of groups) {
    for (const perm of group.permissions || []) {
      // Only collect leaf permissions (those without children) as checked keys
      if (perm.isGranted && !perm.parentName) {
        // Root-level permission — add directly
        keys.push(perm.name);
      }
      // Child permissions will be auto-checked via parent
    }
  }
  // Alternative: collect ALL granted permissions as checked keys (check-strictly mode)
  // Since check-strictly=false, we need leaf nodes only
  const allGranted: string[] = [];
  for (const group of groups) {
    for (const perm of group.permissions || []) {
      if (perm.isGranted) {
        // Check if this permission has children in the same group
        const hasChildren = (group.permissions || []).some(
          (p: any) => p.parentName === perm.name
        );
        if (!hasChildren) {
          allGranted.push(perm.name);
        }
      }
    }
  }
  return allGranted;
}

async function loadRolePermissions(roleName: string) {
  permAssignLoading.value = true;
  permAssignGroups.value = [];
  permAssignGrantedKeys.value = [];
  try {
    const result = await get<any>(
      `/api/permission-management/permissions?providerName=R&providerKey=${roleName}`
    );
    const groups = result?.groups || [];
    permAssignGroups.value = buildPermissionTree(groups);
    permAssignGrantedKeys.value = collectGrantedKeys(groups);
  } catch (e: any) {
    ElMessage.error('加载权限数据失败: ' + (e.message || ''));
  } finally {
    permAssignLoading.value = false;
  }
}

function closePermAssignDialog() {
  permAssignDialogVisible.value = false;
  permAssignGroups.value = [];
  permAssignGrantedKeys.value = [];
}

async function handlePermAssignSave() {
  permAssignSaving.value = true;
  try {
    const tree = permAssignTreeRef.value;
    // Get all checked + half-checked nodes (half-checked = parent with some children checked)
    const checkedKeys = tree?.getCheckedKeys() || [];
    const halfCheckedKeys = tree?.getHalfCheckedKeys() || [];
    // Build permissions payload: all checked/half-checked are granted, everything else is revoked
    // ABP requires us to explicitly set isGranted for each permission
    const allKeys = [...checkedKeys, ...halfCheckedKeys];
    // Collect all permission names from groups to know the full set
    const allPermNames: string[] = [];
    for (const group of permAssignGroups.value) {
      for (const child of group.children || []) {
        allPermNames.push(child.name);
        for (const grandChild of child.children || []) {
          allPermNames.push(grandChild.name);
        }
      }
    }
    const permissions = allPermNames.map((name) => ({
      name,
      isGranted: allKeys.includes(name),
    }));

    const roleName = permAssignRoleName.value;
    await put(
      `/api/permission-management/permissions?providerName=R&providerKey=${roleName}`,
      { permissions }
    );
    ElMessage.success('权限分配成功');
    closePermAssignDialog();
    handleRoleSearch();
  } catch (e: any) {
    ElMessage.error('权限分配失败: ' + (e.message || ''));
  } finally {
    permAssignSaving.value = false;
  }
}

function closeRoleDialog() {
  roleDialogVisible.value = false;
}

async function handleRoleSubmit() {
  roleSubmitting.value = true;
  try {
    // API call would go here
    ElMessage.success(roleFormData.id ? '更新成功' : '创建成功');
    closeRoleDialog();
    handleRoleSearch();
  } catch {
    ElMessage.error('操作失败');
  } finally {
    roleSubmitting.value = false;
  }
}

// =================================================================
// Permissions Sub-page
// =================================================================
const permLoading = ref(false);
const permissionTree = ref<any[]>([]);
const permissionTreeProps = {
  children: 'children',
  label: 'displayName',
};

async function loadPermissions() {
  permLoading.value = true;
  try {
    const result = await get<any>('/api/permission-management/permissions');
    permissionTree.value = result?.items || result || [];
  } catch {
    // Silently handle errors
  } finally {
    permLoading.value = false;
  }
}

function handleRefreshPermissions() {
  loadPermissions();
}

// =================================================================
// Organization Sub-page
// =================================================================
const orgLoading = ref(false);
const orgTree = ref<any[]>([]);
const orgTreeProps = {
  children: 'children',
  label: 'displayName',
};

// Org Form
const orgDialogVisible = ref(false);
const orgSubmitting = ref(false);
const orgFormRef = ref();
const orgFormData = reactive<{
  id: string;
  displayName: string;
  parentId: string;
  parentName: string;
  code: string;
}>({
  id: '',
  displayName: '',
  parentId: '',
  parentName: '',
  code: '',
});

async function loadOrganization() {
  orgLoading.value = true;
  try {
    const result = await get<any>('/api/organization/units');
    orgTree.value = result?.items || result || [];
  } catch {
    // Silently handle errors
  } finally {
    orgLoading.value = false;
  }
}

function handleAddOrgRoot() {
  Object.assign(orgFormData, {
    id: '',
    displayName: '',
    parentId: '',
    parentName: '根节点',
    code: '',
  });
  orgDialogVisible.value = true;
}

function handleAddOrgChild(data: any) {
  Object.assign(orgFormData, {
    id: '',
    displayName: '',
    parentId: data.id,
    parentName: data.displayName || data.name,
    code: '',
  });
  orgDialogVisible.value = true;
}

function handleEditOrg(data: any) {
  Object.assign(orgFormData, {
    id: data.id,
    displayName: data.displayName || data.name,
    parentId: data.parentId || '',
    parentName: data.parentName || '根节点',
    code: data.code || '',
  });
  orgDialogVisible.value = true;
}

function closeOrgDialog() {
  orgDialogVisible.value = false;
}

async function handleOrgSubmit() {
  orgSubmitting.value = true;
  try {
    // API call would go here
    ElMessage.success(orgFormData.id ? '更新成功' : '创建成功');
    closeOrgDialog();
    loadOrganization();
  } catch {
    ElMessage.error('操作失败');
  } finally {
    orgSubmitting.value = false;
  }
}

// =================================================================
// Settings Sub-page
// =================================================================
const settingsLoading = ref(false);
const settingsSaving = ref(false);
const settingsFormRef = ref();
const settingsForm = reactive({
  systemName: 'WMS 仓储管理系统',
  defaultLanguage: 'zh-CN',
  timezone: 'Asia/Shanghai',
  defaultPageSize: 20,
  sessionTimeout: 30,
  enableAuditLog: true,
  enableNotification: true,
});

const originalSettings = reactive({ ...settingsForm });

async function loadSettings() {
  settingsLoading.value = true;
  try {
    const result = await get<any>('/api/settings');
    if (result) {
      Object.assign(settingsForm, result);
      Object.assign(originalSettings, result);
    }
  } catch {
    // Use defaults
  } finally {
    settingsLoading.value = false;
  }
}

async function handleSaveSettings() {
  settingsSaving.value = true;
  try {
    // API call: await put('/api/settings', settingsForm)
    ElMessage.success('设置保存成功');
    Object.assign(originalSettings, settingsForm);
  } catch {
    ElMessage.error('保存失败');
  } finally {
    settingsSaving.value = false;
  }
}

function handleResetSettings() {
  Object.assign(settingsForm, originalSettings);
  ElMessage.info('已重置为上次保存的设置');
}

// =================================================================
// Watchers
// =================================================================
watch(activeMenu, (menu) => {
  switch (menu) {
    case 'users':
      handleUserSearch();
      break;
    case 'roles':
      handleRoleSearch();
      break;
    case 'permissions':
      if (permissionTree.value.length === 0) loadPermissions();
      break;
    case 'organization':
      if (orgTree.value.length === 0) loadOrganization();
      break;
    case 'settings':
      loadSettings();
      break;
  }
});

// ── Initial Load ──────────────────────────────────────────────
onMounted(() => {
  if (activeMenu.value === 'users') handleUserSearch();
  else if (activeMenu.value === 'roles') handleRoleSearch();
});
</script>

<style lang="scss" scoped>
@use '@/styles/variables.scss' as *;

.system-container {
  height: 100%;
  background: $wms-bg-base;
}

.system-content {
  padding: $wms-spacing-lg;
  overflow-y: auto;
  min-width: 0;
}

// ── Card Header ──────────────────────────────────────────────
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

// ── Search Form ──────────────────────────────────────────────
.search-form {
  margin-bottom: $wms-spacing-md;
}

// ── Permission Tree ──────────────────────────────────────────
.permission-tree {
  min-height: 300px;
}

.permission-node {
  display: flex;
  align-items: center;
  gap: $wms-spacing-xs;

  .permission-node-label {
    flex: 1;
  }

  .permission-tag {
    margin-left: $wms-spacing-xs;
  }
}

// ── Organization Tree ────────────────────────────────────────
.org-tree {
  min-height: 300px;
}

.org-node {
  display: flex;
  align-items: center;
  gap: $wms-spacing-xs;
  width: 100%;

  .org-node-label {
    flex: 1;
  }

  .org-node-actions {
    opacity: 0;
    transition: opacity $wms-transition-hover;
    display: flex;
    gap: $wms-spacing-xxs;
  }

  &:hover .org-node-actions {
    opacity: 1;
  }
}

// ── Settings Form ────────────────────────────────────────────
.settings-form {
  max-width: 600px;
  padding: $wms-spacing-lg 0;
}

// ── Permission Assignment Dialog ───────────────────────────
.perm-assign-tree {
  min-height: 300px;
  max-height: 500px;
  overflow-y: auto;
}

.perm-assign-node {
  display: flex;
  align-items: center;
  gap: $wms-spacing-xs;
  width: 100%;

  .perm-assign-node-label {
    flex: 1;
  }
}

// ── Responsive ───────────────────────────────────────────────
@media (max-width: 768px) {
  .system-content {
    padding: $wms-spacing-md;
  }
}
</style>
