// Error code mappings for user-friendly messages
// Maps backend error codes to Chinese user-friendly messages

export interface AppError {
  code?: string
  message?: string
  details?: string
  httpStatus?: number
}

const errorCodeMap: Record<string, string> = {
  // Outbound module errors
  'WMS:Outbound:StatusNotAllowed': '当前状态不允许此操作，请检查单据状态',
  'WMS:Outbound:LineNotFound': '出库明细行不存在',
  'WMS:Outbound:InsufficientInventory': '库存不足，无法完成分配',
  'WMS:Outbound:OverIssueExceeded': '超发数量超出允许比例',
  'WMS:Outbound:InvalidAllocatedQuantity': '分配数量无效',
  'WMS:Outbound:InvalidPickedQuantity': '拣货数量无效',
  'WMS:Outbound:InvalidShippedQuantity': '发货数量无效',
  'WMS:Outbound:MaterialReturnNotImplemented': '退货功能尚未实现',

  // Inventory module errors
  'WMS:Inventory:BalanceNotFound': '库存余额不存在',
  'WMS:Inventory:InsufficientAvailable': '可用库存不足',
  'WMS:Inventory:InvalidReserveQuantity': '预留数量无效',
  'WMS:Inventory:ReleaseExceedsReserved': '释放数量超出预留数量',
  'WMS:Inventory:InvalidFreezeQuantity': '冻结数量无效',
  'WMS:Inventory:InvalidUnfreezeQuantity': '解冻数量无效',
  'WMS:Inventory:FreezeOrderNotFound': '冻结单不存在',
  'WMS:Inventory:FreezeOrderNotPending': '冻结单状态不允许此操作',

  // Inbound module errors
  'WMS:Inbound:StatusNotAllowed': '当前状态不允许此操作',
  'WMS:Inbound:OverReceiptExceeded': '超收数量超出允许比例',

  // Material module errors
  'WMS:Material:MaterialNotFound': '物料不存在',
  'WMS:Material:MaterialCodeExists': '物料编码已存在',

  // Supplier module errors
  'WMS:Supplier:SupplierNotFound': '供应商不存在',
  'WMS:Supplier:SupplierCodeExists': '供应商编码已存在',

  // Warehouse module errors
  'WMS:Warehouse:WarehouseNotFound': '仓库不存在',
  'WMS:Warehouse:LocationNotFound': '库位不存在',

  // General errors
  'WMS:General:InvalidParameter': '参数无效',
  'WMS:General:OperationFailed': '操作失败',
}

export function getFriendlyErrorMessage(error: AppError): string {
  const code = error.code || ''
  const message = error.message || ''

  // Try to find friendly message by exact code match
  if (errorCodeMap[code]) {
    return errorCodeMap[code]
  }

  // Return the backend message if available and not generic
  if (message && !message.includes('服务器内部错误') && !message.includes('internal server error')) {
    return message
  }

  // Generic fallback based on code prefix
  if (code.startsWith('WMS:Outbound:')) {
    return `出库单操作失败（错误代码：${code}）`
  }
  if (code.startsWith('WMS:Inbound:')) {
    return `入库单操作失败（错误代码：${code}）`
  }
  if (code.startsWith('WMS:Inventory:')) {
    return `库存操作失败（错误代码：${code}）`
  }
  if (code.startsWith('WMS:Material:')) {
    return `物料操作失败（错误代码：${code}）`
  }
  if (code.startsWith('WMS:Supplier:')) {
    return `供应商操作失败（错误代码：${code}）`
  }
  if (code.startsWith('WMS:Warehouse:')) {
    return `仓库操作失败（错误代码：${code}）`
  }

  // Default
  return message || `操作失败（错误代码：${code || '未知'}）`
}

export function parseAxiosError(error: any): AppError {
  const responseData = error?.response?.data
  if (responseData?.error) {
    return {
      code: responseData.error.code,
      message: responseData.error.message,
      details: responseData.error.details,
      httpStatus: error.response?.status,
    }
  }
  // Non-ABP error format
  return {
    message: error?.message || '网络错误',
    httpStatus: error?.response?.status,
  }
}