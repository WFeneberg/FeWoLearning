// Exercise 092 — route parameters from a literal (expert).
// Goal:   derive a parameters object from a route pattern, so a typo in
//         the pattern or in the lookup is a compile error.
// Drills: recursive template matching with a marker, building an object
//         from a union, composing with the runtime matcher.
// Passes: every shape of pattern yields the right parameter names, and
//         matchRoute reports them.
//
// This is the type behind every typed router, and it is ex091 with a
// marker instead of a delimiter. Match `:` and take everything up to the
// next `/`:
//
//   S extends `${string}:${infer Param}/${infer Rest}`
//     ? { [K in Param]: string } & RouteParams<`/${Rest}`>
//     : S extends `${string}:${infer Param}`
//       ? { [K in Param]: string }
//       : {}
//
// Two arms because a parameter in the MIDDLE is followed by a slash and
// the LAST one is not, and the greedy-from-the-left rule (ex075) is why
// they have to be tried in that order.
//
// The result is an intersection, which is what the recursion naturally
// produces. That matters for the facts: an intersection of object types
// is not the same TYPE as the flat object it describes, so they are
// asserted through their keys, exactly as ex054's action map was.
//
// A limit worth stating rather than discovering: this handles `:name`
// segments only. Optional parameters, wildcards and regular-expression
// constraints each need their own arm, and a real router has all of
// them.

/** TODO: an object with one string property per `:param` in S. */
export type RouteParams<S extends string> = unknown;

/**
 * TODO: match `path` against `pattern`, returning the captured
 * parameters, or undefined when the shapes do not line up. Segment
 * counts must match; a literal segment must match exactly.
 */
export function matchRoute<S extends string>(
  _pattern: S,
  _path: string,
): RouteParams<S> | undefined {
  throw new Error("TODO: implement matchRoute");
}
