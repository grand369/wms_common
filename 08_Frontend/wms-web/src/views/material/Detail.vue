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
            <el-descriptions-item label="物料编码">{{ material?.materialCode }}</el-descriptions-item>
            <el-descriptions-item label="物料名称">{{ material?.materialName }}</el-descriptions-item>
            <el-descriptions-item label="物料英文名">{{ material?.materialNameEn || '-' }}</el-descriptions-item>
            <el-descriptions-item label="分类">{{ material?.classificationName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="规格">{{ material?.specification || '-' }}</el-descriptions-item>
            <el-descriptions-item label="物料类型">{{ material?.materialTypeDescription || '-' }}</el-descriptions-item>
            <el-descriptions-item label="基本单位">{{ material?.primaryUnitName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="辅单位">{{ material?.secondaryUnitName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="主辅换算率">{{ material?.conversionRate || '-' }}</el-descriptions-item>
            <el-descriptions-item label="采购单位">{{ material?.purchaseUnitName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="库存单位">{{ material?.inventoryUnitName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="销售单位">{{ material?.salesUnitName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="状态">
              <WmsStatusTag v-if="material" :status="material.isActive ? 'Available' : 'Outsourced'" type="inventory" />
            </el-descriptions-item>
            <el-descriptions-item label="ERP同步状态">{{ material?.erpSyncStatusDescription || '-' }}</el-descriptions-item>
            <el-descriptions-item label="创建时间">{{ material?.creationTime || '-' }}</el-descriptions-item>
          </el-descriptions>
        </el-card>

        <el-card shadow="hover" class="card-mt">
          <template #header>
            <span>仓储属性</span>
          </template>
          <el-descriptions :column="3" border>
            <el-descriptions-item label="存储条件">{{ material?.storageConditionTypeDescription || '-' }}</el-descriptions-item>
            <el-descriptions-item label="最大堆叠层数">{{ material?.maxStackingLayers || '-' }}</el-descriptions-item>
            <el-descriptions-item label="包装规格">{{ material?.packageSpec || '-' }}</el-descriptions-item>
            <el-descriptions-item label="每单位重量">{{ material?.weightPerUnit || '-' }}</el-descriptions-item>
          </el-descriptions>
        </el-card>

        <el-card shadow="hover" class="card-mt">
          <template #header>
            <span>质量属性</span>
          </template>
          <el-descriptions :column="3" border>
            <el-descriptions-item label="批次管理">{{ material?.batchManagementEnabled ? '启用' : '关闭' }}</el-descriptions-item>
            <el-descriptions-item label="序列号管理">{{ material?.serialManagementEnabled ? '启用' : '关闭' }}</el-descriptions-item>
            <el-descriptions-item label="有效期管理">{{ material?.expiryManagementEnabled ? '启用' : '关闭' }}</el-descriptions-item>
            <el-descriptions-item label="保质期(天)">{{ material?.shelfLifeDays || '-' }}</el-descriptions-item>
            <el-descriptions-item label="质检方式">{{ material?.qualityInspectionModeDescription || '-' }}</el-descriptions-item>
          </el-descriptions>
        </el-card>

        <el-card shadow="hover" class="card-mt">
          <template #header>
            <span>库存属性</span>
          </template>
          <el-descriptions :column="3" border>
            <el-descriptions-item label="安全库存">{{ material?.safetyStockQuantity || '-' }}</el-descriptions-item>
            <el-descriptions-item label="最小订货量">{{ material?.minOrderQuantity || '-' }}</el-descriptions-item>
            <el-descriptions-item label="ABC分类">{{ material?.abcClassificationDescription || '-' }}</el-descriptions-item>
            <el-descriptions-item label="允许负库存">{{ material?.allowNegativeInventory ? '是' : '否' }}</el-descriptions-item>
          </el-descriptions>
        </el-card>

        <el-card shadow="hover" class="card-mt">
          <template #header>
            <span>发料策略</span>
          </template>
          <el-descriptions :column="3" border>
            <el-descriptions-item label="策略类型">{{ material?.issueStrategyTypeDescription || '-' }}</el-descriptions-item>
            <el-descriptions-item label="策略范围">{{ material?.strategyScopeDescription || '-' }}</el-descriptions-item>
          </el-descriptions>
        </el-card>

        <el-card shadow="hover" class="card-mt" v-if="material?.dangerLevel && material.dangerLevel > 0">
          <template #header>
            <span>危险品属性</span>
          </template>
          <el-descriptions :column="3" border>
            <el-descriptions-item label="危险等级">{{ material?.dangerLevelDescription || '-' }}</el-descriptions-item>
            <el-descriptions-item label="MSDS编号">{{ material?.msdsNumber || '-' }}</el-descriptions-item>
            <el-descriptions-item label="特殊标识">{{ material?.specialMark || '-' }}</el-descriptions-item>
          </el-descriptions>
        </el-card>
      </el-tab-pane>

      <el-tab-pane label="替代物料" name="bom">
        <el-card shadow="hover">
          <template #header>
            <div class="card-header">
              <span>替代物料</span>
              <el-button type="primary" @click="openBomDialog">维护替代物料</el-button>
            </div>
          </template>
          <el-table :data="bomLines" border v-loading="bomLoading">
            <el-table-column type="index" width="50" />
            <el-table-column prop="substituteMaterialCode" label="替代物料编码" />
            <el-table-column prop="substituteMaterialName" label="替代物料名称" />
            <el-table-column prop="substitutePriority" label="优先级" />
            <el-table-column prop="substituteRatio" label="替代比例" />
            <el-table-column label="操作" width="100">
              <template #default="{ row }">
                <el-button type="danger" link @click="handleRemoveSubstitute(row.id)">删除</el-button>
              </template>
            </el-table-column>
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
              <el-input-number v-model="row.substitutePriority" :min="1" />
            </template>
          </el-table-column>
          <el-table-column label="替代比例" width="120">
            <template #default="{ row }">
              <el-input-number v-model="row.substituteRatio" :min="0.01" :precision="2" />
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
import { getMaterial, getMaterialSubstitutes, addMaterialSubstitute, removeMaterialSubstitute } from '@/api/material';
import type { MaterialDto, MaterialSubstituteRelationDto } from '@/api/material';

const route = useRoute();
const router = useRouter();
const materialId = route.params.id as string;

const activeTab = ref('basic');
const material = ref<MaterialDto | null>(null);
const loading = ref(false);
const bomLoading = ref(false);
const bomLines = ref<MaterialSubstituteRelationDto[]>([]);
const bomDialogVisible = ref(false);
const bomSubmitting = ref(false);
const editableBomLines = ref<MaterialSubstituteRelationDto[]>([]);

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
  router.push('/material/list');
}

function openBomDialog() {
  editableBomLines.value = JSON.parse(JSON.stringify(bomLines.value));
  bomDialogVisible.value = true;
}

function addBomLine() {
  editableBomLines.value.push({
    id: '',
    originalMaterialId: '',
    substituteMaterialId: '',
    substituteMaterialCode: '',
    substituteMaterialName: '',
    substitutePriority: 1,
    substituteRatio: 1
  });
}

function removeBomLine(index: number) {
  editableBomLines.value.splice(index, 1);
}

function onMaterialChange(index: number, m: any) {
  if (m) {
    editableBomLines.value[index].substituteMaterialId = m.id;
    editableBomLines.value[index].substituteMaterialCode = m.materialCode;
    editableBomLines.value[index].substituteMaterialName = m.materialName;
  }
}

async function handleRemoveSubstitute(relationId: string) {
  try {
    await removeMaterialSubstitute(materialId, relationId);
    ElMessage.success('删除成功');
    loadBom();
  } catch {
    ElMessage.error('删除失败');
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
        substituteMaterialName: line.substituteMaterialName || '',
        priority: line.substitutePriority,
        ratio: line.substituteRatio
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
.card-mt {
  margin-top: 16px;
}
.bom-editor {
  padding: 8px 0;
}
</style>
