<template>
  <el-select
    :model-value="modelValue"
    :multiple="multiple"
    :disabled="disabled"
    :placeholder="placeholder"
    filterable
    clearable
    :collapse-tags="multiple"
    :max-collapse-tags="3"
    @update:model-value="onSelect"
    @change="onChange"
  >
    <el-option
      v-for="item in warehouseList"
      :key="item.id"
      :label="`${item.code} - ${item.name}`"
      :value="item.id"
    >
      <span class="warehouse-option">
        <span class="warehouse-code">{{ item.code }}</span>
        <span class="warehouse-name">{{ item.name }}</span>
        <wms-status-tag
          v-if="item.status"
          :status="item.status"
          type="inventory"
          size="small"
        />
      </span>
    </el-option>

    <!-- 加载状态 -->
    <template #empty>
      <span v-if="loading">加载中...</span>
      <span v-else>暂无数据</span>
    </template>
  </el-select>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import WmsStatusTag from './WmsStatusTag.vue'
import { getWarehouses } from '@/api/warehouse'

/**
 * COMP-009 WmsWarehouseSelector - 仓库选择器组件
 *
 * 用于选择仓库的下拉选择器
 * 支持按组织过滤
 *
 * @props
 * - modelValue: 选中的仓库ID
 * - multiple: 是否多选
 * - disabled: 是否禁用
 * - filterByOrganization: 按组织ID过滤
 * - placeholder: 占位文本
 *
 * @emits
 * - update:modelValue: 选中值变化
 * - change: 选中项变化，返回完整仓库对象
 */

// 仓库数据接口
export interface WmsWarehouse {
  id: string
  code: string
  name: string
  organizationId?: string
  organizationName?: string
  status?: string
  type?: string
}

// Props 定义
const props = withDefaults(defineProps<{
  modelValue: string | string[]
  multiple?: boolean
  disabled?: boolean
  filterByOrganization?: string
  placeholder?: string
}>(), {
  multiple: false,
  disabled: false,
  placeholder: '请选择仓库'
})

// Emits 定义
const emit = defineEmits<{
  'update:modelValue': [value: string | string[]]
  change: [warehouse: WmsWarehouse | WmsWarehouse[] | null]
}>()

// 仓库列表
const warehouseList = ref<WmsWarehouse[]>([])
// 加载状态
const loading = ref(false)

// 加载仓库列表
async function loadWarehouses() {
  loading.value = true
  try {
    const params: any = { maxResultCount: 9999, skipCount: 0 }
    if (props.filterByOrganization) {
      params.organizationUnitId = props.filterByOrganization
    }
    const res = await getWarehouses(params)
    warehouseList.value = res.items.map(item => ({
      id: item.id,
      code: item.warehouseCode,
      name: item.warehouseName,
      organizationId: item.organizationUnitId,
      organizationName: item.organizationUnitName,
      status: item.isActive ? 'Available' : 'Outsourced',
      type: item.warehouseTypeDescription
    }))
  } catch (error) {
    ElMessage.error('加载仓库列表失败')
    warehouseList.value = []
  } finally {
    loading.value = false
  }
}

// 选择处理
function onSelect(value: string | string[]) {
  emit('update:modelValue', value)
}

// 变化处理
function onChange(value: string | string[]) {
  if (props.multiple) {
    const selected = warehouseList.value.filter(item =>
      (value as string[]).includes(item.id)
    )
    emit('change', selected)
  } else {
    const selected = warehouseList.value.find(item => item.id === value) || null
    emit('change', selected)
  }
}

// 监听组织过滤条件变化
watch(() => props.filterByOrganization, () => {
  loadWarehouses()
})

// 初始加载
onMounted(() => {
  loadWarehouses()
})
</script>

<style scoped lang="scss">
.warehouse-option {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;

  .warehouse-code {
    font-weight: 600;
    color: #303133;
    min-width: 60px;
  }

  .warehouse-name {
    color: #606266;
    flex: 1;
  }
}
</style>
