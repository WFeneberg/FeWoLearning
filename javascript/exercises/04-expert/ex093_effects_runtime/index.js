// Exercise 093 — a generator-driven effect runtime (expert).
// Goal:   describe side effects as data, and interpret them elsewhere.
// Drills: a generator yielding requests, a driver feeding results back
//         with next(value), errors injected with throw(), and the payoff —
//         the same program under a real and a fake interpreter.
// Passes: a program written once runs against test handlers with no
//         mocking, because it never performed an effect itself.

/** An effect descriptor: { type, payload }. TODO: implement. */
export function effect(_type, _payload) {
  throw new Error("TODO: implement effect");
}

/**
 * Runs a generator program: each yielded effect is looked up in
 * `handlers` by `type` and called with its payload. The (awaited) result
 * is sent back into the generator; a handler that throws has its error
 * THROWN INTO the generator, so the program can catch it.
 *
 * Resolves with the generator's return value. An unknown effect type
 * rejects with a RangeError — and, like any failure, must still let the
 * generator's own finally blocks run.
 *
 * TODO: implement.
 */
export async function run(_program, _handlers) {
  throw new Error("TODO: implement run");
}

/**
 * A program, as a generator: reads a user by id, then reads that user's
 * orders, and returns `<name> has <n> orders`. If the "readOrders" effect
 * fails, it returns `<name> has unknown orders` instead.
 *
 * Effects: effect("readUser", id) -> { name }, effect("readOrders", id) ->
 * an array.
 *
 * TODO: implement.
 */
export function* describeUser(_id) {
  throw new Error("TODO: implement describeUser");
}
