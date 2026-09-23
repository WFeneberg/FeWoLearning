// Exercise 087 — structured concurrency (advanced).
// Goal:   run work in parallel under one owner, so nothing it started
//         outlives the call.
// Drills: fail-fast over a set of tasks, cancelling siblings with an
//         AbortSignal, settling once.
// Passes: every task gets the group's signal, the first rejection aborts
//         the rest, and the group still waits for all of them.
//
// `Promise.all` fails fast in one sense only: it REJECTS on the first
// rejection. The other promises keep running, unobserved, their results
// and their errors going nowhere. That is the leak structured
// concurrency exists to close — when the group returns, nothing it
// started is still running.
//
// There is no primitive for it. What there is: hand every task the same
// AbortSignal (ex058), abort it the moment one fails, and wait for all
// of them to settle before returning — including the ones on their way
// out. The tasks have to cooperate, exactly as in ex058; a task that
// ignores its signal cannot be stopped, only waited for.
//
// TWO WAYS TO GET THIS WRONG, and the facts are built around both.
// Returning on the first rejection without waiting is the same leak in
// different clothing. And awaiting the tasks in a loop makes the whole
// thing serial, which no fact about RESULTS can see — so the concurrency
// is graded by observing that every task has started before any of them
// has finished.
//
// A warning, because this row can HANG rather than fail: a group that
// waits on something which is itself waiting on the group never settles,
// and a hung test reports nothing at all rather than going red. If a run
// stops producing output here, that is what happened.

export interface GroupResult<T> {
  /** Each task's value, in task order; undefined where it did not finish. */
  values: (T | undefined)[];
  /** The first failure's message, or undefined when all succeeded. */
  failure: string | undefined;
}

/**
 * TODO: run every task concurrently, handing each the group's signal.
 * On the first rejection, abort the signal and let the others wind down,
 * but still wait for all of them before returning.
 */
export async function runGroup<T>(
  _tasks: readonly ((signal: AbortSignal) => Promise<T>)[],
): Promise<GroupResult<T>> {
  throw new Error("TODO: implement runGroup");
}
