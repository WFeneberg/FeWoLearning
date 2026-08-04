import { Signal, WritableSignal, signal } from "@angular/core";
import { toObservable, toSignal } from "@angular/core/rxjs-interop";
import { Observable, tap } from "rxjs";

// Exercise 054 — toSignal and toObservable (reference solution).

export function toSignalWithInitial<T>(source: Observable<T>, initial: T): Signal<T> {
  // With an initialValue the result is Signal<T> — no undefined for consumers to handle.
  return toSignal(source, { initialValue: initial });
}

export function toSignalMaybe<T>(source: Observable<T>): Signal<T | undefined> {
  // Honest about the gap before the first emission, and awkward downstream for that reason.
  return toSignal(source);
}

export function toSignalRequireSync<T>(source: Observable<T>): Signal<T> {
  // A promise that the source emits on subscribe. Broken, it throws at subscribe time rather
  // than reading undefined from something typed as non-optional.
  return toSignal(source, { requireSync: true });
}

export function toObservableFrom<T>(source: Signal<T>): Observable<T> {
  return toObservable(source);
}

export function countingSignal<T>(
  source: Observable<T>,
  counter: { count: number },
  initial: T,
): Signal<T> {
  // Counted with a tap in the pipeline rather than an effect, so it works with no change
  // detection at all. toSignal subscribes eagerly, so this runs without anyone reading it.
  return toSignal(
    source.pipe(tap(() => (counter.count += 1))),
    { initialValue: initial },
  );
}

export function makeSignal<T>(initial: T): WritableSignal<T> {
  return signal(initial);
}
