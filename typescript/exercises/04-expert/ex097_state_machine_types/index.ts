// Exercise 097 — a machine whose illegal transitions do not compile
// (expert).
// Goal:   put the transition table in the type system, so a wrong move
//         is a build error rather than a runtime guard.
// Drills: a transition map as a type, deriving the allowed events per
//         state, threading the state through a value.
// Passes: every legal move type-checks and reports the state it lands
//         in, and every illegal one is rejected.
//
// The usual state machine checks its transitions at runtime and throws.
// Everything needed to check them at COMPILE time is already in the
// table — it just has to be a type rather than an object.
//
//   Transitions = { idle: { start: "running" }, running: { … }, … }
//
// From there: the events legal in state S are `keyof Transitions[S]`,
// and the state a given event lands in is `Transitions[S][E]`. Both are
// plain indexed access (ex020) over a literal-typed table.
//
// The value side carries the state as a type parameter — `Machine<S>` —
// and `send` returns `Machine<Transitions[S][E]>`, so the chain tracks
// where it is. That is ex096's accumulation with one state instead of a
// union.
//
// Note what this does and does not buy. It rejects a transition the
// table forbids, at the call site, with no runtime check at all. It does
// NOT help when the state comes from outside the program — a state read
// from a database is a string, and something still has to narrow it
// (ex035). Compile-time machines are for code paths, not for data.
//
// The stub's `send` accepts any event and reports the union of every
// state, which is what needs tightening.

export interface Transitions {
  idle: { start: "running" };
  running: { pause: "paused"; finish: "done" };
  paused: { resume: "running"; finish: "done" };
  done: Record<string, never>;
}

export type State = keyof Transitions;

/** TODO: the events legal in state S. */
export type EventsFor<S extends State> = unknown;

/** TODO: the state that event E moves S to. */
export type Next<S extends State, E> = unknown;

export interface Machine<S extends State> {
  readonly state: S;
  /** TODO: move, reporting the state it lands in. Reject an event the
   *  table does not allow from here. */
  send(event: string): Machine<State>;
}

/** TODO: a machine in the given state. */
export function machine<S extends State>(_state: S): Machine<S> {
  throw new Error("TODO: implement machine");
}
