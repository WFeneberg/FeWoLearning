// Exercise 065 — useDebouncedRef composable (intermediate).
// Goal:   a reactive value that only updates after a delay has elapsed
//         since the last time it was set, coalescing rapid-fire sets.
// Drills: ref, watch/timers, cleanup, custom debounce logic in a composable.
import { type Ref } from "vue";

export interface DebouncedRef<T> {
  /** The debounced value: reflects `set()` calls only after `delay` ms of quiet. */
  value: Ref<T>;
  /** Schedule (or reschedule) an update to `newValue`, resetting the delay timer. */
  set: (newValue: T) => void;
}

export function useDebouncedRef<T>(_initial: T, _delay: number): DebouncedRef<T> {
  throw new Error("TODO: implement useDebouncedRef");
}
