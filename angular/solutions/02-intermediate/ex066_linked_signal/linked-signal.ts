import { Signal, WritableSignal, computed, signal } from "@angular/core";

// Exercise 066 — resettable derived state (reference solution).
//
// Angular 19's linkedSignal({source, computation}) is this, built in.

export interface Linked<S, T> {
  readonly value: Signal<T>;
  set(value: T): void;
  reset(): void;
  readonly overridden: Signal<boolean>;
}

/** The override, together with the source *epoch* it was chosen during. */
interface Override<T> {
  readonly forEpoch: object;
  readonly value: T;
}

export function linked<S, T>(source: Signal<S>, compute: (value: S) => T): Linked<S, T> {
  // The change marker. This computed recomputes only when `source` changes, and it produces a
  // brand-new object each time it does — so its *identity* changes exactly once per source change.
  //
  // Comparing source values instead would look right and be subtly wrong: going tools -> toys ->
  // tools would match the stored value again and resurrect an override the user had already lost.
  const epoch = computed<{ readonly of: S }>(() => ({ of: source() }));

  const override = signal<Override<T> | null>(null);

  // Staleness is decided at *read* time, so there is no effect, no ordering to reason about, and
  // no half-updated state anyone can observe.
  const isFresh = computed(() => {
    const current = override();
    return current !== null && Object.is(current.forEpoch, epoch());
  });

  const value = computed(() => {
    const current = override();
    return isFresh() && current !== null ? current.value : compute(epoch().of);
  });

  return {
    value,
    overridden: isFresh,
    set: (next: T) => {
      // Both halves, or there would be nothing to compare against later.
      override.set({ forEpoch: epoch(), value: next });
    },
    reset: () => override.set(null),
  };
}

export function naiveLinked<S, T>(source: Signal<S>, compute: (value: S) => T): WritableSignal<T> {
  // Seeded once, at construction. Writable, and completely disconnected from the source — it
  // neither follows it nor resets, which is exactly the bug.
  return signal(compute(source()));
}
