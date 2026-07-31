// Exercise 092 — useQuery composable (expert).
// Goal:   a `useQuery(key, fetcher)` composable that caches resolved results
//         by key in a module-level cache, so a repeat call with the same key
//         returns the cached data synchronously (no loading flicker, no
//         re-invoking the fetcher), while a fresh key still fetches.
// Drills: module-scoped cache shared across composable instances, dedupe of
//         in-flight promises, reactive loading/error state, manual refetch
//         that bypasses the cache.
import { type Ref } from "vue";

export interface QueryResult<T> {
  data: Ref<T | undefined>;
  error: Ref<unknown>;
  isLoading: Ref<boolean>;
  refetch: () => Promise<void>;
}

export function useQuery<T>(
  _key: string,
  _fetcher: () => Promise<T>,
): QueryResult<T> {
  throw new Error("TODO: implement useQuery");
}
