<template>
  <div class="page-container">
    <el-page-header @back="goBack" title="仓库详情" />

    <el-card shadow="hover" class="detail-header">
      <div class="header-content">
        <div>
          <h2 class="detail-title">{{ warehouse.warehouseName }} <WmsStatusTag :status="warehouse.isActive ? 'Available' : 'Outsourced'" type="inventory" /></h2>
          <p class="detail-subtitle">编码：{{ warehouse.warehouseCode }} | 类型：{{ warehouse.warehouseTypeDescription }}</p>
        </div>
        <div class="header-actions">
          <el-button @click="handleEdit">编辑</el-button>
        </div>
      </div>
    </el-card>

    <el-tabs v-model="activeTab" class="detail-tabs">
      <el-tab-pane label="基本信息" name="basic">
        <el-descriptions :column="3" border>
          <el-descriptions-item label="仓库编码">{{ warehouse.warehouseCode }}</el-descriptions-item>
          <el-descriptions-item label="仓库名称">{{ warehouse.warehouseName }}</el-descriptions-item>
          <el-descriptions-item label="仓库类型">{{ warehouse.warehouseTypeDescription }}</el-descriptions-item>
          <el-descriptions-item label="地址">{{ warehouse.address || '--' }}</el-descriptions-item>
          <el-descriptions-item label="备注">{{ warehouse.remark || '--' }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <WmsStatusTag :status="warehouse.isActive ? 'Available' : 'Outsourced'" type="inventory" />
          </el-descriptions-item>
        </el-descriptions>
      </el-tab-pane>

      <el-tab-pane label="库区列表" name="areas">
        <el-table :data="areas" border>
          <el-table-column prop="areaCode" label="库区编码" />
          <el-table-column prop="areaName" label="库区名称" />
          <el-table-column prop="areaFunction" label="功能类型">
            <template #default="{ row }">{{ getAreaFunctionLabel(row.areaFunction) }}</template>
          </el-table-column>
          <el-table-column prop="remark" label="备注" />
          <el-table-column prop="isActive" label="状态" align="center">
            <template #default="{ row }">
              <WmsStatusTag :status="row.isActive ? 'Available' : 'Outsourced'" type="inventory" />
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>

      <el-tab-pane label="库位树" name="tree">
        <el-tree :data="locationTree" :props="treeProps" node-key="id" default-expand-all />
      </el-tab-pane>

      <el-tab-pane label="统计概览" name="stats">
        <el-row :gutter="16">
          <el-col :span="6">
            <el-statistic title="库区总数" :value="areas.length" />
          </el-col>
          <el-col :span="6">
            <el-statistic title="库位总数" :value="locationCount" />
          </el-col>
        </el-row>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import { getWarehouse, getAreas, getLocations } from '@/api/warehouse';
import type { WarehouseDto, AreaDto, LocationDto } from '@/api/warehouse';

interface LocationTreeNode {
  id: string;
  name: string;
  children?: LocationTreeNode[];
}

const route = useRoute();
const router = useRouter();
const warehouseId = route.params.id as string;

const activeTab = ref('basic');
const warehouse = ref<Partial<WarehouseDto>>({});
const areas = ref<AreaDto[]>([]);
const locationTree = ref<LocationTreeNode[]>([]);
const locationCount = ref(0);

const treeProps = {
  label: 'name',
  children: 'children',
};

const areaFunctionLabels: Record<number, string> = {
  0: '收货区',
  1: '存储区',
  2: '拣选区',
  3: '发货区',
  4: '退货区',
  5: '其他',
};

function getAreaFunctionLabel(value: number): string {
  return areaFunctionLabels[value] || '未知';
}

function goBack() {
  router.back();
}

function handleEdit() {
  router.push('/warehouse/list');
}

async function loadDetail() {
  try {
    warehouse.value = await getWarehouse(warehouseId);
  } catch {
    ElMessage.error('加载仓库详情失败');
  }
}

async function loadAreas() {
  try {
    const res = await getAreas({ warehouseId });
    areas.value = res.items || [];
  } catch {
    ElMessage.error('加载库区列表失败');
  }
}

async function loadLocations() {
  try {
    const res = await getLocations({ areaId: areas.value[0]?.id });
    const items = res.items || [];
    locationCount.value = items.length;
    const grouped = new Map<string, LocationTreeNode>();
    items.forEach((loc: LocationDto) => {
      const areaName = areas.value.find((a) => a.id === loc.areaId)?.areaName || '默认库区';
      if (!grouped.has(areaName)) {
        grouped.set(areaName, { id: loc.areaId || areaName, name: areaName, children: [] });
      }
      grouped.get(areaName)!.children!.push({ id: loc.id, name: `${loc.locationCode}` });
    });
    locationTree.value = Array.from(grouped.values());
  } catch {
    ElMessage.error('加载库位树失败');
  }
}

onMounted(() => {
  loadDetail();
  loadAreas().then(() => loadLocations());
});
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.detail-header {
  margin-top: 16px;
}
.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.detail-title {
  margin: 0 0 8px 0;
  display: flex;
  align-items: center;
  gap: 8px;
}
.detail-subtitle {
  margin: 0;
  color: $text-secondary;
}
.detail-tabs {
  margin-top: 16px;
}
</style>
