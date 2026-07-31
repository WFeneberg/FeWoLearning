// Exercise 027 — useToggle composable (reference solution).
import { ref, type Ref } from "vue";

export type ToggleApi = [state: Ref<boolean>, toggle: () => void];

export function useToggle(initial = false): ToggleApi {
  const state = ref(initial);
  const toggle = () => {
    state.value = !state.value;
  };
  return [state, toggle];
}
