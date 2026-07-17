<template>
  <div class="page-container">
    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>库位地图</span>
          <WmsWarehouseSelector v-model="selectedWarehouseId" placeholder="选择仓库" style="width: 240px" />
        </div>
      </template>

      <WmsLocationMap
        :warehouse-id="selectedWarehouseId"
        :heat-map="true"
        @cell-click="handleCellClick"
      />
    </el-card>

    <el-drawer v-model="drawerVisible" title="库位详情" size="400px">
      <el-descriptions :column="1" border v-if="selectedLocation">
        <el-descriptions-item label="库位编码">{{ selectedLocation.code }}</el-descriptions-item>
        <el-descriptions-item label="库位名称">{{ selectedLocation.name }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <WmsStatusTag :status="selectedLocation.status || 'Available'" type="inventory" />
        </el-descriptions-item>
        <el-descriptions-item label="最大容量">{{ selectedLocation.maxCapacity }}</el-descriptions-item>
        <el-descriptions-item label="当前库存">{{ selectedLocation.currentQty }}</el-descriptions-item>
        <el-descriptions-item label="占用率">{{ Math.round((selectedLocation.currentQty / selectedLocation.maxCapacity) * 100) }}%</el-descriptions-item>
      </el-descriptions>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import WmsLocationMap from '@/components/common/WmsLocationMap.vue';
import WmsWarehouseSelector from '@/components/common/WmsWarehouseSelector.vue';
import WmsStatusTag from '@/components/common/WmsStatusTag.vue';
import type { WmsLocationMapItem } from '@/components/common/WmsLocationMap.vue';

const route = useRoute();
const selectedWarehouseId = ref<string>((route.params.warehouseId as string) || '');
const drawerVisible = ref(false);
const selectedLocation = ref<WmsLocationMapItem | null>(null);

function handleCellClick(cell: WmsLocationMapItem) {
  selectedLocation.value = cell;
  drawerVisible.value = true;
}
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
