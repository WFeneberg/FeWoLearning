// Exercise 100 — a promise from scratch (expert).
// Goal:   build the thing the whole async tier stands on.
// Drills: the three states, settle-once, callbacks queued as MICROTASKS,
//         then() returning a new promise, thenable adoption, and the
//         rules that make a chain behave.
// Passes: a handler never runs synchronously — even on an
//         already-settled promise — and the result of then() adopts a
//         promise a handler returns instead of nesting it.
//
// This is the Promises/A+ core. Everything else (catch, finally, all) is
// built on `then`, and this row builds those too.

export class Thenable {
  /**
   * new Thenable((resolve, reject) => …). The executor runs
   * SYNCHRONOUSLY; a throw from it rejects the promise.
   *
   * TODO: implement. Keep the state and the callback queues private.
   */
  constructor(_executor) {
    throw new Error("TODO: implement the Thenable constructor");
  }

  /**
   * Registers handlers and returns a NEW Thenable for their result.
   *
   *   - handlers always run as microtasks, never synchronously
   *   - a missing handler passes the value or reason through
   *   - a handler's return value fulfils the new promise
   *   - a handler that throws rejects it
   *   - a handler returning a Thenable (or any thenable) is ADOPTED: the
   *     new promise settles with its eventual value, not with the
   *     promise itself
   *
   * TODO: implement.
   */
  then(_onFulfilled, _onRejected) {
    throw new Error("TODO: implement then");
  }

  /** then(undefined, onRejected). TODO: implement. */
  catch(_onRejected) {
    throw new Error("TODO: implement catch");
  }

  /**
   * Runs `onFinally` either way and passes the original outcome through —
   * its return value is discarded, but a throw from it takes over.
   *
   * TODO: implement in terms of then().
   */
  finally(_onFinally) {
    throw new Error("TODO: implement finally");
  }

  /** An already-fulfilled Thenable — or `value` itself if it is one. TODO. */
  static resolve(_value) {
    throw new Error("TODO: implement Thenable.resolve");
  }

  /** An already-rejected Thenable. TODO: implement. */
  static reject(_reason) {
    throw new Error("TODO: implement Thenable.reject");
  }

  /**
   * Fulfils with an array of every value, in input order, or rejects with
   * the first rejection. Accepts plain values alongside thenables.
   *
   * TODO: implement.
   */
  static all(_values) {
    throw new Error("TODO: implement Thenable.all");
  }
}
