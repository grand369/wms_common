<template>
  <div class="wms-steps">
    <el-steps
      :active="activeStep"
      :finish-status="finishStatus"
      :process-status="processStatus"
      :align-center="alignCenter"
      :direction="direction"
    >
      <el-step
        v-for="(step, index) in steps"
        :key="index"
        :title="getStepTitle(step)"
        :description="getStepDescription(step)"
        :icon="getStepIcon(step)"
        :status="getStepStatus(index)"
      >
        <!-- 自定义图标 -->
        <template v-if="getStepCustomIcon(step)" #icon>
          <component :is="getStepCustomIcon(step)" />
        </template>
      </el-step>
    </el-steps>
  </div>
</template>

<script setup lang="ts">
/**
 * COMP-007 WmsSteps - 流程步骤指示器组件
 *
 * 用于入库/出库/调拨等业务流程的状态展示
 * 基于 Element Plus el-steps 封装
 */

// 步骤配置接口
export interface WmsStepItem {
  title: string
  description?: string
  icon?: string
  customIcon?: any
}

// Props 定义
const props = withDefaults(defineProps<{
  steps: Array<string | WmsStepItem>
  activeStep: number
  status?: 'wait' | 'process' | 'finish' | 'error'
  finishStatus?: 'wait' | 'process' | 'finish' | 'error' | 'success'
  processStatus?: 'wait' | 'process' | 'finish' | 'error'
  alignCenter?: boolean
  direction?: 'horizontal' | 'vertical'
}>(), {
  status: 'process',
  finishStatus: 'success',
  processStatus: 'process',
  alignCenter: true,
  direction: 'horizontal'
})

function isStepItem(step: string | WmsStepItem): step is WmsStepItem {
  return typeof step === 'object' && step !== null && 'title' in step
}

function getStepTitle(step: string | WmsStepItem) {
  return isStepItem(step) ? step.title : step
}

function getStepDescription(step: string | WmsStepItem) {
  return isStepItem(step) ? step.description : undefined
}

function getStepIcon(step: string | WmsStepItem) {
  return isStepItem(step) ? step.icon : undefined
}

function getStepCustomIcon(step: string | WmsStepItem) {
  return isStepItem(step) ? step.customIcon : undefined
}

// 获取步骤状态
function getStepStatus(index: number) {
  if (index < props.activeStep) {
    return 'finish' as const
  } else if (index === props.activeStep) {
    return props.status as any
  } else {
    return 'wait' as const
  }
}
</script>

<style scoped lang="scss">
.wms-steps {
  padding: 24px 16px;

  :deep(.el-step__title) {
    font-size: 14px;
  }

  :deep(.el-step__description) {
    font-size: 12px;
  }
}
</style>
