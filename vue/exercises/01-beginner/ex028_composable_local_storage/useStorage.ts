// Exercise 028 — useStorage composable (beginner).
// Goal:   a reactive value that is synced through an injected storage-like object.
// Drills: ref, watch, dependency injection (no direct window.localStorage access).
import { type Ref } from "vue";

export interface StorageLike {
  getItem: (key: string) => string | null;
  setItem: (key: string, value: string) => void;
}

export function useStorage<T>(
  _key: string,
  _defaultValue: T,
  _store: StorageLike,
): Ref<T> {
  throw new Error("TODO: implement useStorage");
}
