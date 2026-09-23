// Exercise 100 — a schema that is its own type (expert).
// Goal:   write a validator whose output type is derived from the
//         schema value, so the two cannot drift.
// Drills: a schema as a discriminated union of values, a recursive
//         Infer, two mapped types intersected, a narrowing parse.
// Passes: Infer produces the right type for every schema shape, and
//         parse accepts and rejects the right values.
//
// The last exercise, and it uses most of the track. A schema here is a
// plain value, and `Infer<S>` walks it with a recursive conditional
// (ex071) over a discriminated union (ex015), building an object with a
// mapped type (ex036) and marking optional fields with a modifier
// (ex037). The runtime `parse` is ex035's boundary validation, narrowing
// through a type predicate (ex014).
//
// What that buys is the thing every schema library is sold on: ONE
// declaration. The validator and the type are the same artefact, so a
// field added to the schema appears in the type and a field removed
// breaks the code that used it. Writing the type by hand beside the
// validator is the arrangement this replaces, and the reason is not
// convenience — two hand-written things drift, and the drift is silent.
//
// The interesting part is `optional`. It cannot be a flag the mapped
// type reads per key and turns into `?`, because a mapped type applies
// ONE modifier to all of its keys. It has to be two mapped types
// intersected — the required keys and the optional keys built
// separately — which is ex041's PartialBy arrived at from the other
// direction.
//
// One thing you will hit, and it is ex095 arriving in practice five
// exercises later: casting a value to `Infer<S>` inside a function
// generic over S makes the checker compare against a RECURSIVE Infer
// instantiated on a generic, and that blows the instantiation depth
// limit — "Type instantiation is excessively deep", at the cast.
// Measured: casting through `unknown` does NOT help, because the
// comparison still happens against the return annotation. `as never`
// does: never is assignable to everything, so nothing is compared.
//
// The Schema union is given so the tests have something to name.
// Everything else is yours.

export type Schema =
  | { readonly kind: "string" }
  | { readonly kind: "number" }
  | { readonly kind: "boolean" }
  | { readonly kind: "array"; readonly of: Schema }
  | { readonly kind: "optional"; readonly of: Schema }
  | { readonly kind: "object"; readonly fields: { readonly [key: string]: Schema } };

/** TODO: the type a value matching S must have. */
export type Infer<S> = unknown;

/** TODO: true when `value` matches `schema` — and narrow it. */
export function matches<S extends Schema>(_schema: S, _value: unknown): boolean {
  throw new Error("TODO: implement matches");
}

/** TODO: `value` when it matches, undefined otherwise. */
export function parse<S extends Schema>(_schema: S, _value: unknown): Infer<S> | undefined {
  throw new Error("TODO: implement parse");
}
