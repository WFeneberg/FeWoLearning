// Exercise 097 — Undo/redo history composable (expert).
// Goal:   wrap a value in an undo/redo stack. Committing a new value pushes onto
//         the past and clears the future; undo/redo walk between them; a capacity
//         limit drops the *oldest* history entries rather than the newest.
// Drills: two-stack (past/future) history modelling, invariants that make redo
//         correct, computed guards, bounded buffers, immutable snapshots.
import { type ComputedRef, type Ref } from "vue";

export interface History<T> {
  /** The current value. Read-only for consumers — commit through `set`. */
  current: Readonly<Ref<T>>;
  /** True when there is at least one earlier value to go back to. */
  canUndo: ComputedRef<boolean>;
  /** True when an undo has left something to go forward to. */
  canRedo: ComputedRef<boolean>;
  /** How many entries are currently on the undo stack. */
  undoCount: ComputedRef<number>;
  /**
   * Commits `value` as the new current value. Pushes the previous one onto the
   * undo stack and **clears the redo stack** — a new edit after an undo makes the
   * abandoned future unreachable, which is what every editor does.
   *
   * Committing a value equal to the current one (`Object.is`) is a no-op: it must
   * not grow the history.
   */
  set: (value: T) => void;
  /** Steps back one entry. No-op when `canUndo` is false. */
  undo: () => void;
  /** Steps forward one entry. No-op when `canRedo` is false. */
  redo: () => void;
  /** Drops all history, keeping the current value. */
  clear: () => void;
}

/**
 * Creates a history around `initial`.
 *
 * `capacity` bounds the undo stack. When it would be exceeded the **oldest**
 * entry is discarded, so you can always undo the most recent `capacity` edits.
 * A capacity of 0 means "no history at all": `set` still updates the value but
 * `canUndo` stays false. Negative capacities are rejected with a RangeError.
 */
export function useHistory<T>(_initial: T, _capacity = 50): History<T> {
  throw new Error("TODO: implement useHistory");
}
