import { computed, Injectable, signal } from "@angular/core";

// Exercise 034 — a custom equality function (beginner).
// Goal:   stop a signal notifying when the new value is not meaningfully different.
// Drills: signal(value, {equal}), the default Object.is behaviour, equality on a computed,
//         and the silent-staleness bug a too-loose `equal` causes.
// Passes: when `npx jest exercises/01-beginner/ex034_signal_equality_fn` is green.
//
// By default a signal compares with Object.is, which for objects means reference identity.
// Exercises 030 and 031 leaned on that. The other half of the same coin: setting a *new*
// object that happens to hold identical values still counts as a change, so everything
// downstream recomputes and re-renders for nothing. A stream of positions arriving from a
// socket, most of them unchanged, is the classic case.
//
// An `equal` function says what "the same" means. Return true and the signal keeps its
// current value and notifies nobody — note that it keeps the *old* object, so the one you
// passed in is discarded.
//
// The danger is the mirror image of the mutation bug. Too loose an `equal` — comparing only
// an id, say, or only a length — reports "same" for values that really did differ, and the
// update is dropped with no error anywhere. `equal` must consider every field the consumers
// can observe.

export interface Point {
  readonly x: number;
  readonly y: number;
}

@Injectable({ providedIn: "root" })
export class PositionStore {
  /**
   * TODO: a signal of Point starting at (0, 0), with an `equal` comparing x and y.
   *
   * Declared without one so the stub compiles — add the option.
   */
  readonly position = signal<Point>({ x: 0, y: 0 });

  /** The same starting value with default equality, for contrast. */
  readonly naivePosition = signal<Point>({ x: 0, y: 0 });

  /** Bumped whenever `distance` re-runs. */
  recomputes = 0;

  /** Bumped whenever `naiveDistance` re-runs. */
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

  /**
   * TODO: a signal of a tag list whose `equal` compares only `length`.
   *
   * Deliberately wrong — the spec uses it to show an update being silently dropped.
   */
  readonly tags = signal<readonly string[]>([]);

  /** Move to a new position. Always hands the signal a brand-new object. */
  moveTo(x: number, y: number): void {
    throw new Error("TODO: implement moveTo");
  }

  /** The same move against `naivePosition`, for the comparison. */
  moveNaivelyTo(x: number, y: number): void {
    throw new Error("TODO: implement moveNaivelyTo");
  }

  /** Replace the tag list with a brand-new array. */
  setTags(tags: readonly string[]): void {
    throw new Error("TODO: implement setTags");
  }
}
