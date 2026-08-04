import { computed, Injectable, signal } from "@angular/core";

// Exercise 030 — immutable array updates in signals (beginner).
// Goal:   change a list held in a signal without mutating the array you were given.
// Drills: update() with spread / map / filter / toSorted, and the reference-equality rule
//         that makes in-place mutation invisible to everything downstream.
// Passes: when `npx jest exercises/01-beginner/ex030_signal_array_update` is green.
//
// A signal decides "did this change?" with a reference comparison (Object.is by default).
// `tasks().push(x)` mutates the array the signal is already holding, so the reference is
// unchanged, so nothing is notified: computeds stay stale and the DOM does not re-render.
// The value is *there* if you look — which is exactly what makes this bug so confusing.
//
// So every change produces a new array: [...old, item] to append, filter() to remove,
// map() to replace one entry, toSorted() rather than sort() (which sorts in place and
// returns the same reference). The old array is left untouched, which also means anything
// that captured it — a previous render, an undo stack — still sees what it saw.
//
// This is cheap: a new array of the same object references, not a deep copy.

export interface Task {
  readonly id: number;
  readonly title: string;
  readonly done: boolean;
}

@Injectable({ providedIn: "root" })
export class TaskStore {
  readonly tasks = signal<readonly Task[]>([]);

  /** Bumped whenever the derived count actually recomputes, so the spec can watch it. */
  recomputes = 0;

  readonly openCount = computed(() => {
    this.recomputes += 1;
    return this.tasks().filter((task) => !task.done).length;
  });

  /** Append a task. A duplicate id is a RangeError. */
  add(task: Task): void {
    throw new Error("TODO: implement add");
  }

  /** Drop the task with this id. An unknown id is a no-op. */
  remove(id: number): void {
    throw new Error("TODO: implement remove");
  }

  /** Flip one task's `done`, leaving every other entry the same object it was. */
  toggle(id: number): void {
    throw new Error("TODO: implement toggle");
  }

  /** Rename one task. A blank title is a ValueError-shaped mistake: throw a RangeError. */
  rename(id: number, title: string): void {
    throw new Error("TODO: implement rename");
  }

  /** Move the task at `from` to index `to`. An out-of-range index is a RangeError. */
  move(from: number, to: number): void {
    throw new Error("TODO: implement move");
  }

  /** A new list sorted by title, ascending. The signal's own order is left alone. */
  sortedByTitle(): readonly Task[] {
    throw new Error("TODO: implement sortedByTitle");
  }

  /**
   * The wrong way, kept deliberately so the spec can show what goes unnoticed.
   *
   * Push straight into the array the signal is holding, without calling set or update.
   */
  addByMutating(task: Task): void {
    throw new Error("TODO: implement addByMutating");
  }
}
