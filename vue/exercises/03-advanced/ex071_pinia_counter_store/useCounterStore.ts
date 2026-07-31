// Exercise 071 — Pinia counter store (advanced).
// Goal:   a tiny, self-contained Pinia-style store: `createPinia`/
//         `setActivePinia` manage which store registry is "active", and
//         `useCounterStore` returns the singleton store for that registry,
//         exposing reactive `count` state and an `increment` action.
// Drills: Pinia's store-per-active-instance model, singleton stores keyed
//         by id, actions mutating shared reactive state, and isolating
//         state between tests via a fresh `setActivePinia(createPinia())`.
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
  throw new Error("TODO: implement useCounterStore");
}
