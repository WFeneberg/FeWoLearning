// Exercise 092 — useQuery composable (reference solution).
import { ref, type Ref } from "vue";

export interface QueryResult<T> {
  data: Ref<T | undefined>;
  error: Ref<unknown>;
  isLoading: Ref<boolean>;
  refetch: () => Promise<void>;
}

interface CacheEntry<T> {
  status: "pending" | "success" | "error";
  data?: T;
  error?: unknown;
  promise: Promise<void>;
}

// Module-scoped cache: shared by every composable instance in the app, so two
// components calling useQuery with the same key reuse the same in-flight (or
// resolved) request instead of triggering the fetcher twice.
const cache = new Map<string, CacheEntry<unknown>>();

function start<T>(key: string, fetcher: () => Promise<T>): CacheEntry<T> {
  const entry: CacheEntry<T> = {
    status: "pending",
    promise: Promise.resolve(),
  };
  entry.promise = fetcher().then(
    (result) => {
      entry.status = "success";
      entry.data = result;
    },
    (err) => {
      entry.status = "error";
      entry.error = err;
    },
  );
  cache.set(key, entry as CacheEntry<unknown>);
  return entry;
}

export function useQuery<T>(
  key: string,
  fetcher: () => Promise<T>,
): QueryResult<T> {
  const data = ref<T | undefined>(undefined) as Ref<T | undefined>;
  const error = ref<unknown>(undefined);
  const isLoading = ref(false);

  function adopt(entry: CacheEntry<T>) {
    if (entry.status === "success") {
      data.value = entry.data;
      isLoading.value = false;
      return;
    }
    if (entry.status === "error") {
      error.value = entry.error;
      isLoading.value = false;
      return;
    }
    isLoading.value = true;
    entry.promise.then(() => {
      data.value = entry.data;
      error.value = entry.error;
      isLoading.value = false;
    });
  }

  const existing = cache.get(key) as CacheEntry<T> | undefined;
  adopt(existing ?? start(key, fetcher));

  function refetch(): Promise<void> {
    cache.delete(key);
    const entry = start(key, fetcher);
    isLoading.value = true;
    error.value = undefined;
    return entry.promise.then(() => {
      data.value = entry.data;
      error.value = entry.error;
      isLoading.value = false;
    });
  }

  return { data, error, isLoading, refetch };
}
