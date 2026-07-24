<template>
  <el-select
    :model-value="modelValue"
    :multiple="multiple"
    :disabled="disabled"
    filterable
    remote
    :remote-method="onSearch"
    :loading="loading"
    :placeholder="placeholder"
    clearable
    :collapse-tags="multiple"
    :max-collapse-tags="3"
    @update:model-value="onSelect"
    @change="onChange"
  >
    <el-option
      v-for="item in materialList"
      :key="item.id"
      :label="`${item.code} - ${item.name}`"
      :value="item.id"
    >
      <span class="material-option">
        <span class="material-code">{{ item.code }}</span>
        <span class="material-name">{{ item.name }}</span>
        <span v-if="item.specification" class="material-spec">
          {{ item.specification }}
        </span>
      </span>
    </el-option>

    <!-- 空状态 -->
    <template #empty>
      <span class="empty-text">请输入关键词搜索物料</span>
    </template>
  </el-select>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getMaterials } from '@/api/material'

/**
 * COMP-008 WmsMaterialSelector - 物料远程搜索选择器
 *
 * 支持远程搜索的物料选择组件
 * 基于 Element Plus el-select 封装
 *
 * @props
 * - modelValue: 选中的物料ID（支持多选）
 * - multiple: 是否多选
 * - disabled: 是否禁用
 * - placeholder: 占位文本
 *
 * @emits
 * - update:modelValue: 选中值变化
 * - change: 选中项变化，返回完整物料对象
 */

// 物料数据接口
export interface WmsMaterial {
  id: string
  code: string
  name: string
  specification?: string
  unit?: string
}

// Props 定义
const props = withDefaults(defineProps<{
  modelValue: string | string[]
  multiple?: boolean
  disabled?: boolean
  placeholder?: string
  materialInfo?: WmsMaterial
}>(), {
  multiple: false,
  disabled: false,
  placeholder: '请输入物料编码或名称搜索',
  materialInfo: undefined
})

// Emits 定义
const emit = defineEmits<{
  'update:modelValue': [value: string | string[]]
  change: [material: WmsMaterial | WmsMaterial[] | null]
}>()

// 物料列表
const materialList = ref<WmsMaterial[]>([])
// 加载状态
const loading = ref(false)

// 远程搜索
async function onSearch(query: string) {
  if (!query || query.length < 2) {
    materialList.value = []
    return
  }

  loading.value = true
  try {
    const res = await getMaterials({
      maxResultCount: 20,
      filter: query
    })
    materialList.value = res.items.map(item => ({
      id: item.id,
      code: item.materialCode,
      name: item.materialName,
      specification: item.specification,
      unit: item.primaryUnitName
    }))
  } catch (error) {
    ElMessage.error('搜索失败')
    materialList.value = []
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
    const selected = materialList.value.filter(item =>
      (value as string[]).includes(item.id)
    )
    emit('change', selected)
  } else {
    const selected = materialList.value.find(item => item.id === value) || null
    emit('change', selected)
  }
}

// 使用传入的物料信息初始化下拉选项
function initMaterialInfo() {
  if (props.materialInfo && props.materialInfo.id) {
    const existing = materialList.value.find(item => item.id === props.materialInfo!.id)
    if (!existing) {
      materialList.value.push({
        id: props.materialInfo.id,
        code: props.materialInfo.code,
        name: props.materialInfo.name,
        specification: props.materialInfo.specification,
        unit: props.materialInfo.unit
      })
    }
  }
}

// 初始加载
onMounted(() => {
  initMaterialInfo()
})

// 监听materialInfo变化
watch(() => props.materialInfo, () => {
  initMaterialInfo()
}, { deep: true })
</script>

<style scoped lang="scss">
.material-option {
  display: flex;
  align-items: center;
  gap: 8px;

  .material-code {
    font-weight: 600;
    color: #303133;
    min-width: 80px;
  }

  .material-name {
    color: #606266;
    flex: 1;
  }

  .material-spec {
    color: #909399;
    font-size: 12px;
  }
}

.empty-text {
  color: #909399;
  font-size: 14px;
}
</style>
