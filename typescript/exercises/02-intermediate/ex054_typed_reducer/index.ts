// Exercise 054 — a reducer, and the action-map idiom (intermediate).
// Goal:   derive an action union from a payload map instead of writing it
//         out, then reduce over it.
// Drills: a mapped type indexed by its own keys, intersecting a tag onto a
//         payload, narrowing in a reducer.
// Passes: Action is the three-member union derived from ActionMap, and
//         reduce handles each one without mutating the state it was given.
//
// Writing a discriminated union by hand (ex015) repeats the tag in every
// member, and the tag is the one part that must not drift. The ACTION MAP
// idiom writes the payloads once and derives the union:
//
//   { [K in keyof Map]: { type: K } & Map[K] }[keyof Map]
//
// Read it inside out. The mapped type produces an object whose values are
// the tagged members; indexing it with `[keyof Map]` takes the union of
// those values. Adding an entry to the map adds a member to the union, and
// ex055's exhaustiveness check then points at every place that has to
// handle it.
//
// The reducer itself is ex015's switch doing real work: narrowing on
// `action.type` is what makes `action.amount` reachable in one arm and not
// in the others.

export interface State {
  items: readonly string[];
  total: number;
}

/** Given. Each entry is an action's payload, keyed by its tag. */
export interface ActionMap {
  add: { item: string; amount: number };
  remove: { item: string };
  clear: Record<string, never>;
}

/** TODO: the union of tagged actions, derived from ActionMap. */
export type Action = unknown;

/**
 * TODO: apply an action, returning a NEW state.
 *   add    — appends the item and adds the amount to total
 *   remove — drops every copy of the item, leaving total alone
 *   clear  — empty items, total 0
 */
export function reduce(_state: State, _action: Action): State {
  throw new Error("TODO: implement reduce");
}
