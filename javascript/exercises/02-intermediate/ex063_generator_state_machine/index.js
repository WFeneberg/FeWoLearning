// Exercise 063 — a generator as a state machine (intermediate).
// Goal:   keep the state in the generator's own suspended position.
// Drills: a generator driven by next(event), the state living in local
//         variables instead of a field, and a driver that feeds it.
// Passes: the machine rejects an impossible transition without losing its
//         state, and two machines never share one.
//
// The machine: a turnstile. States "locked" and "open". Events "coin" and
// "push". A coin opens a locked turnstile; a push closes an open one;
// anything else is refused.

/**
 * Yields the CURRENT state, receives the next event through next(event),
 * and never returns on its own.
 *
 * Yield "locked" first. For each event received:
 *   - "coin" while locked -> yield "open"
 *   - "push" while open   -> yield "locked"
 *   - anything else       -> yield the unchanged state
 *
 * TODO: implement.
 */
export function* turnstile() {
  throw new Error("TODO: implement turnstile");
}

/**
 * Feeds `events` into a fresh turnstile and returns the array of states it
 * reported, starting with its initial state.
 *
 * drive(["coin", "push"]) -> ["locked", "open", "locked"]
 *
 * TODO: implement.
 */
export function drive(_events) {
  throw new Error("TODO: implement drive");
}

/**
 * How many events in `events` actually changed the state.
 *
 * TODO: implement using drive() — no second machine.
 */
export function countTransitions(_events) {
  throw new Error("TODO: implement countTransitions");
}
