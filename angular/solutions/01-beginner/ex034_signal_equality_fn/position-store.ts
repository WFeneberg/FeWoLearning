import { computed, Injectable, signal } from "@angular/core";

// Exercise 034 — a custom equality function (reference solution).

export interface Point {
  readonly x: number;
  readonly y: number;
}

@Injectable({ providedIn: "root" })
export class PositionStore {
  // Compares every field a consumer can observe, so "same" really does mean same.
  readonly position = signal<Point>(
    { x: 0, y: 0 },
    { equal: (a, b) => a.x === b.x && a.y === b.y },
  );

  // Default Object.is: a new object is always a change, however identical its contents.
  readonly naivePosition = signal<Point>({ x: 0, y: 0 });

  recomputes = 0;

  naiveRecomputes = 0;

  readonly distance = computed(() => {
    this.recomputes += 1;
    const { x, y } = this.position();
    return Math.round(Math.hypot(x, y) * 100) / 100;
  });

  readonly naiveDistance = computed(() => {
    this.naiveRecomputes += 1;
    const { x, y } = this.naivePosition();
    return Math.round(Math.hypot(x, y) * 100) / 100;
  });

  // Deliberately wrong: length is not the only thing consumers can see, so a same-length
  // replacement is reported as "no change" and silently discarded.
  readonly tags = signal<readonly string[]>([], {
    equal: (a, b) => a.length === b.length,
  });

  moveTo(x: number, y: number): void {
    // Always a fresh object. Whether that counts as a change is `equal`'s decision, not the
    // caller's — which is exactly the separation of concerns worth having.
    this.position.set({ x, y });
  }

  moveNaivelyTo(x: number, y: number): void {
    this.naivePosition.set({ x, y });
  }

  setTags(tags: readonly string[]): void {
    this.tags.set([...tags]);
  }
}
