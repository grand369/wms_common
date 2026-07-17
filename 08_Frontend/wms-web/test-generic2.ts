export function useCrud<T>(baseUrl: string) {
  async function create(data: Partial<T>): Promise<T> {
    return data as T;
  }
  async function update(id: string, data: Partial<T>): Promise<T> {
    return data as T;
  }
}
