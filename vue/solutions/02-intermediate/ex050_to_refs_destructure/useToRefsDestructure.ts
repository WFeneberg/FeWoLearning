// Exercise 050 — useToRefsDestructure composable (reference solution).
import { reactive, toRefs, type Ref } from "vue";

export interface Profile {
  name: string;
  age: number;
}

export interface ProfileRefs {
  name: Ref<string>;
  age: Ref<number>;
  state: Profile;
  birthday: () => void;
  rename: (next: string) => void;
}

export function useToRefsDestructure(initial: Profile): ProfileRefs {
  const state = reactive({ ...initial });

  const refs = toRefs(state);

  return {
    ...refs,
    state,
    birthday: () => {
      state.age += 1;
    },
    rename: (next: string) => {
      state.name = next;
    },
  };
}
