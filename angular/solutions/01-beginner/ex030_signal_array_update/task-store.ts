import { computed, Injectable, signal } from "@angular/core";

// Exercise 030 — immutable array updates in signals (reference solution).

export interface Task {
  readonly id: number;
  readonly title: string;
  readonly done: boolean;
}

@Injectable({ providedIn: "root" })
export class TaskStore {
  readonly tasks = signal<readonly Task[]>([]);

  recomputes = 0;

  readonly openCount = computed(() => {
    this.recomputes += 1;
    return this.tasks().filter((task) => !task.done).length;
  });

  add(task: Task): void {
    if (this.tasks().some((existing) => existing.id === task.id)) {
      throw new RangeError(`task ${task.id} already exists`);
    }
    this.tasks.update((tasks) => [...tasks, task]);
  }

  remove(id: number): void {
    // filter() over a missing id already changes nothing, so there is no special case.
    this.tasks.update((tasks) => tasks.filter((task) => task.id !== id));
  }

  toggle(id: number): void {
    this.tasks.update((tasks) =>
      // map() returns the *same object* for every entry that is not the target, so only
      // the one that changed is a new reference.
      tasks.map((task) => (task.id === id ? { ...task, done: !task.done } : task)),
    );
  }

  rename(id: number, title: string): void {
    const trimmed = title.trim();
    if (trimmed === "") {
      throw new RangeError("title must not be blank");
    }
    this.tasks.update((tasks) =>
      tasks.map((task) => (task.id === id ? { ...task, title: trimmed } : task)),
    );
  }

  move(from: number, to: number): void {
    const length = this.tasks().length;
    const inRange = (index: number): boolean => index >= 0 && index < length;
    if (!inRange(from) || !inRange(to)) {
      throw new RangeError(`cannot move ${from} to ${to} in a list of ${length}`);
    }
    this.tasks.update((tasks) => {
      const next = [...tasks];
      const [moved] = next.splice(from, 1);
      next.splice(to, 0, moved);
      // Splicing a *copy* is fine — what matters is that the signal never sees the
      // original array mutated.
      return next;
    });
  }

  sortedByTitle(): readonly Task[] {
    // A copy before sorting: sort() reorders in place and would silently rearrange the
    // signal's own array while returning the same reference.
    return [...this.tasks()].sort((a, b) => a.title.localeCompare(b.title));
  }

  addByMutating(task: Task): void {
    // Deliberately wrong. The cast is needed precisely because the type says readonly —
    // TypeScript was trying to stop this.
    (this.tasks() as Task[]).push(task);
  }
}
