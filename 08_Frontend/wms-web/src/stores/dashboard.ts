import { defineStore } from 'pinia';
import { ref } from 'vue';

interface CacheEntry<T> {
  data: T;
  timestamp: number;
}

const TTL_MS = 5 * 60 * 1000; // 5 minutes

export const useDashboardStore = defineStore('dashboard', () => {
  const cache = ref<Map<string, CacheEntry<any>>>(new Map());

  function getCached<T>(key: string): T | null {
    const entry = cache.value.get(key);
    if (!entry) return null;
    if (Date.now() - entry.timestamp > TTL_MS) {
      cache.value.delete(key);
      return null;
    }
    return entry.data as T;
  }

  function setCache<T>(key: string, data: T): void {
    cache.value.set(key, { data, timestamp: Date.now() });
  }

  function invalidate(prefix?: string): void {
    if (prefix) {
      for (const key of cache.value.keys()) {
        if (key.startsWith(prefix)) cache.value.delete(key);
      }
    } else {
      cache.value.clear();
    }
  }

  return { getCached, setCache, invalidate };
});
