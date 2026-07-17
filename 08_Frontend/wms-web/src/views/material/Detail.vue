<template>
  <div class="page-container">
    <el-page-header title="返回" content="物料详情" @back="goBack" />

    <el-tabs v-model="activeTab" type="border-card" class="detail-tabs">
      <el-tab-pane label="基本信息" name="basic">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>物料基本信息</span>
              <el-button type="primary" @click="handleEdit">编辑</el-button>
            </div>
          </template>
          <el-descriptions :column="3" border v-loading="loading">
            <el-descriptions-item label="物料编码">{{ material?.code }}</el-descriptions-item>
            <el-descriptions-item label="物料名称">{{ material?.name }}</el-descriptions-item>
            <el-descriptions-item label="分类">{{ material?.classificationName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="基本单位">{{ material?.unit }}</el-descriptions-item>
            <el-descriptions-item label="规格">{{ material?.specification || '-' }}</el-descriptions-item>
            <el-descriptions-item label="状态">
              <WmsStatusTag v-if="material" :status="material.status === 1 ? 'Available' : 'Outsourced'" type="inventory" />
            </el-descriptions-item>
            <el-descriptions-item label="批次管理">{{ material?.isBatchEnabled ? '启用' : '关闭' }}</el-descriptions-item>
            <el-descriptions-item label="序列号管理">{{ material?.isSerialEnabled ? '启用' : '关闭' }}</el-descriptions-item>
          </el-descriptions>
        </el-card>
      </el-tab-pane>

      <el-tab-pane label="替代物料" name="bom">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>替代物料</span>
              <el-button type="primary" @click="bomDialogVisible = true">维护替代物料</el-button>
            </div>
          </template>
          <el-table :data="bomLines" border v-loading="bomLoading">
            <el-table-column type="index" width="50" />
            <el-table-column prop="substituteMaterialCode" label="替代物料编码" />
            <el-table-column prop="substituteMaterialName" label="替代物料名称" />
            <el-table-column prop="priority" label="优先级" />
            <el-table-column prop="ratio" label="替代比例" />
          </el-table>
        </el-card>
      </el-tab-pane>
    </el-tabs>

    <WmsDialog
      title="维护替代物料"
      :visible="bomDialogVisible"
      show-footer
      width="800px"
      :confirm-loading="bomSubmitting"
      @close="bomDialogVisible = false"
      @cancel="bomDialogVisible = false"
      @confirm="handleSaveBom"
    >
      <div class="bom-editor">
        <el-button type="primary" @click="addBomLine">添加替代物料</el-button>
        <el-table :data="editableBomLines" border style="margin-top: 16px">
          <el-table-column type="index" width="50" />
          <el-table-column label="替代物料" min-width="200">
            <template #default="{ row, $index }">
              <WmsMaterialSelector v-model="row.substituteMaterialId" @change="(m) => onMaterialChange($index, m)" />
            </template>
          </el-table-column>
          <el-table-column prop="substituteMaterialName" label="物料名称" />
          <el-table-column label="优先级" width="120">
            <template #default="{ row }">
              <el-input-number v-model="row.priority" :min="1" />
            </template>
          </el-table-column>
          <el-table-column label="替代比例" width="120">
            <template #default="{ row }">
              <el-input-number v-model="row.ratio" :min="0.01" :precision="2" />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="100">
            <template #default="{ $index }">
              <el-button type="danger" link @click="removeBomLine($index)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import WmsMaterialSelector from '@/components/common/WmsMaterialSelector.vue';
import { getMaterial, getMaterialSubstitutes, addMaterialSubstitute } from '@/api/material';
import type { MaterialDto, MaterialSubstituteDto } from '@/api/material';

const route = useRoute();
const router = useRouter();
const materialId = route.params.id as string;

const activeTab = ref('basic');
const material = ref<MaterialDto | null>(null);
const loading = ref(false);
const bomLoading = ref(false);
const bomLines = ref<MaterialSubstituteDto[]>([]);
const bomDialogVisible = ref(false);
const bomSubmitting = ref(false);
const editableBomLines = ref<MaterialSubstituteDto[]>([]);

async function loadMaterial() {
  loading.value = true;
  try {
    material.value = await getMaterial(materialId);
  } catch {
    ElMessage.error('加载物料失败');
  } finally {
    loading.value = false;
  }
}

async function loadBom() {
  bomLoading.value = true;
  try {
    const res = await getMaterialSubstitutes(materialId);
    bomLines.value = res.items;
  } catch {
    ElMessage.error('加载替代物料失败');
  } finally {
    bomLoading.value = false;
  }
}

function goBack() {
  router.push('/material/list');
}

function handleEdit() {
  router.push(`/material/list`);
}

function addBomLine() {
  editableBomLines.value.push({ id: '', materialId: '', substituteMaterialId: '', substituteMaterialCode: '', substituteMaterialName: '', priority: 1, ratio: 1 });
}

function removeBomLine(index: number) {
  editableBomLines.value.splice(index, 1);
}

function onMaterialChange(index: number, m: any) {
  if (m) {
    editableBomLines.value[index].substituteMaterialId = m.id;
    editableBomLines.value[index].substituteMaterialCode = m.code;
    editableBomLines.value[index].substituteMaterialName = m.name;
  }
}

async function handleSaveBom() {
  const validLines = editableBomLines.value.filter((l) => l.substituteMaterialId);
  if (validLines.length === 0) {
    ElMessage.warning('请至少填写一行有效的替代物料');
    return;
  }
  bomSubmitting.value = true;
  try {
    for (const line of validLines) {
      await addMaterialSubstitute(materialId, {
        substituteMaterialId: line.substituteMaterialId,
        substituteMaterialCode: line.substituteMaterialCode || '',
        priority: line.priority,
        ratio: line.ratio
      });
    }
    ElMessage.success('替代物料保存成功');
    bomDialogVisible.value = false;
    loadBom();
  } catch {
    ElMessage.error('替代物料保存失败');
  } finally {
    bomSubmitting.value = false;
  }
}

onMounted(() => {
  loadMaterial();
  loadBom();
  editableBomLines.value = [...bomLines.value];
});
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.detail-tabs {
  margin-top: 16px;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.bom-editor {
  padding: 8px 0;
}
</style>
