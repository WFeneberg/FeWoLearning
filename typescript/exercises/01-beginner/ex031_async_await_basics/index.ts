// Exercise 031 — async functions and where their failures go (beginner).
// Goal:   see what `async` does to a function's type and to its throws.
// Drills: the inferred Promise return type, awaiting, flattening, rejection
//         instead of a synchronous throw.
// Passes: the three loaders report the right Promise types, and alwaysFails
//         returns a rejected promise rather than throwing at the call site.
//
// Two things `async` does, both of which the facts below grade.
//
// It wraps the return type exactly once: an async function returning a
// string is `() => Promise<string>`, and one returning a Promise<number> is
// `() => Promise<number>` — never Promise<Promise<number>>. Awaiting flattens
// on the way in, and returning flattens on the way out.
//
// And it converts a throw into a REJECTION. A non-async function that throws
// blows up at the call site, before the caller can attach a handler; the same
// body marked `async` hands back a rejected promise instead. That difference
// is invisible until someone calls it without awaiting — which is why
// alwaysFails below starts out non-async, and making it async is the work.
//
// Three of the four carry no return annotation: the graded type is the one
// your body produces.

/** TODO: `n=<value>` from the loader — e.g. `n=42`. */
export async function loadLabel(_load: () => Promise<number>) {
  throw new Error("TODO: implement loadLabel");
}

/** TODO: return the loader's promise directly, without awaiting it. */
export async function passThrough(_load: () => Promise<number>) {
  throw new Error("TODO: implement passThrough");
}

/** TODO: the loaded value, or -1 if the loader rejects. */
export async function loadOrFallback(_load: () => Promise<number>) {
  throw new Error("TODO: implement loadOrFallback");
}

/**
 * TODO: always fail with `new Error("nope")` — but as a REJECTION. Calling
 * this must not throw at the call site, so it has to be async.
 */
export function alwaysFails(): Promise<never> {
  throw new Error("TODO: implement alwaysFails");
}
