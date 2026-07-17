<template>
  <div class="wms-export-button">
    <el-button
      :type="buttonType"
      :disabled="disabled || exporting"
      @click="onExport"
    >
      <el-icon v-if="!exporting"><Download /></el-icon>
      <el-icon v-else class="is-loading"><Loading /></el-icon>
      {{ exporting ? '导出中...' : buttonText }}
    </el-button>

    <!-- 导出进度对话框 -->
    <el-dialog
      v-model="progressVisible"
      title="导出进度"
      width="400px"
      :close-on-click-modal="false"
      :close-on-press-escape="!exporting"
      :show-close="!exporting"
    >
      <div class="export-progress">
        <el-progress
          :percentage="exportProgress"
          :status="exportStatus"
          :stroke-width="16"
          :text-inside="true"
        />

        <div class="progress-info">
          <p>{{ progressText }}</p>
          <p v-if="exportedRows > 0" class="row-count">
            已导出 {{ exportedRows }} 条数据
          </p>
        </div>
      </div>

      <template #footer>
        <el-button
          v-if="exportStatus === 'success'"
          type="primary"
          @click="onDownload"
        >
          下载文件
        </el-button>
        <el-button
          v-if="exportStatus !== 'success'"
          @click="cancelExport"
        >
          取消
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Download, Loading } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

/**
 * COMP-018 WmsExportButton - 导出按钮组件
 *
 * 带进度指示器的导出按钮
 * 支持调用导出 API 并显示进度
 *
 * @props
 * - exportApi: 导出 API 函数
 * - filename: 导出文件名
 * - disabled: 是否禁用
 * - buttonText: 按钮文本
 * - buttonType: 按钮类型
 * - params: 导出参数
 *
 * @emits
 * - start: 开始导出
 * - progress: 导出进度
 * - success: 导出成功
 * - error: 导出失败
 */

// Props 定义
const props = withDefaults(defineProps<{
  exportApi?: (params: any) => Promise<any>
  filename?: string
  disabled?: boolean
  buttonText?: string
  buttonType?: 'primary' | 'success' | 'warning' | 'danger' | 'info' | 'default'
  params?: any
}>(), {
  filename: 'export.xlsx',
  disabled: false,
  buttonText: '导出',
  buttonType: 'primary',
  params: () => ({})
})

// Emits 定义
const emit = defineEmits<{
  start: []
  progress: [percent: number]
  success: [fileUrl: string]
  error: [message: string]
}>()

// 导出状态
const exporting = ref(false)
const progressVisible = ref(false)
const exportProgress = ref(0)
const exportStatus = ref<'success' | 'exception' | undefined>(undefined)
const progressText = ref('')
const exportedRows = ref(0)

// 导出文件URL
let exportFileUrl = ''

// 点击导出
async function onExport() {
  if (!props.exportApi) {
    ElMessage.warning('请配置导出 API')
    return
  }

  exporting.value = true
  progressVisible.value = true
  exportProgress.value = 0
  exportStatus.value = undefined
  progressText.value = '正在准备导出...'
  exportedRows.value = 0

  emit('start')

  try {
    // 模拟进度
    const progressTimer = setInterval(() => {
      if (exportProgress.value < 90) {
        exportProgress.value += 10
        progressText.value = `正在导出数据... ${exportProgress.value}%`
        emit('progress', exportProgress.value)
      }
    }, 300)

    // 调用导出 API
    const result = await props.exportApi(props.params)

    clearInterval(progressTimer)

    // 处理导出结果
    if (result && result.fileUrl) {
      exportFileUrl = result.fileUrl
      exportProgress.value = 100
      exportStatus.value = 'success'
      progressText.value = '导出完成！'
      exportedRows.value = result.rowCount || 0

      emit('success', result.fileUrl)
      ElMessage.success('导出成功')
    } else {
      // 模拟文件URL（实际项目中应从 API 返回）
      exportFileUrl = `/api/wms/export/download/${props.filename}`
      exportProgress.value = 100
      exportStatus.value = 'success'
      progressText.value = '导出完成！'
      exportedRows.value = result?.rowCount || 0

      emit('success', exportFileUrl)
    }
  } catch (error: any) {
    exportStatus.value = 'exception'
    progressText.value = `导出失败: ${error.message || '未知错误'}`
    emit('error', error.message || '导出失败')
    ElMessage.error('导出失败')
  } finally {
    exporting.value = false
  }
}

// 下载文件
function onDownload() {
  if (exportFileUrl) {
    // 创建下载链接
    const link = document.createElement('a')
    link.href = exportFileUrl
    link.download = props.filename
    link.click()
  }
  progressVisible.value = false
}

// 取消导出
function cancelExport() {
  exporting.value = false
  progressVisible.value = false
  exportProgress.value = 0
  ElMessage.info('已取消导出')
}
</script>

<style scoped lang="scss">
.wms-export-button {
  display: inline-block;
}

.export-progress {
  padding: 20px 0;

  .progress-info {
    margin-top: 16px;
    text-align: center;

    p {
      margin: 8px 0;
      color: #606266;
    }

    .row-count {
      color: #909399;
      font-size: 13px;
    }
  }
}
</style>
