// Exercise 027 — useToggle composable (beginner).
// Goal:   a reactive boolean that flips on each call to toggle.
// Drills: ref, returning a tuple API from a composable.
import { type Ref } from "vue";

export type ToggleApi = [state: Ref<boolean>, toggle: () => void];

export function useToggle(_initial = false): ToggleApi {
  throw new Error("TODO: implement useToggle");
}
