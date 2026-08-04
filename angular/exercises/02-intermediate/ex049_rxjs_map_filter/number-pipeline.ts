import { Observable, OperatorFunction } from "rxjs";

// Exercise 049 — map, filter and the pipe operator (intermediate).
// Goal:   transform a stream by composing small operators instead of writing logic in subscribe.
// Drills: pipe, map, filter, tap for side effects, scan for running state, and the fact that
//         operators build a *recipe* rather than doing anything until something subscribes.
// Passes: when `npx jest exercises/02-intermediate/ex049_rxjs_map_filter` is green.
//
// `pipe` composes: each operator takes the stream the previous one produced. Nothing in the chain
// runs when it is written — `pipe` returns a new Observable and the work starts on subscribe. That
// laziness is why the spec can assert "no side effect yet" partway through.
//
// The habit this exercise is really about: logic belongs in the pipeline, not in the subscribe
// callback. An `if` inside subscribe is a filter that cannot be reused, cannot be tested on its
// own, and runs after everything downstream has already been decided.
//
// `tap` is the exception that proves the rule — it exists precisely for the things that are *not*
// transformations, like logging or counting. If a tap changes the value, it is a map wearing a
// disguise, and a reader will trust it not to.
//
// `scan` is reduce that emits: one output per input, carrying the accumulator forward. Reach for
// it when the answer depends on everything seen so far — a running total, a growing list — and
// note it emits *before* the source completes, unlike reduce.

/**
 * TODO: double every value.
 *
 * Returned as an OperatorFunction so it composes into any pipe — the point being that an
 * operator is a value you can name and reuse.
 */
export function double(): OperatorFunction<number, number> {
  throw new Error("TODO: implement double");
}

/** TODO: keep only the even values. */
export function evensOnly(): OperatorFunction<number, number> {
  throw new Error("TODO: implement evensOnly");
}

/** TODO: format a number as a string with a fixed two decimals: 3 becomes "3.00". */
export function asMoney(): OperatorFunction<number, string> {
  throw new Error("TODO: implement asMoney");
}

/**
 * TODO: a running total, one emission per input.
 *
 * 1, 2, 3 produces 1, 3, 6 — using scan, which emits as it goes rather than waiting for the
 * source to complete.
 */
export function runningTotal(): OperatorFunction<number, number> {
  throw new Error("TODO: implement runningTotal");
}

/**
 * TODO: count every value that passes through, without changing any of them.
 *
 * Increments `counter.count` per value using tap — a side effect, explicitly not a transformation.
 */
export function countInto(counter: { count: number }): OperatorFunction<number, number> {
  throw new Error("TODO: implement countInto");
}

/**
 * TODO: compose the operators above into one pipeline.
 *
 * Keep the even values, double them, and format them as money: 1, 2, 3, 4 gives "4.00", "8.00".
 */
export function moneyForEvens(source: Observable<number>): Observable<string> {
  throw new Error("TODO: implement moneyForEvens");
}

/**
 * TODO: the same result written badly, for contrast.
 *
 * Subscribe to the source and do the filtering and formatting inside the callback, pushing into
 * the array you return. It works, and it is untestable, unreusable and unreadable.
 */
export function moneyForEvensImperative(source: Observable<number>): string[] {
  throw new Error("TODO: implement moneyForEvensImperative");
}
