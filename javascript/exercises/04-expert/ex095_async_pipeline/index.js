// Exercise 095 — an async-generator pipeline (expert).
// Goal:   compose streaming stages, and unwind them in the right order.
// Drills: async generators as stages, pipe composition, early exit through
//         a whole chain, and the ORDER the finally blocks run in.
// Passes: breaking out of the consumer closes the WHOLE chain. Measured
//         order: the stages START from the consumer end (the last stage's
//         body runs first, because that is the one being pulled), and they
//         END from the source outwards, since each stage's return() awaits
//         its upstream before running its own finally.

/**
 * Composes async-generator stages into one:
 *   pipeline(source, mapStage, filterStage) — each stage is
 *   (asyncIterable) => asyncIterable.
 *
 * TODO: implement.
 */
export function pipeline(_source, ..._stages) {
  throw new Error("TODO: implement pipeline");
}

/**
 * A stage mapping each value with `fn`, which may be async.
 *
 * TODO: implement — return a function taking an async iterable and
 * returning an async generator.
 */
export function mapStage(_fn) {
  throw new Error("TODO: implement mapStage");
}

/**
 * A stage keeping the values `predicate` accepts.
 *
 * TODO: implement.
 */
export function filterStage(_predicate) {
  throw new Error("TODO: implement filterStage");
}

/**
 * A stage that records its own lifecycle in `log`: pushes
 * `<name>:start` before the first value it pulls, `<name>:<value>` per
 * value, and `<name>:end` in a finally block.
 *
 * TODO: implement.
 */
export function traceStage(_name, _log) {
  throw new Error("TODO: implement traceStage");
}

/**
 * Collects at most `limit` values out of an async iterable and then stops
 * pulling, closing the pipeline behind it.
 *
 * TODO: implement.
 */
export async function collectLimit(_asyncIterable, _limit) {
  throw new Error("TODO: implement collectLimit");
}
