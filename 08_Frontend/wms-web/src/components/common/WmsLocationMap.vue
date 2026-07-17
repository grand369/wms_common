<template>
  <div class="wms-location-map">
    <!-- 筛选栏 -->
    <div v-if="showFilter" class="map-filter">
      <el-select
        v-model="selectedWarehouse"
        placeholder="选择仓库"
        clearable
        @change="loadLocations"
      >
        <el-option
          v-for="wh in warehouseList"
          :key="wh.id"
          :label="wh.name"
          :value="wh.id"
        />
      </el-select>

      <div class="filter-legend">
        <span class="legend-item">
          <span class="legend-color" style="background: #E8E8E8" />
          空位 (0-20%)
        </span>
        <span class="legend-item">
          <span class="legend-color" style="background: #67C23A" />
          正常 (20-80%)
        </span>
        <span class="legend-item">
          <span class="legend-color" style="background: #E6A23C" />
          较高 (80-90%)
        </span>
        <span class="legend-item">
          <span class="legend-color" style="background: #F56C6C" />
          满载 (>90%)
        </span>
      </div>
    </div>

    <!-- 库位网格 -->
    <div class="map-grid" :style="gridStyle">
      <div
        v-for="loc in displayLocations"
        :key="loc.id"
        class="map-cell"
        :class="getOccupancyClass(loc)"
        :title="`${loc.code}: ${getOccupancyText(loc)}`"
        @click="onCellClick(loc)"
      >
        <div class="cell-code">{{ loc.code }}</div>
        <div class="cell-occupancy">{{ getOccupancyPercent(loc) }}%</div>
      </div>
    </div>

    <!-- 空状态 -->
    <el-empty
      v-if="!displayLocations || displayLocations.length === 0"
      description="请选择仓库查看库位地图"
    />

    <!-- 库位详情抽屉 -->
    <el-drawer
      v-model="drawerVisible"
      :title="selectedLocation?.code + ' - 库位详情'"
      size="400px"
    >
      <div v-if="selectedLocation" class="location-detail">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="库位编码">
            {{ selectedLocation.code }}
          </el-descriptions-item>
          <el-descriptions-item label="库位名称">
            {{ selectedLocation.name }}
          </el-descriptions-item>
          <el-descriptions-item label="占用率">
            <el-progress
              :percentage="getOccupancyPercent(selectedLocation)"
              :color="getProgressColor(selectedLocation)"
            />
          </el-descriptions-item>
          <el-descriptions-item label="当前库存">
            {{ selectedLocation.currentQty || 0 }}
          </el-descriptions-item>
          <el-descriptions-item label="最大容量">
            {{ selectedLocation.maxCapacity || '-' }}
          </el-descriptions-item>
        </el-descriptions>
      </div>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getWarehouses, getLocations } from '@/api/warehouse'

/**
 * COMP-012 WmsLocationMap - 可视化库位地图组件
 *
 * 库位占用热力图，使用 CSS Grid 布局
 * 颜色编码: 空位(0-20%) → 绿(20-80%) → 黄(80-90%) → 红(>90%)
 *
 * @props
 * - warehouseId: 仓库ID
 * - locations: 库位数组（包含占用数据）
 * - showFilter: 是否显示筛选栏
 * - columns: 网格列数
 *
 * @emits
 * - cell-click: 库位单元格点击
 */

// 库位数据接口
export interface WmsLocationMapItem {
  id: string
  code: string
  name?: string
  currentQty: number
  maxCapacity: number
  status?: string
}

// Props 定义
const props = withDefaults(defineProps<{
  warehouseId?: string
  locations?: WmsLocationMapItem[]
  showFilter?: boolean
  columns?: number
}>(), {
  showFilter: true,
  columns: 8
})

// Emits 定义
const emit = defineEmits<{
  'cell-click': [location: WmsLocationMapItem]
  'warehouse-change': [warehouseId: string]
}>()

// 仓库列表
const warehouseList = ref<Array<{ id: string; name: string }>>([])
// 选中的仓库
const selectedWarehouse = ref('')
// 显示的库位
const displayLocations = ref<WmsLocationMapItem[]>([])
// 抽屉显示
const drawerVisible = ref(false)
// 选中的库位
const selectedLocation = ref<WmsLocationMapItem | null>(null)

