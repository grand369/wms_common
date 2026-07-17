import { ref, reactive } from 'vue';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';

/**
 * Form handling composable — provides validation, submission, and reset.
 * Standard pattern for all WMS form dialogs.
 */
export function useForm<T extends Record<string, any>>(defaultValues: T) {
  const formRef = ref<FormInstance>();
  const formData = reactive<T>({ ...defaultValues });
  const formRules = ref<FormRules>({});
  const submitting = ref(false);
  const visible = ref(false);

  function openForm(initialData?: Partial<T>): void {
    Object.assign(formData, { ...defaultValues, ...initialData });
    visible.value = true;
  }

  function closeForm(): void {
    visible.value = false;
    resetForm();
  }

  function resetForm(): void {
    formRef.value?.resetFields();
    Object.assign(formData, defaultValues);
  }

  async function validate(): Promise<boolean> {
    if (!formRef.value) return false;
    try {
      await formRef.value.validate();
      return true;
    } catch {
      return false;
    }
  }

  async function submitForm(
    submitFn: (data: T) => Promise<any>,
    successMessage: string = 'Operation successful'
  ): Promise<boolean> {
    const isValid = await validate();
    if (!isValid) return false;

    submitting.value = true;
    try {
      await submitFn(formData as T);
      ElMessage.success(successMessage);
      closeForm();
      return true;
    } catch (error) {
      ElMessage.error('Operation failed');
      return false;
    } finally {
      submitting.value = false;
    }
  }

  return {
    formRef,
    formData,
    formRules,
    submitting,
    visible,
    openForm,
    closeForm,
    resetForm,
    validate,
    submitForm,
  };
}
