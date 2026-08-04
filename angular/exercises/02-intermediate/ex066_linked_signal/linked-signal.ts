import { Signal, WritableSignal } from "@angular/core";

// Exercise 066 — resettable derived state (intermediate).
// Goal:   build a signal that is writable *and* resets itself when its source changes.
// Drills: the gap between signal() and computed(), composing the two with a version marker, and
//         the semantics Angular 19 ships as linkedSignal().
// Passes: when `npx jest exercises/02-intermediate/ex066_linked_signal` is green.
//
// Note on versions: this track runs Angular 18.2, and `linkedSignal()` arrives in 19. The point of
// this exercise is the *pattern*, which is worth understanding before reaching for the API — and
// once you upgrade, `linkedSignal({source, computation})` replaces the whole file.
//
// The problem it solves comes up constantly. A select shows the options for the current category
// and the user picks one. The selection is writable — the user sets it — and derived, because
// changing category must throw the old selection away. `signal()` alone forgets to reset;
// `computed()` alone cannot be written to. Neither is enough on its own.
//
// The shape of the answer: keep the user's choice in a plain signal alongside a marker for *which
// state of the source* it was made against, and read through a computed that compares them. If the
// source has moved on, the stored choice is stale and the derived default wins.
//
// Choosing that marker is the interesting part. Storing the source *value* looks right and is
// subtly wrong: going tools -> toys -> tools would match again and resurrect a choice the user had
// already lost. What you want is something whose identity changes once per source change, and a
// computed returning a fresh object gives you exactly that — it recomputes only when the source
// changes, so its object identity is a reliable change marker.
//
// The subtlety worth noticing: this makes staleness a *read-time* decision rather than something an
// effect has to eagerly fix up. No effect, no ordering to reason about, and nothing that can be
// observed halfway through an update.

export interface Linked<S, T> {
  /** The current value: the explicit choice when fresh, otherwise the computed default. */
  readonly value: Signal<T>;

  /** Override the value for the source's current state. */
  set(value: T): void;

  /** Discard any override and go back to the computed default. */
  reset(): void;

  /** Whether an override is currently in force. */
  readonly overridden: Signal<boolean>;
}

/**
 * TODO: build the resettable derived signal described above.
 *
 * `source` is what the value derives from; `compute` turns a source value into the default.
 *
 * - reading `value` gives the override when it was set against the *current* source value, and the
 *   computed default otherwise
 * - `set` records both the value and a marker for the source state it was chosen against
 * - `reset` drops the override
 * - `overridden` reports whether an override is currently in force
 *
 * Use a plain signal for the override plus computeds for the reads. No effect.
 */
export function linked<S, T>(source: Signal<S>, compute: (value: S) => T): Linked<S, T> {
  throw new Error("TODO: implement linked");
}

/**
 * TODO: the naive version, for contrast — a plain writable signal seeded from the source.
 *
 * Writable, and it never resets, which is the bug this exercise exists to make visible.
 */
export function naiveLinked<S, T>(source: Signal<S>, compute: (value: S) => T): WritableSignal<T> {
  throw new Error("TODO: implement naiveLinked");
}
