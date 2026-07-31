// Exercise 066 — useForm composable (reference solution).
import { reactive, toRefs, type Ref } from "vue";

export interface FormFields {
  name: string;
  email: string;
}

export interface Form<T> {
  state: T;
  fields: { [K in keyof T]: Ref<T[K]> };
}

export function useForm<T extends object>(initial: T): Form<T> {
  const state = reactive(initial) as T;
  const fields = toRefs(state) as { [K in keyof T]: Ref<T[K]> };
  return { state, fields };
}
