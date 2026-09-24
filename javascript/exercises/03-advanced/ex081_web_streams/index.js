// Exercise 081 — web streams (advanced).
// Goal:   the platform's streaming primitives, without a network in sight.
// Drills: ReadableStream with a pull source, a reader loop, TransformStream,
//         pipeThrough, and cancelling a reader.
// Passes: pull is called on demand rather than up front, so an endless
//         source is safe — the tests read a few chunks and cancel.

/**
 * A ReadableStream over `values`, enqueuing ONE value per pull.
 *
 * TODO: implement with `new ReadableStream({ pull(controller) { … } })`.
 * Close the controller once the values run out.
 */
export function streamFrom(_values) {
  throw new Error("TODO: implement streamFrom");
}

/**
 * Reads a stream to the end and returns its chunks as an array.
 *
 * TODO: implement — a getReader() loop, or for await..of, which a
 * ReadableStream supports directly.
 */
export function collectStream(_stream) {
  throw new Error("TODO: implement collectStream");
}

/**
 * A TransformStream applying `fn` to every chunk.
 *
 * TODO: implement.
 */
export function mapStream(_fn) {
  throw new Error("TODO: implement mapStream");
}

/**
 * Reads at most `count` chunks and then CANCELS the stream, returning
 * { chunks, cancelled: true }. Cancelling releases the source, which the
 * tests observe through the stream's own cancel handler.
 *
 * TODO: implement with a reader and reader.cancel().
 */
export function takeAndCancel(_stream, _count) {
  throw new Error("TODO: implement takeAndCancel");
}