// 网格样式
const gridStyle = computed(() => ({
  gridTemplateColumns: `repeat(${props.columns}, 1fr)`
}))

// 加载仓库列表
async function loadWarehouses() {
  try {
    const res = await getWarehouses({ maxResultCount: 9999, skipCount: 0 })
    warehouseList.value = res.items.map(item => ({
      id: item.id,
      name: item.warehouseName
    }))
  } catch {
    warehouseList.value = []
  }
}

// 加载库位
async function loadLocations(warehouseId: string) {
  emit('warehouse-change', warehouseId)
  try {
    const res = await getLocations({ warehouseId, maxResultCount: 9999, skipCount: 0 })
    displayLocations.value = res.items.map(item => ({
      id: item.id,
      code: item.locationCode,
      name: item.locationCode,
      currentQty: item.currentCapacity || 0,
      maxCapacity: item.maxCapacity || 0,
      status: item.isActive ? 'Available' : 'Outsourced'
    }))
  } catch {
    displayLocations.value = []
  }
}

// 获取占用率
function getOccupancyPercent(loc: WmsLocationMapItem) {
  if (!loc.maxCapacity) return 0
  return Math.round((loc.currentQty / loc.maxCapacity) * 100)
}

// 获取占用率文本
function getOccupancyText(loc: WmsLocationMapItem) {
  return `${loc.currentQty}/${loc.maxCapacity} (${getOccupancyPercent(loc)}%)`
}

// 获取占用率样式类
function getOccupancyClass(loc: WmsLocationMapItem) {
  const percent = getOccupancyPercent(loc)
  if (percent <= 20) return 'occupancy-empty'
  if (percent <= 80) return 'occupancy-normal'
  if (percent <= 90) return 'occupancy-high'
  return 'occupancy-full'
}

// 获取进度条颜色
function getProgressColor(loc: WmsLocationMapItem) {
  const percent = getOccupancyPercent(loc)
  if (percent <= 20) return '#E8E8E8'
  if (percent <= 80) return '#67C23A'
  if (percent <= 90) return '#E6A23C'
  return '#F56C6C'
}

// 单元格点击
function onCellClick(loc: WmsLocationMapItem) {
  selectedLocation.value = loc
  drawerVisible.value = true
  emit('cell-click', loc)
}

// 监听外部 locations 数据
import { watch } from 'vue'
watch(() => props.locations, (newVal) => {
  if (newVal) {
    displayLocations.value = newVal
  }
}, { immediate: true })

// 初始化
onMounted(() => {
  loadWarehouses()
  if (props.warehouseId) {
    selectedWarehouse.value = props.warehouseId
    loadLocations(props.warehouseId)
  }
})
</script>

<style scoped lang="scss">
.wms-location-map {
  .map-filter {
    display: flex;
    align-items: center;
    gap: 24px;
    margin-bottom: 16px;
    flex-wrap: wrap;
  }

  .filter-legend {
    display: flex;
    gap: 16px;
    font-size: 12px;
    color: #606266;
  }

  .legend-item {
    display: flex;
    align-items: center;
    gap: 4px;
  }

  .legend-color {
    width: 16px;
    height: 16px;
    border-radius: 2px;
    display: inline-block;
  }

  .map-grid {
    display: grid;
    gap: 8px;
    padding: 16px;
    background: #F5F7FA;
    border-radius: 4px;
    min-height: 300px;
  }

  .map-cell {
    padding: 12px 8px;
    border-radius: 4px;
    text-align: center;
    cursor: pointer;
    transition: all 0.3s;
    border: 1px solid #DCDFE6;

    &:hover {
      transform: scale(1.05);
      box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
    }

    &.occupancy-empty {
      background: #E8E8E8;
      color: #909399;
    }

    &.occupancy-normal {
      background: #67C23A;
      color: white;
    }

    &.occupancy-high {
      background: #E6A23C;
      color: white;
    }

    &.occupancy-full {
      background: #F56C6C;
      color: white;
    }
  }

  .cell-code {
    font-weight: 600;
    font-size: 13px;
    margin-bottom: 4px;
  }

  .cell-occupancy {
    font-size: 12px;
  }

  .location-detail {
    padding: 16px;
  }
}
</style>
