// Exercise 051 — where inference comes from (intermediate).
// Goal:   write signatures that let callers leave the annotations out.
// Drills: contextual typing of a callback parameter, inference flowing
//         from one function position into the next.
// Passes: mapEach infers both its element and result types, and pipe2
//         types its second function's parameter from the first's return.
//
// A callback parameter is CONTEXTUALLY TYPED: TypeScript looks at the
// signature it is being passed to and types the parameter from there, so
// callers write `(n) => n * 2` and never `(n: number) => n * 2`. That only
// works at the call site, and only because the parameter's type is known
// from the surrounding expression.
//
// Pull the same lambda out into a variable first and the context is gone:
//
//   const double = (n) => n * 2;   // error: n implicitly has type any
//   mapEach(items, double);        // too late; the annotation was needed above
//
// Nothing in this row grades that (it is an error either way, with or
// without your work), but it is the single most common reason a codebase
// ends up sprinkled with annotations it does not need.
//
// pipe2 is the sharper case: B is inferred from the FIRST function's return
// and then supplies the SECOND function's parameter, so the two are checked
// against each other without either being written down.
//
// The stubs take `(item: never) => unknown` rather than `unknown` so the
// tests' unannotated callbacks stay contextually typed instead of becoming
// implicit anys.

/** TODO: apply `fn` to every item, keeping both types. */
export function mapEach(_items: readonly never[], _fn: (item: never) => unknown): unknown {
  throw new Error("TODO: implement mapEach");
}

/** TODO: a function that runs `first`, then `second` on its result. */
export function pipe2(
  _first: (input: never) => unknown,
  _second: (middle: never) => unknown,
): unknown {
  throw new Error("TODO: implement pipe2");
}
