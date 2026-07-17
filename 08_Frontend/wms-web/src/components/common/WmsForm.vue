<template>
  <el-form
    ref="formRef"
    :model="formData"
    :rules="computedRules"
    :label-width="labelWidth"
    :disabled="disabled"
    class="wms-form"
  >
    <!-- 分组表单 -->
    <template v-if="groups && groups.length > 0">
      <template v-for="(group, groupIndex) in groups" :key="groupIndex">
        <el-divider v-if="group.title" content-position="left">
          {{ group.title }}
        </el-divider>
        <el-row :gutter="20">
          <el-col
            v-for="item in getGroupItems(group)"
            :key="item.prop"
            :span="item.span || 24"
          >
            <el-form-item
              :label="item.label"
              :prop="item.prop"
              :rules="item.rules"
            >
              <component
                :is="getComponent(item)"
                v-model="formData[item.prop]"
                v-bind="getComponentProps(item)"
                @change="onFieldChange(item.prop, $event)"
              />
            </el-form-item>
          </el-col>
        </el-row>
      </template>
    </template>

    <!-- 无分组表单 -->
    <template v-else>
      <el-row :gutter="20">
        <el-col
          v-for="item in formItems"
          :key="item.prop"
          :span="item.span || 24"
        >
          <el-form-item
            :label="item.label"
            :prop="item.prop"
            :rules="item.rules"
          >
            <component
              :is="getComponent(item)"
              v-model="formData[item.prop]"
              v-bind="getComponentProps(item)"
              @change="onFieldChange(item.prop, $event)"
            />
          </el-form-item>
        </el-col>
      </el-row>
    </template>
  </el-form>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import type { FormInstance, FormItemRule } from 'element-plus'

/**
 * COMP-002 WmsForm - 通用表单包装器组件
 *
 * 基于 Element Plus el-form 封装的业务表单组件
 * 支持通过配置数组自动生成表单项，支持分组显示
 *
 * @props
 * - formItems: 表单项配置数组
 * - formData: 表单数据对象
 * - formRules: 表单验证规则（可选，也可用 item.rules）
 * - labelWidth: 标签宽度，默认 '120px'
 * - groups: 分组配置数组（可选）
 * - disabled: 是否禁用整个表单
 *
 * @emits
 * - change: 表单项值变化时触发
 * - validate: 验证完成时触发
 */

// 表单项配置接口
export interface WmsFormItem {
  prop: string
  label: string
  type: 'input' | 'select' | 'date' | 'number' | 'switch' | 'textarea' | 'remote-search' | 'cascader' | 'datetime' | 'daterange'
  span?: number
  group?: string
  rules?: FormItemRule | FormItemRule[]
  placeholder?: string
  // select 相关
  options?: Array<{ label: string; value: any }>
  // input 相关
  maxlength?: number
  showWordLimit?: boolean
  // number 相关
  min?: number
  max?: number
  precision?: number
  // date 相关
  dateType?: string
  // textarea 相关
  rows?: number
  // remote-search 相关
  remoteApi?: string
  // 通用属性
  clearable?: boolean
  filterable?: boolean
  disabled?: boolean
}

// 分组配置接口
export interface WmsFormGroup {
  title: string
  fields: string[] // 该分组包含的 prop 列表
}

// Props 定义
const props = withDefaults(defineProps<{
  formItems: WmsFormItem[]
  formData: Record<string, any>
  formRules?: Record<string, FormItemRule | FormItemRule[]>
  labelWidth?: string
  groups?: WmsFormGroup[]
  disabled?: boolean
}>(), {
  labelWidth: '120px',
  groups: () => [],
  disabled: false
})

// Emits 定义
const emit = defineEmits<{
  change: [prop: string, value: any]
  validate: [valid: boolean, fields: any]
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 计算验证规则
const computedRules = computed(() => {
  if (props.formRules) {
    return props.formRules
  }
  // 从 formItems 中收集 rules
  const rules: Record<string, FormItemRule | FormItemRule[]> = {}
  props.formItems.forEach(item => {
    if (item.rules) {
      rules[item.prop] = item.rules
    }
  })
  return rules
})

// 获取分组对应的表单项
function getGroupItems(group: WmsFormGroup) {
  return props.formItems.filter(item => group.fields.includes(item.prop))
}

// 获取组件类型
function getComponent(item: WmsFormItem) {
  const componentMap: Record<string, string> = {
    input: 'el-input',
    textarea: 'el-input',
    select: 'el-select',
    date: 'el-date-picker',
    datetime: 'el-date-picker',
    daterange: 'el-date-picker',
    number: 'el-input-number',
    switch: 'el-switch',
    'remote-search': 'el-select',
    cascader: 'el-cascader'
  }
  return componentMap[item.type] || 'el-input'
}

// 获取组件属性
function getComponentProps(item: WmsFormItem) {
  const commonProps = {
    placeholder: item.placeholder || `请输入${item.label}`,
    clearable: item.clearable ?? true,
    disabled: item.disabled
  }

  switch (item.type) {
    case 'input':
      return {
        ...commonProps,
        maxlength: item.maxlength,
        showWordLimit: item.showWordLimit
      }
    case 'textarea':
      return {
        ...commonProps,
        type: 'textarea',
        rows: item.rows || 3,
        maxlength: item.maxlength,
        showWordLimit: item.showWordLimit ?? true
      }
    case 'select':
    case 'remote-search':
      return {
        ...commonProps,
        filterable: item.filterable ?? true,
        remote: item.type === 'remote-search',
        remoteMethod: item.type === 'remote-search' ? onRemoteSearch : undefined
      }
    case 'date':
      return {
        ...commonProps,
        type: 'date',
        format: 'YYYY-MM-DD',
        valueFormat: 'YYYY-MM-DD'
      }
    case 'datetime':
      return {
        ...commonProps,
        type: 'datetime',
        format: 'YYYY-MM-DD HH:mm:ss',
        valueFormat: 'YYYY-MM-DD HH:mm:ss'
      }
    case 'daterange':
      return {
        ...commonProps,
        type: 'daterange',
        format: 'YYYY-MM-DD',
        valueFormat: 'YYYY-MM-DD'
      }
    case 'number':
      return {
        ...commonProps,
        min: item.min,
        max: item.max,
        precision: item.precision
      }
    case 'switch':
      return {
        ...commonProps,
        activeValue: true,
        inactiveValue: false
      }
    default:
      return commonProps
  }
}

// 远程搜索处理
function onRemoteSearch(query: string) {
  // 实际项目中应调用 API
  console.log('Remote search:', query)
}

// 字段值变化处理
function onFieldChange(prop: string, value: any) {
  emit('change', prop, value)
}

// 表单验证方法
async function validate() {
  if (!formRef.value) return false
  try {
    await formRef.value.validate()
    emit('validate', true, props.formData)
    return true
  } catch (fields) {
    emit('validate', false, fields)
    return false
  }
}

// 重置表单
function resetFields() {
  formRef.value?.resetFields()
}

// 清空验证
function clearValidate(props?: string | string[]) {
  formRef.value?.clearValidate(props)
}

// 暴露方法给父组件
defineExpose({
  validate,
  resetFields,
  clearValidate,
  formRef
})
</script>

<style scoped lang="scss">
.wms-form {
  .el-form-item {
    margin-bottom: 22px;
  }

  .el-divider {
    margin: 24px 0 16px;

    &:first-child {
      margin-top: 0;
    }
  }
}
</style>
