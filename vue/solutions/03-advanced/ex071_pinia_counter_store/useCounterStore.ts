// Exercise 071 — Pinia counter store (reference solution).
import { reactive } from "vue";

/** A minimal stand-in for a Pinia instance: a registry of stores by id. */
export interface Pinia {
  readonly stores: Map<string, unknown>;
}

let activePinia: Pinia | undefined;

/** Creates a new, empty store registry (like Pinia's `createPinia()`). */
export function createPinia(): Pinia {
  return { stores: new Map() };
}

/** Marks `pinia` as the registry that `useCounterStore()` reads from. */
export function setActivePinia(pinia: Pinia): void {
  activePinia = pinia;
}

export interface CounterStore {
  readonly count: number;
  increment: () => void;
}

/**
 * Returns the counter store from the currently active pinia, creating it on
 * first access. Repeated calls against the same active pinia return the
 * same (reactive) store instance; a different active pinia gets its own.
 */
export function useCounterStore(): CounterStore {
  if (!activePinia) {
    throw new Error(
      "[pinia] no active pinia - call setActivePinia(createPinia()) first",
    );
  }

  const existing = activePinia.stores.get("counter") as
    | CounterStore
    | undefined;
  if (existing) {
    return existing;
  }

  const state = reactive({ count: 0 });
  const store: CounterStore = {
    get count() {
      return state.count;
    },
    increment() {
      state.count += 1;
    },
  };

  activePinia.stores.set("counter", store);
  return store;
}
