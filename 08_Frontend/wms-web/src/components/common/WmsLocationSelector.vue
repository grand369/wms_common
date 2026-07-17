<template>
  <div class="wms-location-selector">
    <!-- 仓库选择 -->
    <el-select
      v-model="selectedWarehouse"
      :disabled="disabled || !!warehouseId"
      placeholder="请选择仓库"
      clearable
      filterable
      @change="onWarehouseChange"
    >
      <el-option
        v-for="item in warehouseList"
        :key="item.id"
        :label="`${item.code} - ${item.name}`"
        :value="item.id"
      />
    </el-select>

    <!-- 库区选择 -->
    <el-select
      v-model="selectedArea"
      :disabled="disabled || !selectedWarehouse"
      placeholder="请选择库区"
      clearable
      filterable
      @change="onAreaChange"
    >
      <el-option
        v-for="item in areaList"
        :key="item.id"
        :label="`${item.code} - ${item.name}`"
        :value="item.id"
      />
    </el-select>

    <!-- 库位选择 -->
    <el-select
      v-model="selectedLocation"
      :disabled="disabled || !selectedArea"
      placeholder="请选择库位"
      clearable
      filterable
      @change="onLocationChange"
    >
      <el-option
        v-for="item in locationList"
        :key="item.id"
        :label="`${item.code} - ${item.name}`"
        :value="item.id"
      />
    </el-select>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getWarehouses, getAreas, getLocationsByArea } from '@/api/warehouse'

/**
 * COMP-010 WmsLocationSelector - 三级级联库位选择器
 *
 * 仓库 → 库区 → 库位 三级级联选择
 * 支持预选仓库
 *
 * @props
 * - modelValue: 选中的库位ID
 * - warehouseId: 预选仓库ID（可选）
 * - disabled: 是否禁用
 *
 * @emits
 * - update:modelValue: 选中库位ID变化
 * - change: 选中库位变化，返回完整库位对象
 * - warehouse-change: 仓库选择变化
 * - area-change: 库区选择变化
 */

// 仓库接口
interface WmsWarehouse {
  id: string
  code: string
  name: string
}

// 库区接口
interface WmsArea {
  id: string
  code: string
  name: string
  warehouseId: string
}

// 库位接口
interface WmsLocation {
  id: string
  code: string
  name: string
  areaId: string
  warehouseId: string
}

// Props 定义
const props = withDefaults(defineProps<{
  modelValue?: string
  warehouseId?: string
  disabled?: boolean
}>(), {
  modelValue: '',
  warehouseId: '',
  disabled: false
})

// Emits 定义
const emit = defineEmits<{
  'update:modelValue': [value: string]
  change: [location: WmsLocation | null]
  'warehouse-change': [warehouseId: string]
  'area-change': [areaId: string]
}>()

// 选择状态
const selectedWarehouse = ref('')
const selectedArea = ref('')
const selectedLocation = ref('')

// 数据列表
const warehouseList = ref<WmsWarehouse[]>([])
const areaList = ref<WmsArea[]>([])
const locationList = ref<WmsLocation[]>([])

// 加载状态
const loading = ref(false)

// 加载仓库列表
async function loadWarehouses() {
  loading.value = true
  try {
    const res = await getWarehouses({ maxResultCount: 9999, skipCount: 0 })
    warehouseList.value = res.items.map(item => ({
      id: item.id,
      code: item.warehouseCode,
      name: item.warehouseName
    }))

    if (props.warehouseId) {
      selectedWarehouse.value = props.warehouseId
      await loadAreas(props.warehouseId)
    }
  } catch (error) {
    ElMessage.error('加载仓库失败')
  } finally {
    loading.value = false
  }
}

// 加载库区列表
async function loadAreas(warehouseId: string) {
  loading.value = true
  try {
    const res = await getAreas({ warehouseId, maxResultCount: 9999, skipCount: 0 })
    areaList.value = res.items.map(item => ({
      id: item.id,
      code: item.areaCode,
      name: item.areaName,
      warehouseId: item.warehouseId
    }))
  } catch (error) {
    ElMessage.error('加载库区失败')
  } finally {
    loading.value = false
  }
}

// 加载库位列表
async function loadLocations(areaId: string) {
  loading.value = true
  try {
    const res = await getLocationsByArea(areaId)
    locationList.value = res.items.map(item => ({
      id: item.id,
      code: item.locationCode,
      name: item.locationCode,
      areaId: item.areaId,
      warehouseId: item.warehouseId
    }))
  } catch (error) {
    ElMessage.error('加载库位失败')
  } finally {
    loading.value = false
  }
}

// 仓库变化
async function onWarehouseChange(warehouseId: string) {
  selectedArea.value = ''
  selectedLocation.value = ''
  areaList.value = []
  locationList.value = []
  emit('warehouse-change', warehouseId)

  if (warehouseId) {
    await loadAreas(warehouseId)
  }
}

// 库区变化
async function onAreaChange(areaId: string) {
  selectedLocation.value = ''
  locationList.value = []
  emit('area-change', areaId)

  if (areaId) {
    await loadLocations(areaId)
  }
}

// 库位变化
function onLocationChange(locationId: string) {
  emit('update:modelValue', locationId)
  const location = locationList.value.find(item => item.id === locationId) || null
  emit('change', location)
}

// 监听外部 warehouseId 变化
watch(() => props.warehouseId, (newVal) => {
  if (newVal) {
    selectedWarehouse.value = newVal
    loadAreas(newVal)
  }
})

// 初始化
onMounted(() => {
  loadWarehouses()
})
</script>

<style scoped lang="scss">
.wms-location-selector {
  display: flex;
  gap: 8px;

  .el-select {
    flex: 1;
    min-width: 160px;
  }
}
</style>
