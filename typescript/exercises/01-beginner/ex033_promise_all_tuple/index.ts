// Exercise 033 — Promise.all keeps a tuple (beginner).
// Goal:   await several differently typed promises without losing which is
//         which.
// Drills: tuple inference through Promise.all, result order vs settle
//         order, fail-fast.
// Passes: loadAll reports Promise<[string, number, boolean]>, its results
//         come back in argument order, and one rejection fails the lot.
//
// Handed an ARRAY LITERAL of promises, Promise.all infers a tuple and each
// position keeps its own type. Handed an array variable typed
// Promise<unknown>[], it infers unknown[] and the distinction is gone — so
// the literal at the call site is doing real work, and the type fact below
// is what separates the two.
//
// Two behaviours worth having in your hands, both graded at runtime: the
// results are in ARGUMENT order regardless of which promise settles first,
// and the combined promise rejects as soon as any one of them does, without
// waiting for the rest.
//
// No return annotation on loadAll: the graded type is the one your body
// produces.

/** TODO: await all three and give back their values, positions preserved. */
export async function loadAll(
  _a: Promise<string>,
  _b: Promise<number>,
  _c: Promise<boolean>,
) {
  throw new Error("TODO: implement loadAll");
}

/**
 * TODO: resolve with the values when every promise succeeds, or with
 * `failed:<message>` as soon as any one of them rejects with an Error.
 */
export async function loadAllOrMessage(
  _promises: readonly Promise<number>[],
): Promise<number[] | string> {
  throw new Error("TODO: implement loadAllOrMessage");
}
