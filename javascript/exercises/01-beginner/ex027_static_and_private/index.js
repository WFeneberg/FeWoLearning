// Exercise 027 — statics and private fields (beginner).
// Goal:   real privacy, enforced by the language rather than by convention.
// Drills: #private fields and methods, static fields, a static counter, the
//         `#field in object` brand check.
// Passes: the count is invisible to Object.keys, JSON and any outside
//         reader — and Counter.isCounter() can still recognise one.

export class Counter {
  // TODO: declare a PRIVATE instance field `#count`, starting at 0, and a
  // PRIVATE STATIC field `#created`, also 0.

  /**
   * Increments the created-counter. TODO: implement.
   */
  constructor() {
    throw new Error("TODO: implement the Counter constructor");
  }

  /** Adds `amount` (default 1) and returns the new count. TODO. */
  increment(_amount = 1) {
    throw new Error("TODO: implement increment");
  }

  /** The current count, as a getter. TODO. */
  get value() {
    throw new Error("TODO: implement value");
  }

  /**
   * "Counter(<value>)", built by a PRIVATE method #format() that this
   * method calls.
   *
   * TODO: implement both.
   */
  toString() {
    throw new Error("TODO: implement toString");
  }

  /** How many Counters have been constructed. TODO. */
  static get created() {
    throw new Error("TODO: implement Counter.created");
  }

  /**
   * True only for objects that really carry this class's private field —
   * `#count in candidate`. Unlike instanceof, nothing can fake it and no
   * prototype swap can break it.
   *
   * TODO: implement.
   */
  static isCounter(_candidate) {
    throw new Error("TODO: implement Counter.isCounter");
  }
}
