import dayjs from 'dayjs';

/**
 * Format a date string for display.
 */
export function formatDate(value: string | Date, format: string = 'YYYY-MM-DD HH:mm:ss'): string {
  if (!value) return '--';
  return dayjs(value).format(format);
}

/**
 * Format a number with specified decimal places.
 */
export function formatNumber(value: number | string, decimals: number = 2): string {
  const num = typeof value === 'string' ? parseFloat(value) : value;
  if (isNaN(num)) return '--';
  return num.toFixed(decimals);
}

/**
 * Format a quantity with unit display.
 */
export function formatQuantity(quantity: number, unit: string = ''): string {
  return `${formatNumber(quantity, 4)}${unit ? ' ' + unit : ''}`;
}

/**
 * Format file size in human-readable format.
 */
export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 B';
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

/**
 * Generate a display label from an enum value.
 */
export function enumLabel(value: number, labels: Record<number, string>): string {
  return labels[value] ?? `Unknown(${value})`;
}
