<template>
  <div class="wms-order-line-editor">
    <!-- 表头 -->
    <div v-if="!readonly" class="line-header">
      <el-button type="primary" :icon="Plus" @click="addLine">
        添加行
      </el-button>
    </div>

    <!-- 订单行列表 -->
    <div class="line-list">
      <div
        v-for="(line, index) in lineData"
        :key="line._id || index"
        class="line-item"
        :class="{ 'line-error': lineErrors[index] }"
      >
        <div class="line-content">
          <!-- 序号 -->
          <div class="line-index">{{ index + 1 }}</div>

          <!-- 物料选择 -->
          <div class="line-field">
            <wms-material-selector
              v-model="line.materialId"
              :disabled="readonly"
              :material-info="line.materialId ? { id: line.materialId, code: line.materialCode || '', name: line.materialName || '' } : undefined"
              @change="onMaterialChange(index, $event)"
            />
          </div>

          <!-- 物料名称 -->
          <div class="line-field line-material-name">
            <el-input
              :model-value="line.materialName"
              disabled
              placeholder="物料名称"
            />
          </div>

          <!-- 数量 -->
          <div class="line-field">
            <el-input-number
              v-model="line.quantity"
              :disabled="readonly"
              :min="0"
              :precision="2"
              :step="1"
              placeholder="数量"
              @change="validateLine(index)"
            />
          </div>

          <!-- 单位 -->
          <div class="line-field">
            <el-select
              v-model="line.unit"
              :disabled="readonly"
              placeholder="单位"
              clearable
            >
              <el-option
                v-for="item in unitOptions"
                :key="item.itemCode"
                :label="item.itemName"
                :value="item.itemCode"
              />
            </el-select>
          </div>

          <!-- 库位选择（仅入库模式，出库模式由分配策略自动分配） -->
          <div v-if="mode === 'inbound'" class="line-field">
            <wms-location-selector
              v-model="line.locationId"
              :warehouse-id="line.warehouseId"
              :area-id="line.areaId"
              :disabled="readonly"
              @change="(location: any) => onLocationChange(index, location)"
              @warehouse-change="(warehouse: any) => onWarehouseChange(index, warehouse)"
              @area-change="(area: any) => onAreaChange(index, area)"
            />
          </div>

          <!-- 源库位/目标库位（调拨模式） -->
          <template v-if="mode === 'transfer'">
            <div class="line-field">
              <wms-location-selector
                v-model="line.fromLocationId"
                :disabled="readonly"
                placeholder="源库位"
                @change="() => validateLine(index)"
              />
            </div>
            <div class="line-field">
              <wms-location-selector
                v-model="line.toLocationId"
                :disabled="readonly"
                placeholder="目标库位"
                @change="() => validateLine(index)"
              />
            </div>
          </template>

          <!-- 备注 -->
          <div class="line-field line-remarks">
            <el-input
              v-model="line.remarks"
              :disabled="readonly"
              placeholder="备注"
              maxlength="200"
              show-word-limit
            />
          </div>

          <!-- 操作按钮 -->
          <div v-if="!readonly" class="line-actions">
            <el-button
              type="danger"
              :icon="Minus"
              circle
              size="small"
              @click="removeLine(index)"
            />
          </div>
        </div>

        <!-- 行级验证错误 -->
        <div v-if="lineErrors[index]" class="line-error-msg">
          {{ lineErrors[index] }}
        </div>
      </div>
    </div>

    <!-- 空状态 -->
    <el-empty
      v-if="!lineData || lineData.length === 0"
      description="暂无订单行，请点击添加"
    >
      <el-button v-if="!readonly" type="primary" @click="addLine">
        添加第一行
      </el-button>
    </el-empty>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { Plus, Minus } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'
import WmsMaterialSelector from './WmsMaterialSelector.vue'
import WmsLocationSelector from './WmsLocationSelector.vue'
import { getDictionaryItemsByCode } from '@/api/dataDictionary'
import type { DictionaryItemDto } from '@/api/dataDictionary'

/**
 * COMP-011 WmsOrderLineEditor - 订单行编辑器组件
 *
 * 用于入库/出库/调拨单据的订单行编辑
 * 支持动态添加/删除行，内联验证
 *
 * @props
 * - lines: 订单行数据数组
 * - mode: 模式，'inbound' | 'outbound' | 'transfer'
 * - readonly: 是否只读
 *
 * @emits
 * - update:lines: 订单行数据变化
 * - validate: 验证结果
 */

// 订单行数据接口
export interface WmsOrderLine {
  _id?: string  // 前端临时ID
  materialId: string
  materialCode?: string
  materialName?: string
  quantity: number
  unit?: string
  warehouseId?: string
  warehouseCode?: string
  areaId?: string
  areaCode?: string
  locationId?: string
  locationCode?: string
  fromLocationId?: string
  toLocationId?: string
  batchNumber?: string
  expiryDate?: string
  productionDate?: string
  remarks?: string
}

// Props 定义
const props = withDefaults(defineProps<{
  lines: WmsOrderLine[]
  mode?: 'inbound' | 'outbound' | 'transfer'
  readonly?: boolean
}>(), {
  mode: 'inbound',
  readonly: false
})

