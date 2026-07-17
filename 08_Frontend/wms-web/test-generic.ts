export function useCrud<T>(baseUrl: string) {
  async function create(data: Partial<T>): Promise<T> {
    return {} as T;
  }
}
