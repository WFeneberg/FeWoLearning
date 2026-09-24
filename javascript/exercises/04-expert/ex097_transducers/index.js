// Exercise 097 — transducers (expert).
// Goal:   compose map/filter/take without allocating a single intermediate
//         array, and stop early.
// Drills: a reducer-transforming function, composition order, the reduced
//         sentinel for early termination, and one pass over the input.
// Passes: the tests count how many times the source is touched, so a
//         version that chains array methods fails even when its output is
//         right.
//
// A transducer is a function (reducer) => reducer. `mapping(fn)` returns
// one; composing them builds a single reducer that does all the work in
// one pass.

/** Marks a value as final, ending the reduction. TODO: implement. */
export function reduced(_value) {
  throw new Error("TODO: implement reduced");
}

/** True for a value produced by reduced(). TODO: implement. */
export function isReduced(_value) {
  throw new Error("TODO: implement isReduced");
}

/** A transducer mapping every value with `fn`. TODO: implement. */
export function mapping(_fn) {
  throw new Error("TODO: implement mapping");
}

/** A transducer keeping the values `predicate` accepts. TODO: implement. */
export function filtering(_predicate) {
  throw new Error("TODO: implement filtering");
}

/**
 * A transducer passing through the first `count` values and then ending
 * the reduction with reduced().
 *
 * TODO: implement — it needs state per transduce() run, so create it in
 * the returned reducer-builder rather than in the outer closure.
 */
export function taking(_count) {
  throw new Error("TODO: implement taking");
}

/**
 * Composes transducers so they apply LEFT TO RIGHT to the data:
 * compose(mapping(f), filtering(p)) maps first, then filters — even
 * though the composition wraps the reducers the other way round.
 *
 * TODO: implement.
 */
export function compose(..._transducers) {
  throw new Error("TODO: implement compose");
}

/**
 * Runs a transducer over an iterable with a plain reducer and an initial
 * value, honouring reduced() and unwrapping it at the end.
 *
 * TODO: implement.
 */
export function transduce(_transducer, _reducer, _initial, _input) {
  throw new Error("TODO: implement transduce");
}

/** transduce with the array-pushing reducer. TODO: implement. */
export function into(_transducer, _input) {
  throw new Error("TODO: implement into");
}
