// Exercise 015 — discriminated unions (beginner).
// Goal:   model a closed set of alternatives so the checker can tell them
//         apart by one shared field.
// Drills: a literal-typed tag, narrowing by switching on it.
// Passes: AppEvent is the three-member union, and render narrows to reach
//         each member's own fields.
//
// The discriminant has to be a LITERAL type — `type: string` makes the union
// undiscriminated and every field access an error. This is the pattern that
// replaces an abstract base class plus a cast: no inheritance, no instanceof,
// just a field the checker can compare.

/**
 * TODO: a union of three events, each tagged with a `type` field:
 *   click  — with numeric `x` and `y`
 *   key    — with a string `key`
 *   scroll — with a numeric `delta`
 */
export type AppEvent = unknown;

/**
 * TODO: render an event —
 *   click  -> `click@<x>,<y>`
 *   key    -> `key:<key>`
 *   scroll -> `scroll<delta>` with an explicit sign, e.g. `scroll+3`, `scroll-2`
 */
export function render(_event: AppEvent): string {
  throw new Error("TODO: implement render");
}
