// Exercise 072 — DeepPartial, and knowing when to stop (advanced).
// Goal:   make every property optional at every depth without destroying
//         the values on the way down.
// Drills: recursion with several guard clauses, choosing leaves.
// Passes: nesting becomes optional, and a Date, a function and an array's
//         elements survive intact.
//
// The recursion is three lines. Picking the LEAVES is the exercise, and
// getting it wrong produces a type that still compiles and is quietly
// useless.
//
// A `Date` is the canonical casualty. It is an object, so a naive
// DeepPartial recurses into it and produces
// `{ toISOString?: () => string; getTime?: () => number; … }` — which
// accepts `{}`, accepts an object with a wrong-arity `getTime`, and is
// not a Date. The same applies to Map, Set, RegExp and every other
// built-in with behaviour rather than data. They are LEAVES.
//
// A function is the other one, for ex065's reason: mapping over it
// produces `{}` and the signature is gone.
//
// And an array wants thought. Making the PROPERTY optional is right;
// making its ELEMENTS optional is not — `tags?: string[]` is a patch
// that may omit the list, while `tags?: (string | undefined)[]` is a list
// with holes in it, which nobody meant. Recursing into the elements is
// still right when they are objects, so that a patch can describe a
// partial row.
//
// One consequence of this track's exactOptionalPropertyTypes (ex005) worth
// stating: a patch OMITS a key. `{ id: undefined }` is not a legal
// DeepPartial<Profile> here at all, because `id?: string` does not admit
// undefined — so mergeDeep never has to distinguish "absent" from
// "explicitly cleared", and neither do you.

export interface Profile {
  id: string;
  createdAt: Date;
  owner: { name: string; email: string };
  tags: string[];
  rows: { label: string; count: number }[];
  render: (value: string) => string;
}

/** TODO: optional at every level, with the leaves above left alone. */
export type DeepPartial<T> = unknown;

/** TODO: apply `patch` on top of `base`, recursing into plain objects and
 *  replacing everything else outright. Returns a new object. */
export function mergeDeep(_base: Profile, _patch: DeepPartial<Profile>): Profile {
  throw new Error("TODO: implement mergeDeep");
}
