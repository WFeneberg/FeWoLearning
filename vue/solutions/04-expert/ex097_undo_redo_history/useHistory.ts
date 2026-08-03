// Exercise 097 — Undo/redo history composable (reference solution).
import { computed, ref, shallowRef, type ComputedRef, type Ref } from "vue";

export interface History<T> {
  current: Readonly<Ref<T>>;
  canUndo: ComputedRef<boolean>;
  canRedo: ComputedRef<boolean>;
  undoCount: ComputedRef<number>;
  set: (value: T) => void;
  undo: () => void;
  redo: () => void;
  clear: () => void;
}

export function useHistory<T>(initial: T, capacity = 50): History<T> {
  if (capacity < 0) {
    throw new RangeError("useHistory: capacity must not be negative");
  }

  // shallowRef so arbitrary T (including objects) is stored as-is rather than
  // being deeply proxied — the history owns snapshots, not reactive graphs.
  const current = shallowRef(initial) as Ref<T>;
  const past = ref<T[]>([]) as Ref<T[]>;
  const future = ref<T[]>([]) as Ref<T[]>;

  const canUndo = computed(() => past.value.length > 0);
  const canRedo = computed(() => future.value.length > 0);
  const undoCount = computed(() => past.value.length);

  const set = (value: T): void => {
    // An idempotent commit must not grow the history, otherwise undo would
    // appear to do nothing for one press.
    if (Object.is(value, current.value)) return;

    if (capacity > 0) {
      past.value.push(current.value);
      // Bound from the front: the most recent `capacity` edits stay undoable.
      if (past.value.length > capacity) past.value.shift();
    }

    current.value = value;
    // Editing after an undo makes the abandoned future unreachable.
    future.value = [];
  };

  const undo = (): void => {
    // Guard on the flag, not on a popped `undefined` — `undefined` is a
    // legitimate T and must not be mistaken for an empty stack.
    if (!canUndo.value) return;
    const previous = past.value.pop() as T;
    future.value.push(current.value);
    current.value = previous;
  };

  const redo = (): void => {
    if (!canRedo.value) return;
    const next = future.value.pop() as T;
    past.value.push(current.value);
    current.value = next;
  };

  const clear = (): void => {
    past.value = [];
    future.value = [];
  };

  return { current, canUndo, canRedo, undoCount, set, undo, redo, clear };
}
