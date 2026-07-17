import { ref } from 'vue';
import { ElMessage } from 'element-plus';
import type { FormInstance } from 'element-plus';
import { post, put, del } from '@/api';

/**
 * CRUD operations composable for standard entity management.
 * Provides create, update, delete, and refresh operations.
 */
export function useCrud<T>(baseUrl: string) {
  const loading = ref(false);
  const currentId = ref<string>('');
  const formRef = ref<FormInstance>();

  async function create(data: Partial<T>): Promise<T> {
    loading.value = true;
    try {
      const result = await post<T>(baseUrl, data);
      ElMessage.success('Created successfully');
      return result;
    } catch (error) {
      ElMessage.error('Create failed');
      throw error;
    } finally {
      loading.value = false;
    }
  }

  async function update(id: string, data: Partial<T>): Promise<T> {
    loading.value = true;
    try {
      const result = await put<T>(`${baseUrl}/${id}`, data);
      ElMessage.success('Updated successfully');
      return result;
    } catch (error) {
      ElMessage.error('Update failed');
      throw error;
    } finally {
      loading.value = false;
    }
  }

  async function remove(id: string): Promise<void> {
    loading.value = true;
    try {
      await del(`${baseUrl}/${id}`);
      ElMessage.success('Deleted successfully');
    } catch (error) {
      ElMessage.error('Delete failed');
      throw error;
    } finally {
      loading.value = false;
    }
  }

  function resetForm(): void {
    formRef.value?.resetFields();
    currentId.value = '';
  }

  return {
    loading,
    currentId,
    formRef,
    create,
    update,
    remove,
    resetForm,
  };
}
