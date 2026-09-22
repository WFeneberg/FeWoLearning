// Exercise 032 — the event loop: one thread, two queues (beginner).
// Goal:   predict the order in which scheduled work actually runs.
// Drills: microtasks vs macrotasks, await always yielding, a synchronous
//         loop blocking everything.
// Passes: all three functions report the order the runtime really produces.
//
// This is the largest gap between here and .NET, and it is not a detail of
// the library — it is the execution model. There is no thread pool. There is
// ONE thread, and it runs a queue.
//
// Synchronous code runs to completion first. Then the MICROTASK queue drains
// entirely — everything queued by queueMicrotask, by a .then callback, or by
// resuming after an await — and only when it is empty does the loop take ONE
// MACROTASK (a setTimeout callback, an I/O completion) and start over. A
// microtask that queues another microtask is still drained in the same pass,
// so microtasks can starve timers indefinitely.
//
// Two consequences a C# instinct gets wrong:
//
// `await` ALWAYS yields, even on a value that is already available. In C#,
// awaiting a completed Task continues synchronously; here, `await 1` still
// suspends and resumes on the microtask queue. The code after it runs later
// than the code after the call.
//
// And a long synchronous loop does not merely slow things down — it stops
// them. No timer, no promise callback, no incoming request runs while it
// spins, because there is nowhere else for them to run.
//
// Work the orders out from the model above rather than looking them up; that
// is the exercise.

/**
 * TODO: log "sync" immediately, then schedule — IN THIS ORDER — a
 * queueMicrotask logging "micro", a Promise.resolve().then logging
 * "promise", and a setTimeout(…, 0) logging "macro". Resolve with the log
 * once all four have run.
 */
export function scheduleAll(): Promise<string[]> {
  throw new Error("TODO: implement scheduleAll");
}

/**
 * TODO: build this shape and resolve with the log.
 *   - an async function that logs "a", awaits the PLAIN NUMBER 1 (not a
 *     promise), then logs "b"
 *   - call it without awaiting, then log "c"
 *   - await it, then resolve with the log
 */
export function awaitAlwaysYields(): Promise<string[]> {
  throw new Error("TODO: implement awaitAlwaysYields");
}

/**
 * TODO: schedule a setTimeout(…, 0) that sets a flag, then busy-wait
 * synchronously until at least `ms` milliseconds have passed, and return
 * whether the flag was set by the time the wait ended.
 *
 * The answer does not depend on `ms`, and that is the point.
 */
export function timerRanDuringBusyWait(_ms: number): boolean {
  throw new Error("TODO: implement timerRanDuringBusyWait");
}
