import { Signal, WritableSignal, signal } from "@angular/core";

// Exercise 093 — a signal store library: a reusable, generic store factory (reference solution).

export interface SignalStore<State, Action> {
  readonly state: Signal<State>;
  dispatch(action: Action): void;
}

export function createSignalStore<State, Action>(
  initialState: State,
  reducer: (state: State, action: Action) => State,
): SignalStore<State, Action> {
  // Fresh signal per call — this is what keeps two stores from the same factory isolated.
  const stateSignal: WritableSignal<State> = signal(initialState);

  return {
    state: stateSignal.asReadonly(),
    // The only place "next state" is ever computed — dispatch never touches state by hand.
    dispatch: (action) => stateSignal.update((current) => reducer(current, action)),
  };
}
