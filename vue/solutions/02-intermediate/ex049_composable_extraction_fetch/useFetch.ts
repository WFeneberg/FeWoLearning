// Exercise 049 — useFetch composable (reference solution).
import { ref, type Ref } from "vue";

export interface FetchState<T> {
  data: Ref<T | null>;
  error: Ref<unknown>;
  loading: Ref<boolean>;
}

export function useFetch<T>(fetcher: () => Promise<T>): FetchState<T> {
  const data = ref(null) as Ref<T | null>;
  const error = ref<unknown>(null);
  const loading = ref(true);

  fetcher()
    .then((result) => {
      data.value = result;
      error.value = null;
    })
    .catch((err) => {
      error.value = err;
      data.value = null;
    })
    .finally(() => {
      loading.value = false;
    });

  return { data, error, loading };
}
