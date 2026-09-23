// Reference solution — exercise 097.
export interface Transitions {
  idle: { start: "running" };
  running: { pause: "paused"; finish: "done" };
  paused: { resume: "running"; finish: "done" };
  done: Record<string, never>;
}

export type State = keyof Transitions;

export type EventsFor<S extends State> = keyof Transitions[S];

export type Next<S extends State, E> = E extends EventsFor<S>
  ? Transitions[S][E] extends State
    ? Transitions[S][E]
    : never
  : never;

export interface Machine<S extends State> {
  readonly state: S;
  send<E extends EventsFor<S>>(event: E): Machine<Next<S, E>>;
}

export function machine<S extends State>(state: S): Machine<S> {
  return {
    state,
    send(event) {
      // The table is the type; at runtime the transition is a lookup,
      // and it cannot miss because the call site was checked.
      const table = TRANSITIONS[state] as Record<string, State>;
      return machine(table[event as string] as never);
    },
  };
}

const TRANSITIONS: { [S in State]: Record<string, State> } = {
  idle: { start: "running" },
  running: { pause: "paused", finish: "done" },
  paused: { resume: "running", finish: "done" },
  done: {},
};
