import { ref, reactive, computed } from 'vue';
import { get } from '@/api';

/**
 * Table data composable — provides pagination, sorting, filtering.
 * Standard pattern for all WMS list pages.
 */
export function useTable<T>(baseUrl: string) {
  const loading = ref(false);
  const tableData = ref<T[]>([]);
  const total = ref(0);

  const pagination = reactive({
    currentPage: 1,
    pageSize: 20,
    pageSizes: [10, 20, 50, 100],
  });

  const sortConfig = reactive({
    prop: '',
    order: '' as '' | 'ascending' | 'descending',
  });

  const filters = reactive<Record<string, any>>({});

  const queryParams = computed(() => ({
    skipCount: (pagination.currentPage - 1) * pagination.pageSize,
    maxResultCount: pagination.pageSize,
    sorting: sortConfig.prop && sortConfig.order
      ? `${sortConfig.prop} ${sortConfig.order === 'ascending' ? '' : 'DESC'}`
      : '',
    ...filters,
  }));

  async function fetchData(): Promise<void> {
    loading.value = true;
    try {
      const result = await get<{ items: T[]; totalCount: number }>(baseUrl, {
        params: queryParams.value,
      });
      tableData.value = result.items;
      total.value = result.totalCount;
    } catch (error) {
      console.error('Failed to fetch table data:', error);
    } finally {
      loading.value = false;
    }
  }

  function handlePageChange(page: number): void {
    pagination.currentPage = page;
    fetchData();
  }

  function handleSizeChange(size: number): void {
    pagination.pageSize = size;
    pagination.currentPage = 1;
    fetchData();
  }

  function handleSortChange({ prop, order }: { prop: string; order: string }): void {
    sortConfig.prop = prop;
    sortConfig.order = order as '' | 'ascending' | 'descending';
    fetchData();
  }

  function handleSearch(): void {
    pagination.currentPage = 1;
    fetchData();
  }

  function resetFilters(): void {
    Object.keys(filters).forEach((key) => {
      filters[key] = undefined;
    });
    handleSearch();
  }

  return {
    loading,
    tableData,
    total,
    pagination,
    sortConfig,
    filters,
    queryParams,
    fetchData,
    handlePageChange,
    handleSizeChange,
    handleSortChange,
    handleSearch,
    resetFilters,
  };
}
