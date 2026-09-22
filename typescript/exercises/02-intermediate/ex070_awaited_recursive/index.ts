// Exercise 070 — rebuilding Awaited (intermediate).
// Goal:   unwrap promises all the way down, thenables included.
// Drills: a recursive conditional type, inferring through a method
//         signature, distribution over a union.
// Passes: every nesting depth collapses to the value type, a thenable is
//         treated as a promise, and a plain type is left alone.
//
// ex040's Resolved peeled one layer. The real Awaited recurses — because
// the runtime does: `await` keeps unwrapping while what it has is
// thenable, so `Promise<Promise<string>>` resolves to a string, not to a
// promise.
//
// The part worth doing properly is the THENABLE. `await` does not check
// for Promise; it checks for an object with a callable `then`, which is
// how libraries predating Promise still interoperate. Matching that means
// inferring through a method signature: find `then(onfulfilled: (value:
// infer V) => …): …` and recurse on V.
//
// Note that T is bare in the checked position, so this distributes over a
// union (ex046) — `MyAwaited<Promise<string> | number>` is
// `string | number`, member by member, which is what you want and what
// you get for free.
//
// settle carries no return annotation: the graded type is the one your
// body produces.

/** TODO: the type `await` would produce for T, at any nesting depth. */
export type MyAwaited<T> = unknown;

/** TODO: await `value` and hand back what came out. */
export async function settle<T>(_value: T) {
  throw new Error("TODO: implement settle");
}
