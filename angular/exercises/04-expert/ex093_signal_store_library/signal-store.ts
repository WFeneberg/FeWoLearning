import { Signal, WritableSignal, signal } from "@angular/core";

// Exercise 093 — a signal store library: a reusable, generic store factory (expert).
// Goal:   turn exercise 085's ONE typed store (CartStore: one State, one Action, one Injectable
//         class) into a LIBRARY — a single generic function any feature can call to get its own
//         independently-isolated typed store, without writing a new @Injectable class every time.
// Drills: a generic `createSignalStore<State, Action>(initialState, reducer)` factory, returning
//         `state` as a genuinely read-only Signal (via `WritableSignal.asReadonly()`, not just a
//         type annotation the caller could still cast around), and confirming two instances of the
//         SAME factory call never leak state into each other.
// Passes: when `npx jest exercises/04-expert/ex093_signal_store_library` is green.
//
// ex085's CartStore hardcodes CartState and CartAction into the class itself — fine for one
// domain, but a second feature (a wizard's step state, a settings panel, anything else that wants
// "one signal + a pure reducer + dispatch()") would have to copy the whole class and rename every
// type. A factory function sidesteps that: State and Action are generic parameters, so the SAME
// function produces a fully independent, fully typed store for however many domains call it — no
// class, no @Injectable, no DI at all required (a caller that DOES want DI can still put the
// factory's result behind a service; this file is agnostic about that and lower-level than it).
//
// "Independently isolated" is the property actually worth testing here, not just implementing:
// every call to createSignalStore must allocate ITS OWN signal, closed over inside the returned
// object — never a signal shared at module scope, which is the classic way a "singleton by
// accident" bug creeps into a factory function (works fine with one caller, silently corrupts state
// the moment a second caller shows up).
//
// `state` being read-only is not just documentation: `WritableSignal.asReadonly()` returns an
// object that genuinely has no `set`/`update` methods on it at runtime, so a consumer that only
// receives `store.state` (never the whole store) has no way to bypass `dispatch()` even if it
// wanted to — the reducer is the only path a next State can ever come from.

export interface SignalStore<State, Action> {
  /** Read-only — the only way to change it is dispatch(). */
  readonly state: Signal<State>;
  dispatch(action: Action): void;
}

/**
 * TODO: implement createSignalStore.
 *   - Hold `initialState` in a private WritableSignal, created FRESH on every call (never a
 *     module-level signal shared across calls).
 *   - `state` on the returned object must be that signal's `.asReadonly()` view, not the writable
 *     signal itself.
 *   - `dispatch(action)` must be the ONLY way state changes: call `reducer(currentState, action)`
 *     and write the result back — never mutate the current state in place.
 */
export function createSignalStore<State, Action>(
  initialState: State,
  reducer: (state: State, action: Action) => State,
): SignalStore<State, Action> {
  throw new Error("TODO: implement createSignalStore");
}
