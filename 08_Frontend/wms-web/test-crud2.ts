import { ref, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import type { FormInstance } from 'element-plus';
import { post, put, del } from '@/api';

export function useCrud<T>(baseUrl: string) {
  async function create(data: Partial<T>): Promise<T> {
    return data as T;
  }
  async function update(id: string, data: Partial<T>): Promise<T> {
    return data as T;
  }
}
