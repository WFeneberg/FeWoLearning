// Exercise 025 — classes (beginner).
// Goal:   the class syntax, and what it really builds.
// Drills: constructor, methods on the prototype, a getter, a static
//         factory, instanceof, the strictness a class body brings.
// Passes: methods live on the prototype (so Object.keys of an instance
//         shows only the data), and calling Stack without `new` throws.
//
// Coming from C#: `class` is syntax over the prototype model, not a new
// one. Methods are non-enumerable properties of Stack.prototype, shared by
// every instance — there is no per-instance copy.

export class Stack {
  /**
   * Holds its items in an own property named `items`, starting empty.
   *
   * TODO: implement the constructor.
   */
  constructor() {
    throw new Error("TODO: implement the Stack constructor");
  }

  /** Pushes and returns `this`, so calls chain. TODO. */
  push(_value) {
    throw new Error("TODO: implement push");
  }

  /** Removes and returns the top item, or undefined when empty. TODO. */
  pop() {
    throw new Error("TODO: implement pop");
  }

  /** The top item without removing it, or undefined. TODO. */
  peek() {
    throw new Error("TODO: implement peek");
  }

  /** How many items — a GETTER, not a method. TODO. */
  get size() {
    throw new Error("TODO: implement size");
  }

  /**
   * Builds a Stack from any iterable, pushing in iteration order.
   * A static method, so `Stack.from([1, 2])` works.
   *
   * TODO: implement.
   */
  static from(_iterable) {
    throw new Error("TODO: implement Stack.from");
  }
}
