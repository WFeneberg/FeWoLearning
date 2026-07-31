// Exercise 050 — useToRefsDestructure composable (intermediate).
// Goal:   a composable returning a reactive object converted with `toRefs`
//         so callers can destructure its properties without losing
//         reactivity (a plain destructure of a `reactive()` object would
//         break the reactive link).
// Drills: reactive, toRefs, mutating a reactive object from the outside
//         while a destructured ref still tracks the change.
import { type Ref } from "vue";

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

export function useToRefsDestructure(_initial: Profile): ProfileRefs {
  throw new Error("TODO: implement useToRefsDestructure");
}
