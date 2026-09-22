// Exercise 050 — default type arguments (intermediate).
// Goal:   let a type parameter be left out without losing what it means.
// Drills: `<T = X>` on a type and on a function, defaults alongside
//         constraints, and when the default actually applies.
// Passes: the defaults show up where an argument is missing and step aside
//         where one is given or inferable.
//
// `<T = X>` supplies X when the argument is OMITTED. For a type alias that
// is the whole story. For a function it is subtler: a type parameter is
// normally inferred from the arguments, and the default only applies when
// inference finds no candidates at all — so `makeList()` is `string[]`
// while `makeList(1, 2)` is `number[]`, with nothing written at either call
// site.
//
// A default can coexist with a constraint (`<T extends string = string>`),
// and it must satisfy it. Defaults are also positional, like value
// parameters: once one has a default, the ones after it need one too.
//
// The C# generics comparison ends here — there is no equivalent, and it is
// the reason a library can add a type parameter to a published type without
// breaking every consumer.

/** TODO: a success carrying T, or a failure carrying E — which defaults to
 *  Error, because that is what a failure usually is. */
export type Outcome<T, E = Error> = unknown;

/** TODO: a page of items, defaulting the item type to unknown. Carries
 *  `items` and a numeric `total`. */
export type Page<T = unknown> = unknown;

/** TODO: collect the arguments into an array — and give T a default of
 *  string, so that a call with no arguments at all, where there is nothing
 *  to infer from, still produces string[] rather than unknown[]. */
export function makeList<T>(..._items: T[]): T[] {
  throw new Error("TODO: implement makeList");
}
