<template>
  <div class="wms-barcode-input">
    <el-input
      :model-value="modelValue"
      :placeholder="placeholder"
      :disabled="disabled"
      :clearable="clearable"
      :maxlength="maxlength"
      @update:model-value="onInput"
      @keyup.enter="onScan"
      ref="inputRef"
    >
      <template #prefix>
        <el-icon><Aim /></el-icon>
      </template>
      <template #append>
        <el-button :icon="Search" @click="onScan" />
      </template>
    </el-input>

    <!-- 解析结果展示 -->
    <div v-if="parsedData" class="parse-result">
      <el-tag
        v-for="(value, key) in parsedData"
        :key="key"
        :type="getTagType(key)"
        size="small"
        class="result-tag"
      >
        {{ getLabel(key) }}: {{ value }}
      </el-tag>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Aim, Search } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

/**
 * COMP-017 WmsBarcodeInput - 条码输入组件
 *
 * 支持条码扫描输入和自动解析
 * 可解析物料编码、库位编码、托盘编码、任务编码
 *
 * @props
 * - modelValue: 输入值
 * - placeholder: 占位文本
 * - disabled: 是否禁用
 * - clearable: 是否可清除
 * - maxlength: 最大长度
 * - autoParse: 是否自动解析
 *
 * @emits
 * - update:modelValue: 输入值变化
 * - scan: 扫描完成，返回条码数据
 * - parse: 解析完成，返回解析结果
 */

// 条码解析结果接口
export interface WmsBarcodeData {
  type: 'material' | 'location' | 'pallet' | 'task' | 'unknown'
  code: string
  [key: string]: any
}

// Props 定义
const props = withDefaults(defineProps<{
  modelValue: string
  placeholder?: string
  disabled?: boolean
  clearable?: boolean
  maxlength?: number
  autoParse?: boolean
}>(), {
  placeholder: '请扫描或输入条码',
  disabled: false,
  clearable: true,
  maxlength: 100,
  autoParse: true
})

// Emits 定义
const emit = defineEmits<{
  'update:modelValue': [value: string]
  scan: [barcode: string]
  parse: [result: WmsBarcodeData | null]
}>()

// 输入框引用
const inputRef = ref()
// 解析结果
const parsedData = ref<Record<string, any> | null>(null)

// 输入处理
function onInput(value: string) {
  emit('update:modelValue', value)

  if (props.autoParse && value) {
    parseBarcode(value)
  }
}

// 扫描处理（回车或点击搜索）
function onScan() {
  const value = props.modelValue
  if (!value) {
    ElMessage.warning('请输入或扫描条码')
    return
  }

  emit('scan', value)
  parseBarcode(value)
}

// 解析条码
function parseBarcode(barcode: string) {
  // 条码解析规则：
  // - MAT开头: 物料编码
  // - LOC开头: 库位编码
  // - PAL开头: 托盘编码
  // - TSK开头: 任务编码

  let result: WmsBarcodeData | null = null

  if (barcode.startsWith('MAT-')) {
    result = {
      type: 'material',
      code: barcode,
      materialCode: barcode
    }
  } else if (barcode.startsWith('LOC-')) {
    result = {
      type: 'location',
      code: barcode,
      locationCode: barcode
    }
  } else if (barcode.startsWith('PAL-')) {
    result = {
      type: 'pallet',
      code: barcode,
      palletCode: barcode
    }
  } else if (barcode.startsWith('TSK-')) {
    result = {
      type: 'task',
      code: barcode,
      taskCode: barcode
    }
  } else {
    result = {
      type: 'unknown',
      code: barcode
    }
  }

  parsedData.value = result
  emit('parse', result)

  ElMessage.success(`条码解析成功: ${result.type}`)
}

// 获取标签类型
function getTagType(key: string) {
  const typeMap: Record<string, 'success' | 'warning' | 'info' | 'danger'> = {
    type: 'info',
    code: 'success',
    materialCode: 'success',
    locationCode: 'warning',
    palletCode: 'info',
    taskCode: 'danger'
  }
  return typeMap[key] || 'info'
}

// 获取字段标签
function getLabel(key: string) {
  const labelMap: Record<string, string> = {
    type: '类型',
    code: '条码',
    materialCode: '物料编码',
    locationCode: '库位编码',
    palletCode: '托盘编码',
    taskCode: '任务编码'
  }
  return labelMap[key] || key
}

// 清除解析结果
function clearParse() {
  parsedData.value = null
}

// 暴露方法
defineExpose({ clearParse })
</script>

<style scoped lang="scss">
.wms-barcode-input {
  width: 100%;
}

.parse-result {
  margin-top: 8px;
  display: flex;
  flex-wrap: wrap;
  gap: 8px;

  .result-tag {
    margin: 0;
  }
}
</style>
