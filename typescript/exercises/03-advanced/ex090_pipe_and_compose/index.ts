// Exercise 090 — pipe and compose, typed through the chain (advanced).
// Goal:   let a chain of functions of different types check end to end,
//         with nothing annotated at the call site.
// Drills: overload sets as a variadic signature, inference flowing from
//         one position to the next, the implementation signature that
//         has to cover them all.
// Passes: each arity reports the right end-to-end type, a mismatched
//         link is rejected, and compose reads the other way.
//
// ex051's pipe2 did this for two functions by letting B flow out of the
// first and into the second. Generalising it is where TypeScript runs
// into a wall: a truly variadic `pipe<F extends readonly Fn[]>` cannot
// express "each function's input is the previous one's output" without
// a recursive type that degrades badly and produces unreadable errors.
//
// What every real library does instead is write OVERLOADS — one per
// arity, up to some number — and it is not a workaround so much as the
// right tool: each overload is a plain, readable signature, inference is
// exactly as good as ex051's two-function case, and the error for a
// mismatched link points at the link.
//
// The implementation signature underneath has to accept all of them, so
// it is loose, and it is invisible to callers (ex010). That is the whole
// trade: one unchecked function body in exchange for checked call sites.
//
// compose is pipe backwards — `compose(f, g)(x)` is `f(g(x))` — and the
// only interesting thing about it is that the overloads run the other
// way, which is exactly the sort of thing that is easier to get wrong
// than to notice.
//
// The stubs carry a single loose signature, so the tests can call them
// while the overloads are missing. Adding the overload set above each
// implementation is the work.

/** TODO: overloads for one, two and three functions, left to right. */
export function pipe(...fns: readonly ((input: never) => unknown)[]): (input: never) => unknown {
  void fns;
  throw new Error("TODO: implement pipe");
}

/** TODO: overloads for one, two and three functions, right to left. */
export function compose(
  ...fns: readonly ((input: never) => unknown)[]
): (input: never) => unknown {
  void fns;
  throw new Error("TODO: implement compose");
}
