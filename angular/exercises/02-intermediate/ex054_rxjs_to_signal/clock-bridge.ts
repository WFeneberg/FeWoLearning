import { Signal, WritableSignal, signal } from "@angular/core";
import { Observable } from "rxjs";

// Exercise 054 — toSignal and toObservable (intermediate).
// Goal:   move a value between the RxJS world and the signal world.
// Drills: toSignal with an initialValue, toSignal({requireSync}), toObservable, the injection
//         context requirement, and what happens to the subscription when the injector is destroyed.
// Passes: when `npx jest exercises/02-intermediate/ex054_rxjs_to_signal` is green.
//
// The two worlds solve different problems. An Observable models a *sequence over time* and can
// complete or error; a signal models a *current value* and always has one. That asymmetry is why
// the bridge is not symmetric either.
//
// toSignal has to answer "what is the value before the observable has emitted?", and it makes you
// answer it. Give an `initialValue` and you get `Signal<T>`. Give nothing and the type becomes
// `Signal<T | undefined>` — honest, and a nuisance downstream. Claim `requireSync: true` and you
// promise the source emits synchronously on subscribe; if it does not, you get an error at
// subscribe time rather than a silent undefined, which is the right trade for a BehaviorSubject.
//
// Both functions need an injection context, because both create a subscription that has to be
// torn down with something. That is the feature, not the restriction: toSignal unsubscribes when
// the injector is destroyed, so the leak from exercise 022 cannot happen here.

/**
 * TODO: bridge an observable into a signal, with a starting value.
 *
 * Must be called from an injection context — the spec uses TestBed.runInInjectionContext.
 */
export function toSignalWithInitial<T>(source: Observable<T>, initial: T): Signal<T> {
  throw new Error("TODO: implement toSignalWithInitial");
}

/**
 * TODO: bridge an observable into a signal with no starting value.
 *
 * The result reads undefined until the source emits, which is exactly what the type says.
 */
export function toSignalMaybe<T>(source: Observable<T>): Signal<T | undefined> {
  throw new Error("TODO: implement toSignalMaybe");
}

/**
 * TODO: bridge a source that is promised to emit synchronously.
 *
 * Use requireSync so the signal is `Signal<T>` rather than `Signal<T | undefined>`. A source that
 * breaks the promise must fail loudly rather than read undefined.
 */
export function toSignalRequireSync<T>(source: Observable<T>): Signal<T> {
  throw new Error("TODO: implement toSignalRequireSync");
}

/** TODO: bridge a signal back into an observable. Also needs an injection context. */
export function toObservableFrom<T>(source: Signal<T>): Observable<T> {
  throw new Error("TODO: implement toObservableFrom");
}

/**
 * TODO: a signal that tracks how many times the source emitted.
 *
 * Built by bridging `source` into a signal and counting into `counter.count` as values arrive.
 * Use a tap in the pipeline rather than an effect — this must work with no change detection.
 */
export function countingSignal<T>(
  source: Observable<T>,
  counter: { count: number },
  initial: T,
): Signal<T> {
  throw new Error("TODO: implement countingSignal");
}

/** A convenience for the spec: a writable signal to bridge in the other direction. */
export function makeSignal<T>(initial: T): WritableSignal<T> {
  return signal(initial);
}
