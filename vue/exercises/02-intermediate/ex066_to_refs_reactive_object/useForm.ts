// Exercise 066 — useForm composable (intermediate).
// Goal:   toRefs on a reactive form object so fields stay reactive when destructured.
// Drills: reactive, toRefs, keeping a two-way link between a reactive object and its refs.
import { type Ref } from "vue";

export interface FormFields {
  name: string;
  email: string;
}

export interface Form<T> {
  state: T;
  fields: { [K in keyof T]: Ref<T[K]> };
}

export function useForm<T extends object>(_initial: T): Form<T> {
  throw new Error("TODO: implement useForm");
}
