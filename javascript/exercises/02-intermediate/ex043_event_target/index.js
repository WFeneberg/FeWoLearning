// Exercise 043 — EventTarget (intermediate).
// Goal:   use the platform's event plumbing instead of writing one.
// Drills: extending EventTarget, CustomEvent and its `detail`,
//         once/removeEventListener, and the deduplication rule that
//         surprises everyone.
// Passes: on() returns a working unsubscribe, once() really fires once,
//         and addTwice() shows that the same function with the same
//         options is only registered once.

export class Bus extends EventTarget {
  /**
   * Dispatches a CustomEvent named `name` carrying `detail`.
   * Returns what dispatchEvent returns.
   *
   * TODO: implement.
   */
  emit(_name, _detail) {
    throw new Error("TODO: implement emit");
  }

  /**
   * Subscribes `fn` — called with the DETAIL, not the event — and returns a
   * function that unsubscribes it.
   *
   * TODO: implement.
   */
  on(_name, _fn) {
    throw new Error("TODO: implement on");
  }

  /**
   * Like on(), but auto-removed after the first event. Use the `once`
   * option rather than unsubscribing by hand.
   *
   * TODO: implement.
   */
  once(_name, _fn) {
    throw new Error("TODO: implement once");
  }
}

/**
 * Adds THE SAME function as a listener for the same event twice on a fresh
 * Bus, emits once, and returns how many times it was called.
 *
 * The answer is 1: addEventListener ignores a duplicate of the same
 * function with the same capture setting. An array-of-callbacks emitter
 * written by hand would answer 2.
 *
 * TODO: implement.
 */
export function addTwiceCallCount() {
  throw new Error("TODO: implement addTwiceCallCount");
}
