<template>
  <div class="wms-area-selector">
    <el-select
      v-model="selectedWarehouse"
      :disabled="disabled"
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
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getWarehouses, getAreas, getArea } from '@/api/warehouse'

interface WmsWarehouse {
  id: string
  code: string
  name: string
}

interface WmsArea {
  id: string
  code: string
  name: string
  warehouseId: string
  warehouseCode: string
}

const props = withDefaults(defineProps<{
  modelValue?: string
  warehouseId?: string
  disabled?: boolean
}>(), {
  modelValue: '',
  warehouseId: '',
  disabled: false
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
  change: [area: WmsArea | null]
  'warehouse-change': [warehouseId: string]
}>()

const selectedWarehouse = ref('')
const selectedArea = ref('')

const warehouseList = ref<WmsWarehouse[]>([])
const areaList = ref<WmsArea[]>([])

async function loadWarehouses() {
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
  } catch {
    ElMessage.error('加载仓库失败')
  }
}

async function loadAreas(warehouseId: string) {
  try {
    const res = await getAreas({ warehouseId, maxResultCount: 9999, skipCount: 0 })
    areaList.value = res.items.map(item => ({
      id: item.id,
      code: item.areaCode,
      name: item.areaName,
      warehouseId: item.warehouseId,
      warehouseCode: item.warehouseCode
    }))

    if (props.modelValue) {
      const matchedArea = areaList.value.find(a => a.id === props.modelValue)
      if (matchedArea) {
        selectedArea.value = props.modelValue
      }
    }
  } catch {
    ElMessage.error('加载库区失败')
  }
}

async function onWarehouseChange(warehouseId: string) {
  selectedArea.value = ''
  areaList.value = []
  emit('warehouse-change', warehouseId)

  if (warehouseId) {
    await loadAreas(warehouseId)
  }
}

function onAreaChange(areaId: string) {
  emit('update:modelValue', areaId)
  const area = areaList.value.find(item => item.id === areaId) || null
  emit('change', area)
}

watch(() => props.warehouseId, (newVal) => {
  if (newVal) {
    selectedWarehouse.value = newVal
    loadAreas(newVal)
  }
})

watch(() => props.modelValue, (newVal) => {
  if (newVal && areaList.value.length > 0) {
    const matchedArea = areaList.value.find(a => a.id === newVal)
    if (matchedArea) {
      selectedArea.value = newVal
      selectedWarehouse.value = matchedArea.warehouseId
    }
  }
})

async function initWithArea() {
  if (props.modelValue) {
    try {
      const area = await getArea(props.modelValue)
      selectedWarehouse.value = area.warehouseId
      selectedArea.value = area.id
      await loadAreas(area.warehouseId)
    } catch {
      ElMessage.error('加载库区信息失败')
    }
  }
}

onMounted(async () => {
  await loadWarehouses()
  await initWithArea()
})
</script>

<style scoped lang="scss">
.wms-area-selector {
  display: flex;
  gap: 8px;

  .el-select {
    flex: 1;
    min-width: 160px;
  }
}
</style>