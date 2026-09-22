// Exercise 062 — typing `this` (intermediate).
// Goal:   say what a function expects to be called on, and check it.
// Drills: the `this` parameter, ThisParameterType, what an arrow captures.
// Passes: increment only accepts a Counter as its receiver, and the two
//         ways of detaching a method behave differently.
//
// A `this` parameter is a FAKE first parameter: `function f(this: T, x: N)`
// is called as `f.call(t, n)` or `t.f(n)`, takes one real argument, and
// emits nothing — `this` is erased entirely. It is the only way to say
// what a standalone function expects to be called on, and with `strict`
// (which includes strictBindCallApply) the checker enforces it at every
// call, bind and apply.
//
// The other half is the oldest trap in JavaScript. A method pulled off its
// object — `const f = counter.increment` — loses its receiver, and `this`
// is undefined when it runs. An ARROW function has no `this` of its own
// and cannot take a `this` parameter at all; it closes over the `this` of
// wherever it was written, which is what makes `() => this.x` the usual
// fix and also what makes an arrow useless as a method on a prototype.
//
// The stub's signature has no `this` parameter, so adding it is the work.

export interface Counter {
  count: number;
}

/** TODO: add `by` to the receiver's count and return the new value.
 *  Declare what this function must be called on. */
export function increment(_by: number): number {
  throw new Error("TODO: implement increment");
}

/** TODO: a zero-argument function that increments `counter` by 1 each time
 *  it is called, whatever it is called on — so it keeps working after
 *  being passed around. */
export function ticker(_counter: Counter): () => number {
  throw new Error("TODO: implement ticker");
}