// Emits 定义
const emit = defineEmits<{
  'update:lines': [lines: WmsOrderLine[]]
  validate: [valid: boolean]
}>()

// 行数据（本地副本）
const lineData = ref<WmsOrderLine[]>([])
// 行级错误
const lineErrors = ref<Record<number, string>>({})
// 单位选项
const unitOptions = ref<DictionaryItemDto[]>([])

// 初始化
watch(() => props.lines, (newVal) => {
  lineData.value = newVal.map(line => ({
    ...line,
    _id: line._id || generateId()
  }))
}, { immediate: true, deep: true })

// 加载单位选项
async function loadUnitOptions() {
  try {
    unitOptions.value = await getDictionaryItemsByCode('SysUnit');
  } catch {
    ElMessage.error('加载单位选项失败');
  }
}

// 生成临时ID
function generateId() {
  return `line_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`
}

// 添加行
function addLine() {
  const newLine: WmsOrderLine = {
    _id: generateId(),
    materialId: '',
    quantity: 0,
    unit: unitOptions.value.length > 0 ? unitOptions.value[0].itemCode : ''
  }
  lineData.value.push(newLine)
  emitUpdate()
}

// 组件挂载时加载单位选项
onMounted(() => {
  loadUnitOptions()
})

// 删除行
function removeLine(index: number) {
  lineData.value.splice(index, 1)
  emitUpdate()
}

// 物料变化
function onMaterialChange(index: number, material: any) {
  if (material) {
    lineData.value[index].materialCode = material.code
    lineData.value[index].materialName = material.name
    lineData.value[index].unit = material.unit || ''
  }
  emitUpdate()
}

// 仓库变化
function onWarehouseChange(index: number, warehouse: any) {
  if (warehouse) {
    lineData.value[index].warehouseId = warehouse.id
    lineData.value[index].warehouseCode = warehouse.code
  } else {
    lineData.value[index].warehouseId = ''
    lineData.value[index].warehouseCode = ''
  }
  lineData.value[index].areaId = ''
  lineData.value[index].areaCode = ''
  lineData.value[index].locationId = ''
  lineData.value[index].locationCode = ''
  emitUpdate()
}

// 库区变化
function onAreaChange(index: number, area: any) {
  if (area) {
    lineData.value[index].areaId = area.id
    lineData.value[index].areaCode = area.code
  } else {
    lineData.value[index].areaId = ''
    lineData.value[index].areaCode = ''
  }
  lineData.value[index].locationId = ''
  lineData.value[index].locationCode = ''
  emitUpdate()
}

// 库位变化
function onLocationChange(index: number, location: any) {
  if (location) {
    lineData.value[index].locationId = location.id
    lineData.value[index].locationCode = location.code
  } else {
    lineData.value[index].locationId = ''
    lineData.value[index].locationCode = ''
  }
  validateLine(index)
  emitUpdate()
}

// 验证单行
function validateLine(index: number) {
  const line = lineData.value[index]
  const errors: string[] = []

  if (!line.materialId) {
    errors.push('请选择物料')
  }
  if (!line.quantity || line.quantity <= 0) {
    errors.push('数量必须大于0')
  }

  if (props.mode === 'inbound') {
    if (!line.locationId) {
      errors.push('请选择库位')
    }
  } else if (props.mode === 'transfer') {
    if (!line.fromLocationId) {
      errors.push('请选择源库位')
    }
    if (!line.toLocationId) {
      errors.push('请选择目标库位')
    }
  }
  // outbound mode: location is auto-allocated, no validation needed

  if (errors.length > 0) {
    lineErrors.value[index] = errors.join('; ')
    return false
  } else {
    delete lineErrors.value[index]
    emitUpdate()
    return true
  }
}

// 验证所有行
function validate() {
  let valid = true
  lineErrors.value = {}

  lineData.value.forEach((_, index) => {
    if (!validateLine(index)) {
      valid = false
    }
  })

  emit('validate', valid)
  return valid
}

// 触发更新
function emitUpdate() {
  emit('update:lines', [...lineData.value])
}

// 暴露验证方法
defineExpose({ validate })
</script>

<style scoped lang="scss">
.wms-order-line-editor {
  .line-header {
    margin-bottom: 16px;
  }

  .line-list {
    display: flex;
    flex-direction: column;
    gap: 12px;
  }

  .line-item {
    padding: 16px;
    border: 1px solid #DCDFE6;
    border-radius: 4px;
    background: #FAFAFA;

    &.line-error {
      border-color: #F56C6C;
    }
  }

  .line-content {
    display: flex;
    align-items: flex-start;
    gap: 12px;
    flex-wrap: wrap;
  }

  .line-index {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background: $wms-color-primary;
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: 600;
    flex-shrink: 0;
  }

  .line-field {
    min-width: 110px;

    &.line-material-name {
      min-width: 180px;
    }

    &.line-remarks {
      min-width: 160px;
    }
  }

  .line-actions {
    display: flex;
    align-items: center;
    padding-top: 4px;
  }

  .line-error-msg {
    margin-top: 8px;
    color: #F56C6C;
    font-size: 12px;
    padding-left: 44px;
  }
}
</style>
