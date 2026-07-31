// Exercise 072 — Pinia getters (advanced).
// Goal:   model a Pinia "setup store" whose `doubleCount` getter is derived
//         from `count` state and stays reactive when `count` changes via the
//         store's own action, and whose `quadrupleCount` getter is in turn
//         derived from `doubleCount` (a getter built on top of another
//         getter, exactly like `doubleCount(): number { return this.doubleCount * 2 }`
//         would look in Pinia's Options-API stores).
// Drills: Pinia getters as `computed()` over store state, chaining getters,
//         keeping getters read-only and recomputed only from their action.
//
// Note: this exercise reproduces the exact shape of a Pinia setup store
// (`state` as `ref`, `getters` as `computed`, `actions` as plain functions)
// using Vue's own reactivity primitives, since `defineStore` itself is a
// thin wrapper around them. The public API below is what `useCounterStore()`
// would return whether it were written with `defineStore(...)` or, as here,
// as a plain composable — the reactive behavior under test is identical.
import { type ComputedRef, type Ref } from "vue";

export interface CounterStore {
  count: Ref<number>;
  doubleCount: ComputedRef<number>;
  quadrupleCount: ComputedRef<number>;
  increment: (amount?: number) => void;
}

export function useCounterStore(_initial = 0): CounterStore {
  throw new Error("TODO: implement useCounterStore");
}
