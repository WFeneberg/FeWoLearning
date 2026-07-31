// Exercise 049 — useFetch composable (intermediate).
// Goal:   extract inline component fetch-logic into a reusable composable.
// Drills: composable extraction, async state modelling (data/error/loading refs).
import { ref, type Ref } from "vue";

export interface FetchState<T> {
  data: Ref<T | null>;
  error: Ref<unknown>;
  loading: Ref<boolean>;
}

export function useFetch<T>(_fetcher: () => Promise<T>): FetchState<T> {
  throw new Error("TODO: implement useFetch");
}
